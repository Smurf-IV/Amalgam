#region Copyright (C)

// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ICBFSFunc.cs" company="Smurf-IV">
//
//  Copyright (C) 2013-2014 Simon Coghlan (Aka Smurf-IV)
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
//  Email: http://www.codeplex.com/site/users/view/smurfiv
//  </summary>
// --------------------------------------------------------------------------------------------------------------------

#endregion Copyright (C)

using System;
using System.Resources;
using System.Runtime.InteropServices;
using CallbackFS;
using NLog;
using StringBuffers;

namespace CBFS
{
   // ReSharper disable UnusedMemberInSuper.Global
   internal interface ICBFSFuncs
   {
      #region Basic CBFS functions

      /// <summary>
      /// This event is fired after Callback File System mounts the storage and makes it available.
      /// The event is optional - you don't have to handle it.
      /// </summary>
      void Mount();

      /// <summary>
      /// This event is fired after Callback File System unmounts the storage and it becomes unavailable for the system.
      /// The event is optional - you don't have to handle it.
      /// </summary>
      void UnMount();

      /// <summary>
      /// This event is fired when the OS wants to obtain information about the size and available space on the disk.
      /// Minimal size of the volume accepted by Windows is 6144 bytes (based on 3072-byte sector, 1 sector per cluster
      /// and 2 clusters per storage), however CBFS adjusts the size to be at least 16 sectors to ensure compatibility
      /// with possible changes in future versions of Windows.
      /// </summary>
      /// <param name="TotalNumberOfSectors">Tthe total number of the allocation units (clusters) on device to this parameter </param>
      /// <param name="NumberOfFreeSectors">The number of available (free) allocation units (clusters) on device to this parameter </param>
      void GetVolumeSize(out long TotalNumberOfSectors, out long NumberOfFreeSectors);

      /// <summary>
      /// This event is fired when the OS wants to obtain the volume label.
      /// Maximum length of the label supported by the OS is 32 characters.
      /// </summary>
      string VolumeLabel { get; set; }

      /// <summary>
      /// This event is fired when Callback File System wants to obtain the volume Id.
      /// The volume Id is unique user defined value (within Callback File System volumes).
      /// </summary>
      uint VolumeId { get; set; }

