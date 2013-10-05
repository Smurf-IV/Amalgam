#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="HandleMappingThread.cs" company="Smurf-IV">
// 
//  Copyright (C) 2013 Simon Coghlan (aka Smurf-IV)
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using AmalgamClientTray.ClientForms;
using AmalgamClientTray.Dokan;
using AmalgamClientTray.FTP;
using NLog;

namespace CBFS
{
   public class HandleMappingThread : ICBFSFuncs
   {
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();
      private readonly ClientShareDetail csd;
      private readonly CBFSHandlers CBFSHandlerThread;
      private readonly FtpClientExt ftpCmdInstance;

      // currently open files...
      // last key
      private UInt64 openFilesLastKey;
      // lock
      private readonly ReaderWriterLockSlim openFilesSync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
      // dictionary of all open files
      private readonly Dictionary<UInt64, CachedData> openFiles = new Dictionary<UInt64, CachedData>();

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

      public HandleMappingThread(ClientShareDetail clientShareDetail)
      {
         csd = clientShareDetail;
         CBFSHandlerThread = new CBFSHandlers(this, false);
      }

      public bool Start()
      {
         CBFSHandlerThread.MountMedia(csd.APITimeoutms);

         // This call will create mounting point in the current local session.
         // Consider that in this case the mounting point will not be visible
         // in other user logon sessions. 
         CBFSHandlerThread.AddMountingPoint(csd.DriveLetter);

         return true;
      }

      public bool Stop()
      {
         CBFSHandlerThread.UnmountMedia();
         CBFSHandlerThread.DeleteStorage(true);

         return true;
      }

      public void Mounted()
      {
         throw new NotImplementedException();
      }

      public void Unmounted()
      {
         throw new NotImplementedException();
      }

      public void GetVolumeSize(ref long TotalNumberOfSectors, ref long NumberOfFreeSectors)
      {
         throw new NotImplementedException();
      }

      public void OpenVolume()
      {
         throw new NotImplementedException();
      }

      public void CloseVolume()
      {
         throw new NotImplementedException();
      }

      public UInt64 CreateFile(string fileName, FileMode desiredAccess, FileAttributes fileAttributes, uint extFlagsAndAttributes, FileShare shareMode)
      {
         fileName = GetPath(fileName);
         CachedData foundFileInfo;
         bool isDirectory = ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory);
         bool requestCreate = false;
         switch (desiredAccess)
         {
            case FileMode.CreateNew:
            case FileMode.Create:
            case FileMode.OpenOrCreate:
               requestCreate = true;
         }
         if (!cachedFileSystemFTPInfo.TryGetValue(fileName, out foundFileInfo))
         {
            foundFileInfo = new CachedData(isDirectory?(FileSystemFTPInfo) new DirectoryFTPInfo(ftpCmdInstance, fileName)
                                          :new FileFTPInfo(ftpCmdInstance, fileName));
            if (!foundFileInfo.Fsi.Exists)
            {
               if ( !requestCreate )
                  throw new Win32Exception(CBFSWinError.ERROR_FILE_NOT_FOUND);
               if (csd.TargetIsReadonly)
                  throw new Win32Exception(CBFSWinError.ERROR_FILE_READ_ONLY);
               ulong handle = isDirectory ? InternalCreateDirectory(fileName, desiredAccess)
                  :InternalOpenFile(fileName, desiredAccess);
               SetFileAttributes(fileName, handle, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, fileAttributes, extFlagsAndAttributes);
               return handle;
            }
         }
         return InternalOpenFile(fileName, desiredAccess);
      }

      private UInt64 InternalCreateDirectory(string dirName, FileMode desiredAccess)
      {
         try
         {
            Log.Trace("InternalCreateDirectory IN dirName[{0}] desiredAccess[{1}]", dirName, desiredAccess);
            if (csd.FileNamesToIgnore.Any(dirName.EndsWith))
            {
               throw new Win32Exception(CBFSWinError.ERROR_FILE_NOT_FOUND);
            }

            string path = GetPath(dirName);
            CachedData foundDirInfo;
            if (!cachedFileSystemFTPInfo.TryGetValue(path, out foundDirInfo))
            {
               foundDirInfo = new CachedData( new DirectoryFTPInfo(ftpCmdInstance, path));
               cachedFileSystemFTPInfo[path] = foundDirInfo;
            }
            if (!foundDirInfo.Fsi.Exists)
            {
               ((DirectoryFTPInfo)foundDirInfo.Fsi).Create();
               using (openFilesSync.WriteLock())
               {
                  ulong key = ++openFilesLastKey;
                  openFiles.Add(key, foundDirInfo);
                  return key;
               }
            }
            else
            {
               throw new Win32Exception(CBFSWinError.ERROR_ALREADY_EXISTS);
            }
         }
         finally
         {
            Log.Trace("InternalCreateDirectory OUT");
         }
      }

