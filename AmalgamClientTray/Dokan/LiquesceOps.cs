﻿#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="LiquesceOps.cs" company="Smurf-IV">
// 
//  Copyright (C) 2011-2012 Smurf-IV
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//   any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: http://amalgam.codeplex.com
//  Email: http://www.codeplex.com/site/users/view/smurfiv
//  </summary>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// Implement API's based on http://dokan-dev.net/en/docs/dokan-readme/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using AmalgamClientTray.ClientForms;
using AmalgamClientTray.FTP;
using DokanNet;
using Microsoft.Win32.SafeHandles;
using NLog;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace AmalgamClientTray.Dokan
{
   internal class LiquesceOps : IDokanOperations
   {
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();
      private readonly ClientShareDetail csd;
      private readonly FtpClientExt ftpCmdInstance;

      // currently open files...
      // last key
      private UInt64 openFilesLastKey;
      // lock
      private readonly ReaderWriterLockSlim openFilesSync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      // dictionary of all open files
      private readonly Dictionary<UInt64, OptimizedFTPFileReadHandler> openFiles = new Dictionary<UInt64, OptimizedFTPFileReadHandler>();

      private class CachedData
      {
         public FileSystemFTPInfo Fsi { get; set; }
         public OptimizedFTPFileReadHandler Ofh { get; set; }

         public CachedData(FileSystemFTPInfo fsi)
         {
            Fsi = fsi;
         }
      }

      private readonly CacheHelper<string, CachedData> cachedFileSystemFTPInfo;

      public LiquesceOps(ClientShareDetail csd, FtpClientExt ftpCmdInstance)
      {
         this.csd = csd;
         this.ftpCmdInstance = ftpCmdInstance;
         cachedFileSystemFTPInfo = new CacheHelper<string, CachedData>(csd.CacheInfoExpireSeconds);
      }

      #region IDokanOperations Implementation

      /// <summary>
      /// The information given in the Dokan info is a bit misleading about the return codes
      /// This is what the Win OS suystem is expecting http://msdn.microsoft.com/en-us/library/aa363858%28VS.85%29.aspx
      /// So.. Everything succeeds but the Return code is ERROR_ALREADY_EXISTS
      /// </summary>
      /// <param name="dokanFilename"></param>
      /// <param name="rawFlagsAndAttributes"></param>
      /// <param name="info"></param>
      /// <param name="rawAccessMode"></param>
      /// <param name="rawShare"></param>
      /// <param name="rawCreationDisposition"></param>
      /// <returns></returns>
      public int CreateFile(string dokanFilename, uint rawAccessMode, uint rawShare, uint rawCreationDisposition, uint rawFlagsAndAttributes, DokanFileInfo info)
      {
         int actualErrorCode = DokanNet.Dokan.DOKAN_SUCCESS;
         try
         {
            Log.Debug(
               "CreateFile IN dokanFilename [{0}], rawAccessMode[{1}], rawShare[{2}], rawCreationDisposition[{3}], rawFlagsAndAttributes[{4}], ProcessId[{5}]",
               dokanFilename, rawAccessMode, rawShare, rawCreationDisposition, rawFlagsAndAttributes, info.ProcessId);
            string path = GetPath(dokanFilename);

            CachedData foundFileInfo;
            if (!cachedFileSystemFTPInfo.TryGetValue(path, out foundFileInfo))
            {
               foundFileInfo = new CachedData(new FileFTPInfo(ftpCmdInstance, path) );
               if (!foundFileInfo.Fsi.Exists)
               {
                  foundFileInfo.Fsi = new DirectoryFTPInfo(ftpCmdInstance, path);
                  if (foundFileInfo.Fsi.Exists)
                  {
                     actualErrorCode = OpenDirectory(dokanFilename, info);
                     return actualErrorCode;
                  }
               }
               // If the directory existed it would have returned
               cachedFileSystemFTPInfo[path] = foundFileInfo;
            }
            bool fileExists = foundFileInfo.Fsi.Exists;
            if (foundFileInfo.Fsi is DirectoryFTPInfo)
            {
               if (fileExists)
               {
                  info.IsDirectory = true;
                  actualErrorCode = DokanNet.Dokan.DOKAN_SUCCESS;
               }
               else
                  actualErrorCode = DokanNet.Dokan.ERROR_PATH_NOT_FOUND;
               return actualErrorCode;
            }
            // Stop using exceptions to throw ERROR_FILE_NOT_FOUND
            // http://msdn.microsoft.com/en-us/library/aa363858%28v=vs.85%29.aspx
            switch (rawCreationDisposition)
            {
               case Proxy.CREATE_NEW:
                  if (fileExists)
                     return actualErrorCode = DokanNet.Dokan.ERROR_FILE_EXISTS;
                  break;
               case Proxy.CREATE_ALWAYS:
               case Proxy.OPEN_ALWAYS:
                  // Notepad and wordpad do not like this error code when they are writing back the file contents
                  //if (fileExists)
                  //   actualErrorCode = DokanNet.Dokan.ERROR_ALREADY_EXISTS;
                  break;
               case Proxy.OPEN_EXISTING:
               case Proxy.TRUNCATE_EXISTING:
                  if (!fileExists)
                  {
                     Log.Debug("dokanFilename [{0}] ERROR_FILE_NOT_FOUND", dokanFilename);
                     return actualErrorCode = DokanNet.Dokan.ERROR_FILE_NOT_FOUND;
                  }
                  break;
            }

            OptimizedFTPFileReadHandler Ofh = new OptimizedFTPFileReadHandler(csd, rawCreationDisposition, foundFileInfo.Fsi as FileFTPInfo, foundFileInfo.Ofh);
            
            if (foundFileInfo.Ofh == null)
               foundFileInfo.Ofh = Ofh;
            using (openFilesSync.WriteLock())
               openFiles.Add(++openFilesLastKey, Ofh);
            info.refFileHandleContext = openFilesLastKey;
         }
         catch (Exception ex)
         {
            Log.ErrorException("CreateFile threw: ", ex);
            actualErrorCode = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("CreateFile OUT actualErrorCode=[{0}] context[{1}]", actualErrorCode, openFilesLastKey);
         }
         return actualErrorCode;
      }

      private string GetPath(string dokanFilename)
      {
         return csd.TargetShareName + dokanFilename;
      }

      public int OpenDirectory(string dokanFilename, DokanFileInfo info)
      {
         int dokanError = DokanNet.Dokan.DOKAN_ERROR;
         try
         {
            Log.Trace("OpenDirectory IN DokanProcessId[{0}]", info.ProcessId);
            string path = GetPath(dokanFilename);
            CachedData foundDirInfo;
            if (!cachedFileSystemFTPInfo.TryGetValue(path, out foundDirInfo))
            {
               foundDirInfo = new CachedData( new DirectoryFTPInfo(ftpCmdInstance, path));
               cachedFileSystemFTPInfo[path] = foundDirInfo;
            }
            if (foundDirInfo.Fsi.Exists)
            {
               info.IsDirectory = true;
               dokanError = DokanNet.Dokan.DOKAN_SUCCESS;
            }
            else
               dokanError = DokanNet.Dokan.ERROR_PATH_NOT_FOUND;
         }
         finally
         {
            Log.Trace("OpenDirectory OUT. dokanError[{0}]", dokanError);
         }
         return dokanError;
      }


      public int CreateDirectory(string dokanFilename, DokanFileInfo info)
      {
         int dokanError = DokanNet.Dokan.DOKAN_ERROR;

         try
         {
            Log.Trace("CreateDirectory IN DokanProcessId[{0}]", info.ProcessId);
            string path = GetPath(dokanFilename);
            CachedData foundDirInfo;
            if (!cachedFileSystemFTPInfo.TryGetValue(path, out foundDirInfo))
            {
               foundDirInfo = new CachedData( new DirectoryFTPInfo(ftpCmdInstance, path));
               cachedFileSystemFTPInfo[path] = foundDirInfo;
            }
            if (!foundDirInfo.Fsi.Exists)
            {
               ((DirectoryFTPInfo)foundDirInfo.Fsi).Create();
               dokanError = DokanNet.Dokan.DOKAN_SUCCESS;
            }
            else
            {
               dokanError = DokanNet.Dokan.ERROR_ALREADY_EXISTS;
            }
            info.IsDirectory = true;
         }
         catch (Exception ex)
         {
            Log.ErrorException("CreateDirectory threw: ", ex);
            dokanError = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("CreateDirectory OUT dokanError[{0}]", dokanError);
         }
         return dokanError;
      }

      /*
      Cleanup is invoked when the function CloseHandle in Windows API is executed. 
      If the file system application stored file handle in the refFileHandleContext variable when the function CreateFile is invoked, 
      this should be closed in the Cleanup function, not in CloseFile function. If the user application calls CloseHandle
      and subsequently open the same file, the CloseFile function of the file system application may not be invoked 
      before the CreateFile API is called. This may cause sharing violation error. 
      Note: when user uses memory mapped file, WriteFile or ReadFile function may be invoked after Cleanup in order to 
      complete the I/O operations. The file system application should also properly work in this case.
      */
      /// <summary>
      /// When info->DeleteOnClose is true, you must delete the file in Cleanup.
      /// </summary>
      /// <param name="dokanFilename"></param>
      /// <param name="info"></param>
      /// <returns></returns>
      public int Cleanup(string dokanFilename, DokanFileInfo info)
      {
         try
         {
            Log.Trace("Cleanup IN DokanProcessId[{0}] with dokanFilename [{1}]", info.ProcessId, dokanFilename);
            CloseAndRemove(info);
            string path = GetPath(dokanFilename);
            CachedData foundInfo;
            if (cachedFileSystemFTPInfo.TryGetValue(path, out foundInfo))
            {
               if (info.DeleteOnClose)
               {
                  cachedFileSystemFTPInfo.Remove(path);
                  if (foundInfo.Fsi != null) 
                     foundInfo.Fsi.Delete();
               }
               else
               {
                  cachedFileSystemFTPInfo.Lock(path, false);
               }
            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("Cleanup threw: ", ex);
            return Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("Cleanup OUT");
         }
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int CloseFile(string dokanFilename, DokanFileInfo info)
      {
         try
         {
            Log.Trace("CloseFile IN DokanProcessId[{0}]", info.ProcessId);
         }
         catch (Exception ex)
         {
            Log.ErrorException("CloseFile threw: ", ex);
            return Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("CloseFile OUT");
         }
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }


      public int ReadFileNative(string dokanFilename, IntPtr rawBuffer, uint rawBufferLength, ref uint rawReadLength, long rawOffset, DokanFileInfo info)
      {
         int errorCode = DokanNet.Dokan.DOKAN_SUCCESS;
         bool closeOpenedContext = false;
         try
         {
            Log.Debug("ReadFile IN offset=[{1}] DokanProcessId[{0}]", info.ProcessId, rawOffset);
            rawReadLength = 0;
            string path = GetPath(dokanFilename);
            if (info.refFileHandleContext == 0)
            {
               // Some applications (like Notepad) come in "under the wire" and not via the CreateFile to perform a read
               using (openFilesSync.WriteLock())
               {
                  // Increment now in case there is an exception later
                  info.refFileHandleContext = ++openFilesLastKey; // never be Zero !
                  openFiles.Add(openFilesLastKey,
                                new OptimizedFTPFileReadHandler(csd, (uint)FileMode.Open, new FileFTPInfo(ftpCmdInstance, path), null)
                                );
                  closeOpenedContext = true;
               }
            }

            using (openFilesSync.ReadLock())
            {
               FileStreamFTP fileStream = openFiles[info.refFileHandleContext];
               if (rawOffset > fileStream.Length)
               {
                  errorCode = DokanNet.Dokan.DOKAN_ERROR;
               }
               else
               {
                  fileStream.Seek(rawOffset, SeekOrigin.Begin);

                  byte[] buf = new Byte[rawBufferLength];
                  uint readLength = fileStream.Read(buf, rawBufferLength);
                  if (readLength != 0)
                  {
                     rawReadLength = readLength;
                     Marshal.Copy(buf, 0, rawBuffer, (int)rawReadLength);
                  }
                  errorCode = DokanNet.Dokan.DOKAN_SUCCESS;
               }
            }
            if (!closeOpenedContext
               && rawReadLength > 0
               )
            {
               // A successful read means that this file must exist, so lock it in the cache
               cachedFileSystemFTPInfo.Lock(path, true);
            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("ReadFile threw: ", ex);
            errorCode = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            if (closeOpenedContext)
            {
               CloseAndRemove(info);
            }
            Log.Debug("ReadFile OUT readBytes=[{0}], errorCode[{1}]", rawReadLength, errorCode);
         }
         return errorCode;
      }

      public int WriteFileNative(string dokanFilename, IntPtr rawBuffer, uint rawNumberOfBytesToWrite, ref uint rawNumberOfBytesWritten, long rawOffset, DokanFileInfo info)
      {
         int errorCode = DokanNet.Dokan.DOKAN_SUCCESS;
         rawNumberOfBytesWritten = 0;
         try
         {
            Log.Trace("WriteFile IN DokanProcessId[{0}]", info.ProcessId);
            using (openFilesSync.ReadLock())
            {
               FileStreamFTP fileStream = openFiles[info.refFileHandleContext];
               if (info.WriteToEndOfFile)       //  If true, write to the current end of file instead of Offset parameter.
                  rawOffset = fileStream.Length;

               if (rawOffset > fileStream.Length)
               {
                  errorCode = DokanNet.Dokan.DOKAN_ERROR;
               }
               else
               {
                  fileStream.Seek(rawOffset, SeekOrigin.Begin);

                  byte[] buf = new Byte[rawNumberOfBytesToWrite];
                  Marshal.Copy(rawBuffer, buf, 0, (int)rawNumberOfBytesToWrite);

                  if (fileStream.Write(buf, rawNumberOfBytesToWrite))
                  {
                     rawNumberOfBytesWritten = rawNumberOfBytesToWrite;
                     errorCode = DokanNet.Dokan.DOKAN_SUCCESS;
                  }
               }
               if (rawNumberOfBytesWritten > 0)
               {
                  // A successful read means that this file must exist, so lock it in the cache
                  cachedFileSystemFTPInfo.Lock(GetPath(dokanFilename), true);
               }

            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("WriteFile threw: ", ex);
            errorCode = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("WriteFile OUT Written[{0}] errorCode[{1}]", rawNumberOfBytesWritten, errorCode);
         }
         return errorCode;
      }


      public int FlushFileBuffers(string dokanFilename, DokanFileInfo info)
      {
         // TODO: Will probably be useful for Asynch operations
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int GetFileInformation(string dokanFilename, ref FileInformation fileinfo, DokanFileInfo info)
      {
         int dokanReturn = DokanNet.Dokan.ERROR_FILE_NOT_FOUND;
         try
         {
            Log.Trace("GetFileInformation IN DokanProcessId[{0}]", info.ProcessId);
            string path = GetPath(dokanFilename);

            CachedData fsi;
            if (!cachedFileSystemFTPInfo.TryGetValue(path, out fsi))
            {
               FileFTPInfo foundFileInfo = new FileFTPInfo(ftpCmdInstance, path);
               bool fileExists = foundFileInfo.Exists;
               if (!fileExists)
               {
                  DirectoryFTPInfo foundDirInfo = new DirectoryFTPInfo(ftpCmdInstance, path);
                  if (foundDirInfo.Exists)
                  {
                     OpenDirectory(dokanFilename, info);
                     fsi.Fsi = foundDirInfo;
                  }
                  else
                  {
                     cachedFileSystemFTPInfo[path] = new CachedData(foundFileInfo);
                  }
               }
               else
                  fsi.Fsi = foundFileInfo;
            }
            if ((fsi != null)
               && (fsi.Fsi != null)
               && (fsi.Fsi.Exists)
               )
            {
               fileinfo = ConvertToDokan(fsi.Fsi, info.IsDirectory);
               dokanReturn = DokanNet.Dokan.DOKAN_SUCCESS;
            }

         }
         catch (Exception ex)
         {
            Log.ErrorException("FlushFileBuffers threw: ", ex);
            dokanReturn = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("GetFileInformation OUT Attributes[{0}] Length[{1}] dokanReturn[{2}]", fileinfo.Attributes, fileinfo.Length, dokanReturn);
         }
         return dokanReturn;
      }

      public int FindFilesWithPattern(string dokanFilename, string pattern, out FileInformation[] files, DokanFileInfo info)
      {
         throw new NotImplementedException("FindFilesWithPattern");
      }

      public int FindFiles(string dokanFilename, out FileInformation[] files, DokanFileInfo info)
      {
         return FindFiles(dokanFilename, out files);
      }

      private int FindFiles(string dokanFilename, out FileInformation[] files, string pattern = "*")
      {
         files = null;
         try
         {
            Log.Debug("FindFiles IN [{0}], pattern[{1}]", dokanFilename, pattern);
            List<FileInformation> uniqueFiles = new List<FileInformation>();
            DirectoryFTPInfo dirInfo = new DirectoryFTPInfo(ftpCmdInstance, GetPath(dokanFilename));
            if (dirInfo.Exists)
            {
               FileSystemFTPInfo[] fileSystemInfos = dirInfo.GetFileSystemInfos(pattern, SearchOption.TopDirectoryOnly);
               foreach (FileSystemFTPInfo info2 in fileSystemInfos)
               {
                  AddToUniqueLookup(info2, uniqueFiles);
               }
            }

            files = new FileInformation[uniqueFiles.Count];
            uniqueFiles.CopyTo(files, 0);
         }
         catch (Exception ex)
         {
            Log.ErrorException("FindFiles threw: ", ex);
            return Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Debug("FindFiles OUT [found {0}]", (files != null ? files.Length : 0));
            if (Log.IsTraceEnabled)
            {
               if (files != null)
               {
                  StringBuilder sb = new StringBuilder();
                  sb.AppendLine();
                  foreach (FileInformation fileInformation in files)
                  {
                     sb.AppendLine(fileInformation.FileName);
                  }
                  Log.Trace(sb.ToString());
               }
            }
         }
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int SetFileAttributes(string dokanFilename, FileAttributes attr, DokanFileInfo info)
      {
         try
         {
            Log.Trace("SetFileAttributes IN DokanProcessId[{0}]", info.ProcessId);
            throw new NotImplementedException("SetFileAttributes");
            //string path = GetPath(dokanFilename);
            //// This uses  if (!Win32Native.SetFileAttributes(fullPathInternal, (int) fileAttributes))
            //// And can throw PathTOOLong
            //File.SetAttributes(path, attr);
         }
         catch (Exception ex)
         {
            Log.ErrorException("SetFileAttributes threw: ", ex);
            return Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("SetFileAttributes OUT");
         }
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int SetFileTimeNative(string dokanFilename, ref FILETIME rawCreationTime, ref FILETIME rawLastAccessTime,
          ref FILETIME rawLastWriteTime, DokanFileInfo info)
      {
         SafeFileHandle safeFileHandle = null;
         bool needToClose = false;
         try
         {
            Log.Trace("SetFileTime IN DokanProcessId[{0}]", info.ProcessId);
            throw new NotImplementedException("SetFileTimeNative");
            //using (openFilesSync.ReadLock())
            //{
            //   if (info.refFileHandleContext != 0)
            //   {
            //      Log.Trace("info.refFileHandleContext [{0}]", info.refFileHandleContext);
            //      safeFileHandle = openFiles[info.refFileHandleContext].SafeFileHandle;
            //   }
            //   else
            //   {
            //      // Workaround the dir set
            //      // ERROR LiquesceSvc.LiquesceOps: SetFileTime threw:  System.UnauthorizedAccessException: Access to the path 'G:\_backup\Kylie Minogue\Dir1' is denied.
            //      // To create a handle to a directory, you have to use FILE_FLAG_BACK_SEMANTICS.
            //      string path = GetPath(dokanFilename);
            //      const uint rawAccessMode = Proxy.GENERIC_READ | Proxy.GENERIC_WRITE;
            //      const uint rawShare = Proxy.FILE_SHARE_READ | Proxy.FILE_SHARE_WRITE;
            //      const uint rawCreationDisposition = Proxy.OPEN_EXISTING;
            //      uint rawFlagsAndAttributes = Directory.Exists(path) ? Proxy.FILE_FLAG_BACKUP_SEMANTICS : 0;
            //      safeFileHandle = CreateFile(path, rawAccessMode, rawShare, IntPtr.Zero, rawCreationDisposition, rawFlagsAndAttributes, IntPtr.Zero);
            //      needToClose = true;
            //   }
            //   if ((safeFileHandle != null)
            //      && !safeFileHandle.IsInvalid
            //      )
            //   {
            //      if (!SetFileTime(safeFileHandle, ref rawCreationTime, ref rawLastAccessTime, ref rawLastWriteTime))
            //      {
            //         Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error(), new IntPtr(-1));
            //      }
            //   }
            //}
         }
         catch (Exception ex)
         {
            Log.ErrorException("SetFileTime threw: ", ex);
            return Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            try
            {
               if (needToClose
                   && (safeFileHandle != null)
                   && !safeFileHandle.IsInvalid
                  )
               {
                  safeFileHandle.Close();
               }
            }
            catch
            {
            }
            Log.Trace("SetFileTime OUT");
         }
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      /// <summary>
      /// You should not delete file on DeleteFile or DeleteDirectory.
      // When DeleteFile or DeleteDirectory, you must check whether
      // you can delete or not, and return 0 (when you can delete it)
      // or appropriate error codes such as -ERROR_DIR_NOT_EMPTY,
      // -ERROR_SHARING_VIOLATION.
      // When you return 0 (ERROR_SUCCESS), you get Cleanup with
      // FileInfo->DeleteOnClose set TRUE, you delete the file.
      //
      /// </summary>
      /// <param name="dokanFilename"></param>
      /// <param name="info"></param>
      /// <returns></returns>
      public int DeleteFile(string dokanFilename, DokanFileInfo info)
      {
         int dokanReturn = DokanNet.Dokan.DOKAN_ERROR;
         try
         {
            Log.Trace("DeleteFile IN DokanProcessId[{0}]", info.ProcessId);
            throw new NotImplementedException("DeleteFile");
            // dokanReturn = (File.Exists(GetPath(dokanFilename)) ? DokanNet.Dokan.DOKAN_SUCCESS : DokanNet.Dokan.ERROR_FILE_NOT_FOUND);
         }
         catch (Exception ex)
         {
            Log.ErrorException("DeleteFile threw: ", ex);
            dokanReturn = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("DeleteFile OUT dokanReturn[(0}]", dokanReturn);
         }
         return dokanReturn;
      }

      public int DeleteDirectory(string dokanFilename, DokanFileInfo info)
      {
         int dokanReturn = DokanNet.Dokan.DOKAN_ERROR;
         try
         {
            Log.Trace("DeleteDirectory IN DokanProcessId[{0}]", info.ProcessId);
            throw new NotImplementedException("DeleteDirectory");
            //string path = GetPath(dokanFilename);
            //DirectoryInfo dirInfo = new DirectoryInfo(path);
            //if (dirInfo.Exists)
            //{
            //   FileSystemInfo[] fileInfos = dirInfo.GetFileSystemInfos();
            //   dokanReturn = (fileInfos.Length > 0) ? DokanNet.Dokan.ERROR_DIR_NOT_EMPTY : DokanNet.Dokan.DOKAN_SUCCESS;
            //}
            //else
            //   dokanReturn = DokanNet.Dokan.ERROR_FILE_NOT_FOUND;
         }
         catch (Exception ex)
         {
            Log.ErrorException("DeleteDirectory threw: ", ex);
            dokanReturn = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("DeleteDirectory OUT dokanReturn[(0}]", dokanReturn);
         }

         return dokanReturn;
      }


      public int MoveFile(string dokanFilename, string newname, bool replaceIfExisting, DokanFileInfo info)
      {
         try
         {
            Log.Trace("MoveFile IN DokanProcessId[{0}]", info.ProcessId);
            throw new NotImplementedException("MoveFile");
            //Log.Info("MoveFile replaceIfExisting [{0}] dokanFilename: [{1}] newname: [{2}]", replaceIfExisting, dokanFilename, newname);
            //if (dokanFilename == newname)   // This is some weirdness that SyncToy tries to pull !!
            //   return DokanNet.Dokan.DOKAN_SUCCESS;

            //string pathTarget = GetPath(newname, true);

            //if (!info.IsDirectory)
            //{
            //   if (info.refFileHandleContext != 0)
            //   {
            //      Log.Trace("info.refFileHandleContext [{0}]", info.refFileHandleContext);
            //      FileStreamName fileHandle = openFiles[info.refFileHandleContext];
            //      if (fileHandle != null)
            //         fileHandle.Close();
            //   }
            //   string pathSource = GetPath(dokanFilename);
            //   Log.Info("MoveFile pathSource: [{0}] pathTarget: [{1}]", pathSource, pathTarget);
            //   XMoveFile(pathSource, pathTarget, replaceIfExisting);
            //   roots.RemoveTargetFromLookup(pathSource);
            //}
            //else
            //{

            //   // getting all paths of the source location
            //   List<string> allDirSources = roots.GetAllPaths(dokanFilename);
            //   if (allDirSources.Count == 0)
            //   {
            //      Log.Error("MoveFile: Could not find directory [{0}]", dokanFilename);
            //      return DokanNet.Dokan.DOKAN_ERROR;
            //   }
            //   // rename every 
            //   foreach (string dirSource in allDirSources)
            //   {
            //      string dirTarget = Roots.GetRoot(dirSource) + newname;
            //      filemanager.XMoveDirectory(dirSource, dirTarget, replaceIfExisting);
            //   }
            //}
         }
         catch (Exception ex)
         {
            Log.ErrorException("MoveFile threw: ", ex);
            return Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("MoveFile OUT");
         }
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int SetEndOfFile(string dokanFilename, long length, DokanFileInfo info)
      {
         Log.Trace("SetEndOfFile IN DokanProcessId[{0}]", info.ProcessId);
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int SetAllocationSize(string dokanFilename, long length, DokanFileInfo info)
      {
         Log.Trace("SetAllocationSize IN DokanProcessId[{0}]", info.ProcessId);
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int LockFile(string dokanFilename, long offset, long length, DokanFileInfo info)
      {
         Log.Trace("LockFile IN DokanProcessId[{0}]", info.ProcessId);
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int UnlockFile(string dokanFilename, long offset, long length, DokanFileInfo info)
      {
         Log.Trace("UnlockFile IN DokanProcessId[{0}]", info.ProcessId);
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int GetDiskFreeSpace(ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
      {
         int dokanReturn = DokanNet.Dokan.DOKAN_ERROR;
         try
         {
            Log.Trace("GetDiskFreeSpace IN DokanProcessId[{0}]", info.ProcessId);
            ftpCmdInstance.GetDiskFreeSpace(csd.BufferWireTransferSize, ref freeBytesAvailable, ref totalBytes, ref totalFreeBytes);
            dokanReturn = DokanNet.Dokan.DOKAN_SUCCESS;
         }
         catch (Exception ex)
         {
            Log.ErrorException("UnlockFile threw: ", ex);
            dokanReturn = Utils.BestAttemptToWin32(ex);
         }
         finally
         {
            Log.Trace("DeleteDirectory OUT dokanReturn[(0}]", dokanReturn);
         }
         return dokanReturn;
      }

      public int Unmount(DokanFileInfo info)
      {
         Log.Trace("Unmount IN DokanProcessId[{0}]", info.ProcessId);
         using (openFilesSync.WriteLock())
         {
            if (openFiles != null)
            {
               foreach (OptimizedFTPFileReadHandler obj2 in openFiles.Values)
               {
                  try
                  {
                     if (obj2 != null)
                     {
                        obj2.Close();
                     }
                  }
                  catch (Exception ex)
                  {
                     Log.InfoException("Unmount closing files threw: ", ex);
                  }
               }
               openFiles.Clear();
            }
         }
         Log.Trace("Unmount out");
         return DokanNet.Dokan.DOKAN_SUCCESS;
      }

      public int GetFileSecurityNative(string file, ref SECURITY_INFORMATION rawRequestedInformation, ref SECURITY_DESCRIPTOR rawSecurityDescriptor, uint rawSecurityDescriptorLength, ref uint rawSecurityDescriptorLengthNeeded, DokanFileInfo info)
      {
         throw new NotImplementedException("GetFileSecurityNative");
      }

      public int SetFileSecurityNative(string file, ref SECURITY_INFORMATION rawSecurityInformation, ref SECURITY_DESCRIPTOR rawSecurityDescriptor, uint rawSecurityDescriptorLength, DokanFileInfo info)
      {
         throw new NotImplementedException("SetFileSecurityNative");
      }

      #endregion


      private void AddToUniqueLookup(FileSystemFTPInfo info2, List<FileInformation> files)
      {
         bool isDirectoy = (info2.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
         if ( cachedFileSystemFTPInfo[info2.FullName] == null )
            cachedFileSystemFTPInfo[info2.FullName] = new CachedData(info2);

         FileInformation item = ConvertToDokan(info2, isDirectoy);
         files.Add(item);
      }

      private static FileInformation ConvertToDokan(FileSystemFTPInfo info2, bool isDirectoy)
      {
         // The NTFS file system records times on disk in UTC
         // see http://msdn.microsoft.com/en-us/library/ms724290%28v=vs.85%29.aspx
         return new FileInformation
                   {
                      // Prevent expensive time spent allowing indexing == FileAttributes.NotContentIndexed
                      // Prevent the system from timing out due to slow access through the driver == FileAttributes.Offline
                      Attributes = info2.Attributes | FileAttributes.NotContentIndexed | ((Log.IsTraceEnabled) ? FileAttributes.Offline : 0),
                      CreationTime = info2.CreationTimeUtc,
                      LastAccessTime = info2.LastWriteTimeUtc, // Not supported by FTP
                      LastWriteTime = info2.LastWriteTimeUtc,
                      Length = (isDirectoy) ? 0L : info2.Length,
                      FileName = info2.Name
                   };
      }

      private void CloseAndRemove(DokanFileInfo info)
      {
         if (info.refFileHandleContext != 0)
         {
            Log.Trace("CloseAndRemove info.refFileHandleContext [{0}]", info.refFileHandleContext);
            using (openFilesSync.UpgradableReadLock())
            {
               // The File can be closed by the remote client via Delete (as it does not know to close first!)
               OptimizedFTPFileReadHandler fileStream;
               if (openFiles.TryGetValue(info.refFileHandleContext, out fileStream))
               {
                  using (openFilesSync.WriteLock())
                  {
                     openFiles.Remove(info.refFileHandleContext);
                  }
                  Log.Trace("CloseAndRemove [{0}] info.refFileHandleContext[{1}]", fileStream.FullName, info.refFileHandleContext);
                  fileStream.Close();
               }
               else
               {
                  Log.Debug("Something has already closed info.refFileHandleContext [{0}]", info.refFileHandleContext);
               }
            }
            info.refFileHandleContext = 0;
         }
         else
         {
            Log.Warn("CloseAndRemove has a zero!");
         }
      }
   }

}