      /// <summary>
      /// This event is fired when the OS wants to create a file or directory with given name and attributes.
      /// The directories are created with this call.
      /// To check, what should be created (file or directory), check FileAttributes as follows:
      ///   Directory = FileAttributes & FILE_ATTRIBUTE_DIRECTORY == FILE_ATTRIBUTE_DIRECTORY;
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="DesiredAccess"></param>
      /// <param name="fileAttributes"></param>
      /// <param name="ShareMode"></param>
      /// <param name="fileInfo"></param>
      /// <param name="userContextInfo"></param>
      void CreateFile(string FileName, uint DesiredAccess, uint fileAttributes, uint ShareMode,
         CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      /// <summary>
      /// This event is fired when the OS wants to open an existing file or directory with given name and attributes.
      /// The directory can be opened, for example, in order to change its attributes or to enumerate its contents.
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="DesiredAccess"></param>
      /// <param name="fileAttributes"></param>
      /// <param name="ShareMode"></param>
      /// <param name="fileInfo"></param>
      /// <param name="userContextInfo"></param>
      void OpenFile(string FileName, uint DesiredAccess, uint fileAttributes, uint ShareMode, CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      /// <summary>
      /// This event is fired when the OS needs to close the previously created or opened file.
      /// Use FileInfo and HandleInfo to identify the file that needs to be closed.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="userContextInfo"></param>
      void CloseFile(CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      /// <summary>
      /// This event is fired when the OS needs to close the previously created or opened handle to the file.
      /// This event is different from OnCloseFile in that OnCleanupFile happens immediately when the last handle is
      /// closed by the application, while OnCloseFile can be called much later when the OS itself decides that the
      /// file can be closed. Use FileInfo and HandleInfo to identify the file that needs to be closed.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="userContextInfo"></param>
      void CleanupFile(CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      /// <summary>
      /// The event is fired when the OS needs to get information about the file or directory.
      /// If the file exists, FileExists parameter must be set to true and all information (besides optional parameters,
      /// as specified above) must be set.
      /// If the file doesn't exist, then FileExists must be set to false. In this case no parameters are read back.
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="FileExists"></param>
      /// <param name="CreationTime"></param>
      /// <param name="LastAccessTime"></param>
      /// <param name="LastWriteTime"></param>
      /// <param name="lengthOfFile">the size of the file data in bytes to this parameter</param>
      /// <param name="AllocationSize"></param>
      /// <param name="FileId"></param>
      /// <param name="FileAttributes"></param>
      /// <param name="ShortFileName"></param>
      /// <param name="RealFileName"></param>
      void GetFileInfo(string FileName, ref bool FileExists, ref DateTime CreationTime, ref DateTime LastAccessTime,
         ref DateTime LastWriteTime, ref long lengthOfFile, ref long AllocationSize,
         ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes, ref string ShortFileName, ref string RealFileName);

      /// <summary>
      /// This event is fired when the OS wants to enumerate the directory entries by mask.
      /// </summary>
      /// <param name="directoryInfo">information about the directory which is being read. This structure is common to all directory operations that occur at the same time in parallel.</param>
      /// <param name="HandleInfo">information about particular directory handle which was opened for the purpose of directory enumeration</param>
      /// <param name="DirectoryEnumerationInfo">information about current enumeration </param>
      /// <param name="Mask"></param>
      /// The mask can (but not necessarily does) include wildcard characters ("*" and "?") and any characters,
      /// allowed in file names, in any combination. Eg. you can recieve masks like "smth?*.abc?e?*" and other complex combinations.
      /// <param name="Restart">signals that the request for the entry was already done, but the entry should be searched for once again, starting at the beginning of the directory </param>
      /// <param name="FileFound">set this parameter to true if the file exists and the information is provided and false otherwise </param>
      /// <param name="FileName">set this parameter to the name of the found file or directory</param>
      /// <param name="ShortFileName">if short file name support is enabled, place the short name of the file to this parameter </param>
      /// <param name="CreationTime"></param>
      /// <param name="LastAccessTime"></param>
      /// <param name="LastWriteTime"></param>
      /// <param name="lengthOfFile">Size of the file data in bytes</param>
      /// <param name="AllocationSize">Size of the space in bytes, allocated for the file, to this parameter. The allocation size is usually a multiple of the allocation unit size.</param>
      /// <param name="FileId">Currently unused.</param>
      /// <param name="FileAttributes"></param>
      void EnumerateDirectory(CbFsFileInfo directoryInfo, CbFsHandleInfo HandleInfo,
         CbFsDirectoryEnumerationInfo DirectoryEnumerationInfo, string Mask, bool Restart,
         ref bool FileFound, ref string FileName, ref string ShortFileName,
         ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long lengthOfFile,
         ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes);

      /// <summary>
      /// This event is fired when the OS has finished enumerating the directory contents and requests the resources,
      /// allocated for enumeration, to be released.
      /// </summary>
      /// <param name="directoryInfo"></param>
      /// <param name="directoryEnumerationInfo"></param>
      void CloseDirectoryEnumeration(CbFsFileInfo directoryInfo, CbFsDirectoryEnumerationInfo directoryEnumerationInfo);

      /// <summary>
      /// AllocationSize is usually larger (and much larger) than the size of the file data.
      /// This happens because some file operations first reserve space for the file, then start writing actual data to this file.
      /// The application should track such situations and avoid re-allocating file space where possible to improve speed.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="AllocationSize"></param>
      void SetAllocationSize(CbFsFileInfo fileInfo, long AllocationSize);

      /// <summary>
      /// This event is fired when the OS or the application needs to change the size of the open file.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="EndOfFile"></param>
      void SetEndOfFile(CbFsFileInfo fileInfo, long EndOfFile);

      /// <summary>
      /// This event is fired when the OS needs to check if the file or directory can be deleted.
      /// Firing of this event doesn't necessarily means, that the entry will be deleted even if CanBeDeleted was set to true.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="HandleInfo"></param>
      /// <returns>CanBeDeleted</returns>
      bool CanFileBeDeleted(CbFsFileInfo fileInfo, CbFsHandleInfo HandleInfo);

      /// <summary>
      /// This event is fired when the OS or the application needs to change the times and/or the attributes of the open file.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="userContextInfo"></param>
      /// <param name="creationTime">The value can be empty (DateTime.MinValue in .NET) if the parameter is not set.</param>
      /// <param name="lastAccessTime">The value can be empty (DateTime.MinValue in .NET) if the parameter is not set.</param>
      /// <param name="lastWriteTime">The value can be empty (DateTime.MinValue in .NET) if the parameter is not set.</param>
      /// <param name="fileAttributes">
      /// Attributes can be 0 if attributes must be left intact (not changed).
      /// FILE_ATTRIBUTE_NORMAL is used to reset attributes.
      /// </param>
      void SetFileAttributes(CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo, DateTime creationTime,
         DateTime lastAccessTime, DateTime lastWriteTime, uint fileAttributes);

      /// <summary>
      /// This event is fired when the OS needs to delete the file or directory.
      /// There's no way to cancel deletion of the file or directory from this event.
      /// If your application needs to prevent deletion, you need to do this in OnCanFileBeDeleted callback/event handler.
      /// </summary>
      /// <param name="FileInfo"></param>
      void DeleteFile(CbFsFileInfo FileInfo);

      /// <summary>
      /// This event is fired when the OS needs to rename or move the file within a file system.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="NewFileName">New fully qualified name for the file. </param>
      void RenameOrMoveFile(CbFsFileInfo fileInfo, string NewFileName);

      /// <summary>
      /// This event is fired when the OS needs to read the data from the open file or volume.
      /// Write the data (no more than BytesToRead bytes) to the provided Buffer.
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="Position"></param>
      /// <param name="Buffer">In .NET you should not try to replace the Buffer. Instead you need to copy the data to the provided buffer.</param>
      /// <param name="BytesToRead"></param>
      /// <param name="bytesRead"></param>
      /// <returns>
      /// Actual number of read bytes to BytesRead.
      /// Note, that unless you create the virtual disk for some specific application, your callback handler should
      /// be able to provide exactly BytesToRead bytes of data. Reading less data than expected is an unexpected
      /// situation for many applications, and they will fail if you provide less bytes than requested.
      /// </returns>
      void ReadFile(CbFsFileInfo fileInfo, long Position, byte[] Buffer, uint BytesToRead, out uint bytesRead);

      /// <summary>
      /// This event is fired when the OS needs to write the data to the open file or volume
      /// </summary>
      /// <param name="fileInfo"></param>
      /// <param name="Position"></param>
      /// <param name="Buffer"></param>
      /// <param name="BytesToWrite">Writing less data than expected is an unexpected situation for many applications, and they will fail if you write less bytes than requested.</param>
      /// <param name="bytesWritten"></param>
      /// <returns>BytesWritten</returns>
      void WriteFile(CbFsFileInfo fileInfo, long Position, byte[] Buffer, uint BytesToWrite, out uint bytesWritten);

      /// <summary>
      /// This event is fired when the OS wants to check whether the directory is empty or contains some files.
      /// </summary>
      /// <param name="directoryInfo"></param>
      /// <param name="DirectoryName"></param>
      /// <returns></returns>
      bool IsDirectoryEmpty(CbFsFileInfo directoryInfo, string DirectoryName);

      #endregion Basic CBFS functions
   }

   // ReSharper restore UnusedMemberInSuper.Global

   // ReSharper disable RedundantAssignment
   /// <summary>
   /// Class that forces the abstraction from the CBFS, and handles the error exception conversion.
   /// </summary>
   public abstract class CBFSHandlers : ICBFSFuncs
   {
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();
      internal readonly CallbackFileSystem CbFs;

      public abstract string VolumeLabel { get; set; }

      public abstract uint VolumeId { get; set; }

      public static bool CheckStatus(string productNameCBFS)
      {
         bool isInstalled = false;

         int VersionHigh = 0, VersionLow = 0;

         SERVICE_STATUS status = new SERVICE_STATUS();

         CallbackFileSystem.GetModuleStatus(productNameCBFS, CallbackFileSystem.CBFS_MODULE_DRIVER, ref isInstalled, ref VersionHigh, ref VersionLow, ref status);

         Log.Fatal("Driver (ver {0}.{1}.{2}.{3}) installed[{4}] for [{5}]",
            VersionHigh >> 16, VersionHigh & 0xFFFF, VersionLow >> 16, VersionLow & 0xFFFF, isInstalled, productNameCBFS);
         if (isInstalled)
         {
            string state;

            switch (status.currentState)
            {
               case (int)CbFsDriverState.CBFS_SERVICE_CONTINUE_PENDING:
                  state = "continue is pending";
                  break;

               case (int)CbFsDriverState.CBFS_SERVICE_PAUSE_PENDING:
                  state = "pause is pending";
                  break;

               case (int)CbFsDriverState.CBFS_SERVICE_PAUSED:
                  state = "is paused";
                  break;

               case (int)CbFsDriverState.CBFS_SERVICE_RUNNING:
                  state = "is running";
                  break;

               case (int)CbFsDriverState.CBFS_SERVICE_START_PENDING:
                  state = "is starting";
                  break;

               case (int)CbFsDriverState.CBFS_SERVICE_STOP_PENDING:
                  state = "is stopping";
                  break;

               case (int)CbFsDriverState.CBFS_SERVICE_STOPPED:
                  state = "is stopped";
                  break;

               default:
                  state = string.Format("in undefined state [{0}]", status.currentState);
                  break;
            }
            Log.Fatal("Service state: {0}", state);
         }
         else
         {
            Log.Fatal("Driver not installed");
         }
         return isInstalled;
      }

      public void RegisterAndInit(string salt, string productNameCbfs, uint threadPoolSize, CbFsStorageType storageType)
      {
         CallbackFileSystem.SetRegistrationKey(salt.FromBuffer());
         CallbackFileSystem.Initialize(productNameCbfs);
         CreateStorage(storageType, threadPoolSize);
      }

      protected CBFSHandlers()
      {
         CbFs = new CallbackFileSystem
            {
               OnMount = Mount,
               OnUnmount = UnMount,
               OnGetVolumeSize = GetVolumeSize,
               OnGetVolumeLabel = GetVolumeLabel,
               OnSetVolumeLabel = SetVolumeLabel,
               OnGetVolumeId = GetVolumeId,
               OnCreateFile = CreateFile,
               OnCleanupFile = CleanupFile,
               OnOpenFile = OpenFile,
               OnCloseFile = CloseFile,
               OnGetFileInfo = GetFileInfo,
               OnEnumerateDirectory = EnumerateDirectory,
               OnCloseDirectoryEnumeration = CloseDirectoryEnumeration,
               OnFlushFile = FlushFile,
               OnSetAllocationSize = SetAllocationSize,
               OnSetEndOfFile = SetEndOfFile,
               OnSetFileAttributes = SetFileAttributes,
               OnCanFileBeDeleted = CanFileBeDeleted,
               OnDeleteFile = DeleteFile,
               OnRenameOrMoveFile = RenameOrMoveFile,
               OnReadFile = ReadFile,
               OnWriteFile = WriteFile,
               OnIsDirectoryEmpty = IsDirectoryEmpty,
               CallAllOpenCloseCallbacks = true,
               UseFileCreationFlags = true
            };
         /*
          * If the file is opened successfully, the opened file is used for the next file open operations,
          * which happen while the original file handle is opened. Usually this works correctly, but if
          * the file is opened with different access mode, such scheme is not acceptable. To disable the
          * described behaviour and have OnOpenFile callback be called every time,
          * set CallAllOpenCloseCallbacks property to true.
          * To make use of the security checks you must first set CallAllOpenCloseCallbacks property to true.
          * */
         // UseFileCreationFlags = true // When this property is set to true, the value of dwFlagsAndAttributes
         // parameter of CreateFile() WinAPI function is propagated to OnCreateFile/OnCloseFile callbacks.
      }

      private void Mount(CallbackFileSystem Sender)
      {
         CBFSWinUtil.Invoke("Mount", Mount);
      }

      public abstract void Mount();

      private void UnMount(CallbackFileSystem Sender)
      {
         CBFSWinUtil.Invoke("UnMount", UnMount);
      }

      public abstract void UnMount();

      private void GetVolumeSize(CallbackFileSystem Sender, ref long TotalNumberOfSectors, ref long NumberOfFreeSectors)
      {
         Log.Trace("GetVolumeSize IN");
         try
         {
            GetVolumeSize(out TotalNumberOfSectors, out NumberOfFreeSectors);
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("GetVolumeSize OUT");
         }
      }

      public abstract void GetVolumeSize(out long TotalNumberOfSectors, out long NumberOfFreeSectors);

      private void GetVolumeLabel(CallbackFileSystem Sender, ref string volumeLabel)
      {
         Log.Trace("GetVolumeLabel IN");
         try
         {
            volumeLabel = VolumeLabel;
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("GetVolumeLabel OUT");
         }
      }

      private void SetVolumeLabel(CallbackFileSystem Sender, string volumeLabel)
      {
         Log.Trace("SetVolumeLabel IN");
         try
         {
            VolumeLabel = volumeLabel;
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("SetVolumeLabel OUT");
         }
      }

      private void GetVolumeId(CallbackFileSystem Sender, ref uint volumeID)
      {
         Log.Trace("GetVolumeId IN");
         try
         {
            volumeID = VolumeId;
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("GetVolumeId OUT");
         }
      }

      private void CreateFile(CallbackFileSystem Sender, string FileName, uint DesiredAccess, uint fileAttributes, uint ShareMode,
         CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo)
      {
         CBFSWinUtil.Invoke("CreateFile", () => CreateFile(FileName, DesiredAccess, fileAttributes, ShareMode, fileInfo,
                                                 userContextInfo));
      }

      public abstract void CreateFile(string FileName, uint DesiredAccess, uint fileAttributes, uint ShareMode,
         CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      private void OpenFile(CallbackFileSystem Sender, string FileName, uint DesiredAccess, uint fileAttributes, uint ShareMode,
         CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo)
      {
         CBFSWinUtil.Invoke("OpenFile", () => OpenFile(FileName, DesiredAccess, fileAttributes, ShareMode, fileInfo, userContextInfo));
      }

      public abstract void OpenFile(string FileName, uint DesiredAccess, uint fileAttributes, uint ShareMode, CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      private void CleanupFile(CallbackFileSystem sender, CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo)
      {
         CBFSWinUtil.Invoke("CloseFile", () => CleanupFile(fileInfo, userContextInfo));
      }

      public abstract void CleanupFile(CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      private void CloseFile(CallbackFileSystem Sender, CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo)
      {
         CBFSWinUtil.Invoke("CloseFile", () => CloseFile(fileInfo, userContextInfo));
      }

      public abstract void CloseFile(CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo);

      private void GetFileInfo(CallbackFileSystem Sender, string FileName, ref bool FileExists,
         ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime,
         ref long lengthOfFile, ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId,
         ref uint FileAttributes, ref string ShortFileName, ref string RealFileName)
      {
         Log.Trace("GetFileInfo IN");
         try
         {
            GetFileInfo(FileName, ref FileExists, ref CreationTime, ref LastAccessTime, ref LastWriteTime,
               ref lengthOfFile, ref AllocationSize, ref FileId, ref FileAttributes, ref ShortFileName, ref RealFileName);
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("GetFileInfo OUT");
         }
      }

      public abstract void GetFileInfo(string FileName, ref bool FileExists,
         ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime,
         ref long lengthOfFile, ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes,
         ref string ShortFileName, ref string RealFileName);

      private void EnumerateDirectory(CallbackFileSystem Sender, CbFsFileInfo directoryInfo, CbFsHandleInfo HandleInfo,
         CbFsDirectoryEnumerationInfo DirectoryEnumerationInfo, string Mask, int Index,
         [MarshalAs(UnmanagedType.U1)] bool Restart, ref bool FileFound, ref string FileName, ref string ShortFileName,
         ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long lengthOfFile,
         ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes)
      {
         Log.Trace("EnumerateDirectory IN");
         try
         {
            EnumerateDirectory(directoryInfo, HandleInfo, DirectoryEnumerationInfo, Mask, Restart,
                  ref FileFound, ref FileName, ref ShortFileName,
                  ref CreationTime, ref LastAccessTime, ref LastWriteTime,
                  ref lengthOfFile, ref AllocationSize, ref FileId, ref FileAttributes);
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("EnumerateDirectory OUT");
         }
      }

      public abstract void EnumerateDirectory(CbFsFileInfo directoryInfo, CbFsHandleInfo HandleInfo,
   CbFsDirectoryEnumerationInfo DirectoryEnumerationInfo, string Mask, bool Restart,
   ref bool FileFound, ref string FileName, ref string ShortFileName,
   ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long lengthOfFile,
   ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes);

      /// <summary>
      ///
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="fileInfo">
      /// Contains information about the file. Can be null.
      /// If FileInfo is empty, your code should attempt to flush everything, related to the disk.
      /// </param>
      private void FlushFile(CallbackFileSystem sender, CbFsFileInfo fileInfo)
      {
         CBFSWinUtil.Invoke("FlushFile", () => FlushFile(fileInfo));
      }

      public abstract void FlushFile(CbFsFileInfo FileInfo);

      private void SetAllocationSize(CallbackFileSystem Sender, CbFsFileInfo fileInfo, long AllocationSize)
      {
         CBFSWinUtil.Invoke("SetAllocationSize", () => SetAllocationSize(fileInfo, AllocationSize));
      }

      public abstract void SetAllocationSize(CbFsFileInfo fileInfo, long AllocationSize);

      private void SetEndOfFile(CallbackFileSystem Sender, CbFsFileInfo fileInfo, long EndOfFile)
      {
         CBFSWinUtil.Invoke("SetEndOfFile", () => SetEndOfFile(fileInfo, EndOfFile));
      }

      public abstract void SetEndOfFile(CbFsFileInfo fileInfo, long EndOfFile);

      private void CanFileBeDeleted(CallbackFileSystem Sender, CbFsFileInfo fileInfo, CbFsHandleInfo HandleInfo, ref bool CanBeDeleted)
      {
         bool canBeDeleted = false;
         CBFSWinUtil.Invoke("CanFileBeDeleted", () => canBeDeleted = CanFileBeDeleted(fileInfo, HandleInfo));
         CanBeDeleted = canBeDeleted;
      }

      public abstract bool CanFileBeDeleted(CbFsFileInfo fileInfo, CbFsHandleInfo HandleInfo);

      private void CloseDirectoryEnumeration(CallbackFileSystem Sender, CbFsFileInfo directoryInfo, CbFsDirectoryEnumerationInfo directoryEnumerationInfo)
      {
         CBFSWinUtil.Invoke("CloseDirectoryEnumeration", () => CloseDirectoryEnumeration(directoryInfo, directoryEnumerationInfo));
      }

      public abstract void CloseDirectoryEnumeration(CbFsFileInfo directoryInfo, CbFsDirectoryEnumerationInfo directoryEnumerationInfo);

      private void SetFileAttributes(CallbackFileSystem Sender, CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo, DateTime CreationTime,
                                       DateTime LastAccessTime, DateTime LastWriteTime, uint FileAttributes)
      {
         CBFSWinUtil.Invoke("SetFileAttributes", () =>
            SetFileAttributes(fileInfo, userContextInfo, CreationTime, LastAccessTime, LastWriteTime, FileAttributes));
      }

      public abstract void SetFileAttributes(CbFsFileInfo fileInfo, CbFsHandleInfo userContextInfo, DateTime creationTime,
         DateTime lastAccessTime, DateTime lastWriteTime, uint fileAttributes);

      private void ReadFile(CallbackFileSystem Sender, CbFsFileInfo fileInfo, long Position, byte[] Buffer,
         int BytesToRead, ref int bytesRead)
      {
         Log.Trace("ReadFile IN");
         uint read = 0;
         try
         {
            ReadFile(fileInfo, Position, Buffer, (uint)BytesToRead, out read);
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            bytesRead = (int)read;
            Log.Trace("ReadFile OUT");
         }
      }

      public abstract void ReadFile(CbFsFileInfo fileInfo, long Position, byte[] Buffer, uint BytesToRead, out uint bytesRead);

      private void WriteFile(CallbackFileSystem Sender, CbFsFileInfo fileInfo, long Position, byte[] Buffer,
         int BytesToWrite, ref int bytesWritten)
      {
         Log.Trace("WriteFile IN");
         uint written = 0;
         try
         {
            WriteFile(fileInfo, Position, Buffer, (uint)BytesToWrite, out written);
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            bytesWritten = (int)written;
            Log.Trace("WriteFile OUT");
         }
      }

      public abstract void WriteFile(CbFsFileInfo fileInfo, long Position, byte[] Buffer, uint BytesToWrite, out uint bytesWritten);

      private void IsDirectoryEmpty(CallbackFileSystem Sender, CbFsFileInfo directoryInfo, string DirectoryName,
         ref bool IsEmpty)
      {
         bool isEmpty = false;
         CBFSWinUtil.Invoke("IsDirectoryEmpty", () => isEmpty = IsDirectoryEmpty(directoryInfo, DirectoryName));
         IsEmpty = isEmpty;
      }

      public abstract bool IsDirectoryEmpty(CbFsFileInfo directoryInfo, string DirectoryName);

      private void DeleteFile(CallbackFileSystem Sender, CbFsFileInfo fileInfo)
      {
         CBFSWinUtil.Invoke("DeleteFile", () => DeleteFile(fileInfo));
      }

      public abstract void DeleteFile(CbFsFileInfo fileInfo);

      private void RenameOrMoveFile(CallbackFileSystem Sender, CbFsFileInfo fileInfo, string NewFileName)
      {
         CBFSWinUtil.Invoke("RenameOrMoveFile", () => RenameOrMoveFile(fileInfo, NewFileName));
      }

      public abstract void RenameOrMoveFile(CbFsFileInfo fileInfo, string NewFileName);

      #region Driver redirect API's

      /// <summary>
      /// Use GetOriginatorProcessId to get the process ID (PID) of the process that originated the operation.
      /// Call this method only from the callback / event handlers.
      /// Do not call this method from handlers for OnReadFile, OnWriteFile and other callbacks that work with opened
      /// files, as that callbacks can be initiated by the system components (cache manager, memory manager etc.).
      /// Instead do the following:
      ///   Call GetOriginatorProcessId from OnCreateFile or OnOpenFile event handlers / callbacks;
      ///   Store obtained information somewhere and store the reference to this information in the UserContext;
      ///   When you need to check the originator information in some file-related callback, access the stored information via UserContext
      /// </summary>
      /// <remarks>
      /// Note that the PID is guaranteed to be unique only during lifetime of the process. When the process is
      /// terminated, it's PID can (in theory) be used by other process. In fact this never happens, but such
      /// possibility is documented by Microsoft.
      /// </remarks>
      /// <returns></returns>
      protected int GetProcessId()
      {
         uint processId = 0;
         CbFs.GetOriginatorProcessId(ref processId);
         return (int)processId;
      }

      /// <summary>
      /// Redirect accessor
      /// </summary>
      public void MountMedia(int apiTimeout)
      {
         CBFSWinUtil.Invoke("MountMedia", () => CbFs.MountMedia(apiTimeout));
      }

      /// <summary>
      /// Redirect and massaging
      /// </summary>
      /// <param name="storageType">https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_storagetype.html</param>
      /// <param name="threadPoolSize"></param>
      /// <param name="iconRef"></param>
      public void CreateStorage(CbFsStorageType storageType, uint threadPoolSize, string iconRef = null)
      {
         Log.Trace("CreateStorage IN");
         try
         {
            // Must set StorageType before mounting !!
            CbFs.StorageType = storageType;
            // Must set ThreadPoolSize before mounting !!
            CbFs.MinWorkerThreadCount = 1;
            CbFs.MaxWorkerThreadCount = threadPoolSize;
            CbFs.NonexistentFilesCacheEnabled = true; // https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_nonexistentfilescacheenabled.html
            CbFs.MetaDataCacheEnabled = true;         // https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_metadatacacheenabled.html
            CbFs.FileCacheEnabled = true;             // https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_filecacheenabled.html
            CbFs.ShortFileNameSupport = false;
            CbFs.ClusterSize = 0;// The value must be a multiple of sector size. Default value of 0 tells the driver to have cluster size equal to sector size.
            CbFs.SectorSize = 4096;
            // Make this a local style disk
            CbFs.StorageCharacteristics = 0;          // https://www.eldos.com/forum/read.php?FID=13&TID=3681
            // the CallAllOpenCloseCallbacks is going to be forced to be true in the next version, so ensure stuff work here with it.
            CbFs.CallAllOpenCloseCallbacks = true;    // https://www.eldos.com/forum/read.php?FID=13&TID=479
            // Pass the creation around, This can then be used to determine which of the already opened flags can be decremented to finally release the handle.
            CbFs.UseFileCreationFlags = true;         // https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_usefilecreationflags.html

            CbFs.SerializeCallbacks = true;          // https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_serializecallbacks.html
            CbFs.ParallelProcessingAllowed = false;    // https://www.eldos.com/documentation/cbfs/ref_cl_cbfs_prp_parallelprocessingallowed.html
            CbFs.FileSystemName = "NTFS";

            // Go create stuff
            CbFs.CreateStorage();
            if (!string.IsNullOrWhiteSpace(iconRef))
            {
               if (!CbFs.IconInstalled(iconRef))
               {
                  throw new MissingSatelliteAssemblyException("Requested Icon ref is not installed: " + iconRef);
               }
               CbFs.SetIcon(iconRef);
            }
            CbFs.NotifyDirectoryChange("\"", CbFsNotifyFileAction.fanAdded, false);
         }
         catch (Exception ex)
         {
            CBFSWinUtil.BestAttemptToECBFSError(ex);
         }
         finally
         {
            Log.Trace("CreateStorage OUT");
         }
      }

      private uint flags;
      private string mountingPoint;

      /// <summary>
      /// Redirect and massaging
      /// </summary>
      /// <param name="driveLetter">https://www.eldos.com/documentation/cbfs/ref_gen_mounting_points.html</param>
      /// <param name="mountingType">Basic mounting point creation. see cref</param>
      /// <param name="networkSymLinkFlags">Additional flags for network mounting points. see cref</param>
      /// <see cref="http://www.eldos.com/documentation/cbfs/ref_cl_cbfs_mtd_addmountingpoint.html"/>
      public void AddMountingPoint(string driveLetter, uint mountingType, CbFsNetworkSymLinkFlags networkSymLinkFlags)
      {
         flags = mountingType + (uint)networkSymLinkFlags;
         if (driveLetter.Length == 1)
         {
            driveLetter = driveLetter.ToUpper() + ":";
         }
         mountingPoint = driveLetter;
         CBFSWinUtil.Invoke("AddMountingPoint", () => CbFs.AddMountingPoint(driveLetter, flags, null));
      }

      /// <summary>
      /// Redirect accessor
      /// </summary>
      public void DeleteMountingPoint()
      {
         CBFSWinUtil.Invoke("DeleteMountingPoint", () => CbFs.DeleteMountingPoint(mountingPoint, flags, null));
      }

      /// <summary>
      /// Redirect accessor
      /// </summary>
      public void UnmountMedia()
      {
         CBFSWinUtil.Invoke("UnmountMedia", () => CbFs.UnmountMedia());
      }

      /// <summary>
      /// Redirect accessor
      /// </summary>
      public void DeleteStorage(bool forceUnmount)
      {
         CBFSWinUtil.Invoke("DeleteStorage", () => CbFs.DeleteStorage(forceUnmount));
      }

      #endregion Driver redirect API's
   }

   // ReSharper restore RedundantAssignment
}