      public UInt64 OpenFile(string fileName, FileMode desiredAccess, FileShare shareMode)
      {
         return InternalOpenFile(fileName, desiredAccess);
      }

      private UInt64 InternalOpenFile(string fileName, FileMode desiredAccess)
      {
         try
         {
            Log.Debug("InternalOpenFile IN fileName [{0}], DesiredAccess[{1}]", fileName, desiredAccess);
            if ( string.IsNullOrWhiteSpace(fileName))
               throw new ArgumentNullException("fileName", "Not allowed to pass a null filename");
            if (csd.FileNamesToIgnore.Any(fileName.EndsWith))
            {
               throw new Win32Exception(CBFSWinError.ERROR_FILE_NOT_FOUND);
            }
            fileName = GetPath(fileName);
            CachedData foundFileInfo;
            if (!cachedFileSystemFTPInfo.TryGetValue(fileName, out foundFileInfo))
            {
               foundFileInfo = new CachedData(new FileFTPInfo(ftpCmdInstance, fileName));
               if (!foundFileInfo.Fsi.Exists)
               {
                  foundFileInfo = new CachedData(new DirectoryFTPInfo(ftpCmdInstance, fileName));
               }
               cachedFileSystemFTPInfo[fileName] = foundFileInfo;
            }
            if (desiredAccess != FileMode.Open)
               if (csd.TargetIsReadonly)
                  throw new Win32Exception(CBFSWinError.ERROR_FILE_READ_ONLY);

            OptimizedFTPFileReadHandler Ofh = new OptimizedFTPFileReadHandler(csd, desiredAccess, foundFileInfo.Fsi as FileFTPInfo, foundFileInfo.Ofh);

            if (foundFileInfo.Ofh == null)
               throw new Win32Exception(CBFSWinError.ERROR_BAD_ARGUMENTS);
            foundFileInfo.Ofh = Ofh;
            using (openFilesSync.WriteLock())
            {
               ulong key = ++openFilesLastKey;
               openFiles.Add(key, foundFileInfo);
               return key;
            }
         }
         finally
         {
            Log.Trace("InternalOpenFile OUT");
         }
      }

      private string GetPath(string fileName)
      {
         return csd.TargetShareName + fileName;
      }

      public void CloseFile(string fileName, UInt64 fileHandle)
      {
         throw new NotImplementedException();
      }

      public void GetFileInfo(string fileName, ref bool FileExists, ref DateTime CreationTime, ref DateTime LastAccessTime,
                              ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize, ref long FileId,
                              ref uint FileAttributes, ref string LongFileName)
      {
         FileExists = false;

         VirtualFile vfile = null;

         if (FindVirtualFile(FileName, ref vfile))
         {
            FileExists = true;

            CreationTime = vfile.CreationTime;

            LastAccessTime = vfile.LastAccessTime;

            LastWriteTime = vfile.LastWriteTime;

            EndOfFile = vfile.EndOfFile;

            AllocationSize = vfile.AllocationSize;

            FileId.QuadPart = 0;

            FileAttributes = Convert.ToUInt32(vfile.Attributes);
         }
      }

      public void EnumerateDirectory(string DirectoryName, ref IntPtr EnumerationContext, string Mask, int Index, bool Restart,
                                     ref bool FileFound, ref string fileName, ref string ShortFileName, ref DateTime CreationTime,
                                     ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile,
                                     ref long AllocationSize, ref long FileId, ref uint FileAttributes)
      {
         bool ResetEnumeration = false;
         bool ExactMatch = false;
         FileFound = false;
         ENUM_INFO pInfo = null;
         VirtualFile vfile = null, vdir = null;

         ExactMatch = !Mask.Equals("*");

         if ((Restart || EnumerationContext == IntPtr.Zero) &&
             !ExactMatch)
            ResetEnumeration = true;


         if (Restart && (EnumerationContext != IntPtr.Zero))
         {
            if (GCHandle.FromIntPtr(EnumerationContext).IsAllocated)
            {
               GCHandle.FromIntPtr(EnumerationContext).Free();
            }
            EnumerationContext = IntPtr.Zero;
         }
         if (EnumerationContext.Equals(IntPtr.Zero))
         {
            pInfo = new ENUM_INFO();

            GCHandle gch = GCHandle.Alloc(pInfo);

            EnumerationContext = GCHandle.ToIntPtr(gch);

            FindVirtualFile(DirectoryInfo.FileName, ref vdir);

            pInfo.vfile = vdir;

         }
         else
         {
            pInfo = (ENUM_INFO)GCHandle.FromIntPtr(EnumerationContext).Target;

            vdir = pInfo.vfile;
         }

         if (ResetEnumeration)
            pInfo.Index = 0;

         if (pInfo.ExactMatch == true)
         {
            FileFound = false;
         }
         else
         {
            FileFound = ExactMatch ?
                vdir.Context.GetFile(Mask, ref vfile) : vdir.Context.GetFile(pInfo.Index, ref vfile);
         }

         if (FileFound)
         {
            FileName = vfile.Name;

            FileNameLength = Convert.ToUInt16(vfile.Name.Length);

            CreationTime = vfile.CreationTime;

            LastAccessTime = vfile.LastAccessTime;

            LastWriteTime = vfile.LastWriteTime;

            EndOfFile = vfile.EndOfFile;

            AllocationSize = vfile.AllocationSize;

            FileId.QuadPart = 0;

            FileAttributes = Convert.ToUInt32(vfile.Attributes);

         }
         pInfo.ExactMatch = ExactMatch;
         ++pInfo.Index;
      }

      public void SetAllocationSize(string fileName, UInt64 fileHandle, long AllocationSize)
      {
         throw new NotImplementedException();
      }

      public void SetEndOfFile(string fileName, UInt64 fileHandle, long EndOfFile)
      {
         throw new NotImplementedException();
      }

      public void SetFileAttributes(string fileName, UInt64 fileHandle, DateTime CreationTime, DateTime LastAccessTime,
                                    DateTime LastWriteTime, FileAttributes fileAttributes, uint extFlagsAndAttributes)
      {
         GCHandle gch = GCHandle.FromIntPtr(FileHandleContext);

         VirtualFile vfile = (VirtualFile)gch.Target;

         // the case when FileAttributes == 0 indicates that file attributes
         // not changed during this callback

         if (Attributes != 0)
            vfile.Attributes = (FileAttributes)Attributes;

         if (CreationTime != DateTime.MinValue)
         {
            vfile.CreationTime = CreationTime;
         }
         if (LastAccessTime != DateTime.MinValue)
         {
            vfile.LastAccessTime = LastAccessTime;
         }
         if (LastWriteTime != DateTime.MinValue)
         {
            vfile.LastWriteTime = LastWriteTime;
         }
      }

      public bool CanFileBeDeleted(string fileName, UInt64 fileHandle)
      {
         throw new NotImplementedException();
      }

      public void DeleteFile(string fileName, UInt64 fileHandle)
      {
         throw new NotImplementedException();
      }

      public void RenameOrMoveFile(string fileName, string NewFileName)
      {
         throw new NotImplementedException();
      }

      public int ReadFile(string fileName, UInt64 fileHandle, long Position, byte[] Buffer, int BytesToRead)
      {
         throw new NotImplementedException();
      }

      public int WriteFile(string fileName, UInt64 fileHandle, long Position, byte[] Buffer, int BytesToWrite)
      {
         throw new NotImplementedException();
      }

      public void CloseEnumeration(string DirectoryName, IntPtr EnumerationContext)
      {
         throw new NotImplementedException();
      }

      public bool IsDirectoryEmpty(string FileInfo)
      {
         throw new NotImplementedException();
      }

      public void EnumerateNamedStreams(string fileName, UInt64 fileHandle, ref IntPtr NamedStreamContext, ref string StreamName,
                                        ref long StreamSize, ref long StreamAllocationSize, ref bool NamedStreamFound)
      {
         throw new NotImplementedException();
      }

      public void SetFileSecurity(string fileName, UInt64 fileHandle, uint SecurityInformation, IntPtr SecurityDescriptor,
                                  uint Length)
      {
         throw new NotImplementedException();
      }

      public void GetFileSecurity(string fileName, UInt64 fileHandle, uint SecurityInformation, IntPtr SecurityDescriptor,
                                  uint Length, ref uint LengthNeeded)
      {
         throw new NotImplementedException();
      }

      public string GetFileNameByFileId(long FileId)
      {
         throw new NotImplementedException();
      }

      public void FlushFile(string fileName, UInt64 fileHandle)
      {
         throw new NotImplementedException();
      }

      public void StorageEjected()
      {
         throw new NotImplementedException();
      }

      public string VolumeLabel { get; set; }
      public uint VolumeID { get; private set; }
   }
}