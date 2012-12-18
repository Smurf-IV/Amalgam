using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using CallbackFS;

namespace CBFS
{
   internal static class CBFSWinError
   {
      public static int HiWord(int number)
      {
         return ((number & 0x80000000) == 0x80000000) ? number >> 16 : (number >> 16) & 0xffff;
      }

      public static int LoWord(int number)
      {
         return number & 0xffff;
      }

      public static int BestAttemptToWin32(Exception ex)
      {
         Win32Exception win32Exception = ex as Win32Exception;
         if (win32Exception != null)
         {
            return win32Exception.NativeErrorCode;
         }
         SocketException socketException = ex.InnerException as SocketException;
         if (socketException != null)
         {
            return socketException.ErrorCode;
         }
         int HrForException = Marshal.GetHRForException(ex);
         // Check http://msdn.microsoft.com/en-us/library/ms819772.aspx (WinError.h) for error codes
         return (HiWord(HrForException) == -32761/*0x8007*/) ? -LoWord(HrForException) : ERROR_EXCEPTION_IN_SERVICE;
         // #define ERROR_DISK_OPERATION_FAILED 1127L //  While accessing the hard disk, a disk operation failed even after retries.
         // The above might be a better error code ??
      }

      // ReSharper disable InconsistentNaming
      // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 169

      #region File Operation Errors

      // From WinError.h -> http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382%28v=vs.85%29.aspx
      public const int ERROR_FILE_NOT_FOUND = 2; // MessageText: The system cannot find the file specified.
      public const int ERROR_PATH_NOT_FOUND = 3; // MessageText: The system cannot find the path specified.
      public const int ERROR_ACCESS_DENIED = 5; // MessageText: Access is denied.
      public const int ERROR_SHARING_VIOLATION = 32;
      public const int ERROR_FILE_EXISTS = 80;
      public const int ERROR_CALL_NOT_IMPLEMENTED = 120;
      public const int ERROR_DISK_FULL = 112; // There is not enough space on the disk.
      public const int ERROR_INVALID_NAME = 123;
      public const int ERROR_DIR_NOT_EMPTY = 145; // MessageText: The directory is not empty.
      public const int ERROR_ALREADY_EXISTS = 183; // MessageText: Cannot create a file when that file already exists.

      public const int ERROR_EXCEPTION_IN_SERVICE = 1064;
      //  An exception occurred in the service when handling thecontrol request.

      public const int ERROR_FILE_READ_ONLY = 6009; // The specified file is read only.

      public const int ERROR_SUCCESS = 0;
      public const int ERROR_NOACCESS = 998; // Invalid access to memory location.
      public const int ERROR_NOT_SUPPORTED = 50; // The request is not supported.


      public const int ERROR_INVALID_PARAMETER = 87;  // The parameter is incorrect.
      public const int ERROR_INVALID_HANDLE = 1609;   // Handle is in an invalid state.
      public const int ERROR_NOT_LOCKED = 158;        // The segment is already unlocked.
      public const int ERROR_NO_SYSTEM_RESOURCES = 1450;// Insufficient system resources exist to complete the requested service.
      public const int ERROR_NOT_ENOUGH_MEMORY = 8;   // Not enough storage is available to process this command.
      public const int ERROR_MORE_DATA = 234;         // More data is available.
      public const int ERROR_INSUFFICIENT_BUFFER = 122;// The data area passed to a system call is too small.
      public const int ERROR_NO_MORE_FILES = 18;      // There are no more files.
      public const int ERROR_INVALID_FUNCTION = 1;    // Incorrect function.
      public const int ERROR_HANDLE_EOF = 38;         // Reached the end of the file.
      public const int ERROR_DISK_CORRUPT = 1393;     // The disk structure is corrupted and unreadable.
      public const int ERROR_BAD_COMMAND = 22;        // The device does not recognize the command.
      public const int ERROR_CANNOT_MAKE = 82;        // The directory or file cannot be created.
      public const int ERROR_PROC_NOT_FOUND = 127;    // The specified procedure could not be found.
      public const int ERROR_OPERATION_ABORTED = 995; // The I/O operation has been aborted because of either a thread exit or an application request.
      public const int ERROR_IO_DEVICE = 1117;        // The request could not be performed because of an I/O device error.
      // public const int TYPE_E_IOERROR = 0;
      public const int ERROR_BAD_UNIT = 20;           // The system cannot find the device specified.
      public const int ERROR_BAD_ARGUMENTS = 160;     // One or more arguments are not correct.
      public const int ERROR_BAD_EXE_FORMAT = 193;    // %1 is not a valid Win32 application.
      public const int ERROR_WAIT_NO_CHILDREN = 128;  // There are no child processes to wait for.
      public const int ERROR_RETRY = 1237;            // The operation could not be completed. A retry should be performed.
      public const int ERROR_INVALID_ADDRESS = 487;   // Attempt to access invalid address.
      public const int ERROR_BUSY = 170;              // The requested resource is in use.
      public const int ERROR_DIRECTORY = 267;         // The directory name is invalid.
      public const int ERROR_TOO_MANY_OPEN_FILES = 4; // The system cannot open the file.
      public const int ERROR_EA_TABLE_FULL = 277;     // The extended attribute table file is full.
      public const int ERROR_FILE_INVALID = 1006;     // The volume for a file has been externally altered so that the opened file is no longer valid.
      public const int ERROR_CONNECTION_UNAVAIL = 1201;// The device is not currently connected but it is a remembered connection.
      public const int ERROR_TOO_MANY_LINKS = 1142;   // An attempt was made to create more links on a file than the file system supports.
      public const int ERROR_BROKEN_PIPE = 109;       // The pipe has been ended.
      public const int ERROR_ARITHMETIC_OVERFLOW = 534;// Arithmetic result exceeded 32 bits.
      public const int ERROR_POSSIBLE_DEADLOCK = 1131;// A potential deadlock condition has been detected.
      public const int ERROR_BUFFER_OVERFLOW = 111;   // The file name is too long.
      public const int ERROR_TOO_MANY_SEMAPHORES = 100;// Cannot create another system semaphore.
      public const int ERROR_ARENA_TRASHED = 7;       // The storage control blocks were destroyed.
      public const int ERROR_INVALID_BLOCK = 9;       // The storage control block address is invalid.
      public const int ERROR_BAD_ENVIRONMENT = 10;    // The environment is incorrect.
      public const int ERROR_FILENAME_EXCED_RANGE = 206;// The filename or extension is too long.
      public const int ERROR_NOT_READY = 21;          // The device is not ready.
      public const int ERROR_FILE_OFFLINE = 4350;     // This file is currently not available for use on this computer.
      public const int ERROR_REMOTE_STORAGE_NOT_ACTIVE = 4351;// The remote storage service is not operational at this time.
      public const int ERROR_NO_SUCH_PRIVILEGE = 1313;// A specified privilege does not exist.
      public const int ERROR_PRIVILEGE_NOT_HELD = 1314;// A required privilege is not held by the client.
      public const int ERROR_CANNOT_IMPERSONATE = 1368;// Unable to impersonate using a named pipe until data has been read from that pipe.
      public const int ERROR_WRITE_PROTECT = 19;      // The media is write protected.
      public const int ERROR_LOGON_FAILURE = 1326;    // Logon failure: unknown user name or bad password.

      #endregion

#pragma warning restore 169
      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore InconsistentNaming
   }

   interface ICBFSFuncs
   {
      void Mount();
      void Unmount();
      void GetVolumeSize(ref long TotalNumberOfSectors, ref long NumberOfFreeSectors);
      void GetVolumeLabel(ref string VolumeLabel);
      void SetVolumeLabel(string VolumeLabel);
      void GetVolumeId(ref uint VolumeID);
      void OpenVolume();
      void CloseVolume();
      void CreateFile(string FileName, uint DesiredAccess, uint FileAttributes, uint ShareMode, ref IntPtr FileHandleContext);
      void OpenFile(string FileName, uint DesiredAccess, uint ShareMode, ref IntPtr FileHandleContext);
      void CloseFile(CbFsFileInfo FileInfo, IntPtr FileHandleContext);
      void GetFileInfo(string FileName, ref bool FileExists, ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes, ref string LongFileName, ref ushort LongFileNameLength);
      void EnumerateDirectory(CbFsFileInfo DirectoryInfo, ref IntPtr EnumerationContext, string Mask, int Index, [MarshalAs(UnmanagedType.U1)] bool Restart, ref bool FileFound, ref string FileName, ref ushort FileNameLength, ref string ShortFileName, ref byte ShortFileNameLength, ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes);
      void SetAllocationSize(CbFsFileInfo FileInfo, IntPtr FileHandleContext, long AllocationSize);
      void SetEndOfFile(CbFsFileInfo FileInfo, IntPtr FileHandleContext, long EndOfFile);
      void SetFileAttributes(CbFsFileInfo FileInfo, IntPtr FileHandleContext, DateTime CreationTime, DateTime LastAccessTime, DateTime LastWriteTime, uint FileAttributes);
      void CanFileBeDeleted(CbFsFileInfo FileInfo, ref bool CanBeDeleted);
      void DeleteFile(CbFsFileInfo FileInfo);
      void RenameOrMoveFile(CbFsFileInfo FileInfo, string NewFileName);
      void ReadFile(CbFsFileInfo FileInfo, IntPtr FileHandleContext, long Position, byte[] Buffer, int BytesToRead, ref int BytesRead);
      void WriteFile(CbFsFileInfo FileInfo, IntPtr FileHandleContext, long Position, byte[] Buffer, int BytesToWrite, ref int BytesWritten);
      void CloseEnumeration(CbFsFileInfo DirectoryInfo, IntPtr EnumerationContext);
      void IsDirectoryEmpty(string FileInfo, ref bool IsEmpty);
      void EnumerateNamedStreams(CbFsFileInfo FileInfo, ref IntPtr NamedStreamContext, ref string StreamName, ref int StreamNameLength, ref long StreamSize, ref long StreamAllocationSize, ref bool NamedStreamFound);
      void SetFileSecurity(CbFsFileInfo FileInfo, IntPtr FileHandleContext, uint SecurityInformation, IntPtr SecurityDescriptor, uint Length);
      void GetFileSecurity(CbFsFileInfo FileInfo, IntPtr FileHandleContext, uint SecurityInformation, IntPtr SecurityDescriptor, uint Length, ref uint LengthNeeded);
      void GetFileNameByFileId(long FileId, ref string FilePath, ref ushort FilePathLength);
      void FlushFile(CbFsFileInfo FileInfo);
      void StorageEjected();
   }

   // ReSharper disable RedundantAssignment
   public abstract class CBFSHandlers : ICBFSFuncs
   {
      private CallbackFileSystem CbFs;
      protected abstract string VolumeLabel { get; set; }

      protected abstract uint VolumeID { get; }

      public void CBFSHandlers()
      {
         CbFs = new CallbackFileSystem
            {
               OnMount = Mount,
               OnUnmount = Unmount,
               OnGetVolumeSize = GetVolumeSize,
               OnGetVolumeLabel = GetVolumeLabel,
               OnSetVolumeLabel = SetVolumeLabel,
               OnGetVolumeId = GetVolumeId,
               OnOpenVolume = OpenVolume,
               OnCloseVolume = CloseVolume,
               OnCreateFile = CreateFile,
               OnOpenFile = OpenFile,
               OnCloseFile = CloseFile,
               OnGetFileInfo = GetFileInfo,
               OnEnumerateDirectory = EnumerateDirectory,
               OnCloseEnumeration = CloseEnumeration,
               OnSetAllocationSize = SetAllocationSize,
               OnSetEndOfFile = SetEndOfFile,
               OnSetFileAttributes = SetFileAttributes,
               OnCanFileBeDeleted = CanFileBeDeleted,
               OnDeleteFile = DeleteFile,
               OnRenameOrMoveFile = RenameOrMoveFile,
               OnReadFile = ReadFile,
               OnWriteFile = WriteFile,
               OnIsDirectoryEmpty = IsDirectoryEmpty,
               CallAllOpenCloseCallbacks = true
            };
         /*
          * If the file is opened successfully, the opened file is used for the next file open operations, 
          * which happen while the original file handle is opened. Usually this works correctly, but if 
          * the file is opened with different access mode, such scheme is not acceptable. To disable the 
          * described behaviour and have OnOpenFile callback be called every time, 
          * set CallAllOpenCloseCallbacks property to true. 
          * To make use of the security checks you must first set CallAllOpenCloseCallbacks property to true. 
          * */
      }

      public abstract void Mount(CallbackFileSystem Sender);
      public abstract void Unmount(CallbackFileSystem Sender);
      public abstract void GetVolumeSize(CallbackFileSystem Sender, ref long TotalNumberOfSectors, ref long NumberOfFreeSectors);

      public void GetVolumeLabel(CallbackFileSystem Sender, ref string volumeLabel)
      {
         volumeLabel = VolumeLabel;
         throw new ECBFSError(CallbackFileSystem.); 
      }

      public void SetVolumeLabel(CallbackFileSystem Sender, string volumeLabel)
      {
         VolumeLabel = volumeLabel;
      }

      public void GetVolumeId(CallbackFileSystem Sender, ref uint volumeID)
      {
         volumeID = VolumeID;
      }

      public abstract void OpenVolume(CallbackFileSystem Sender);
      public abstract void CloseVolume(CallbackFileSystem Sender);

      public abstract void CreateFile(CallbackFileSystem Sender, string FileName, uint DesiredAccess,
                                      uint FileAttributes, uint ShareMode,
                                      ref IntPtr FileHandleContext);
      //{
      //   long l = Marshal.ReadInt64(FileHandleContext);
      //}

      public abstract void OpenFile(CallbackFileSystem Sender, string FileName, uint DesiredAccess, uint ShareMode,
                                    ref IntPtr FileHandleContext);

      public abstract void CloseFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext);

      public abstract void GetFileInfo(CallbackFileSystem Sender, string FileName, ref bool FileExists, ref DateTime CreationTime,
                                       ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize,
                                       ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes, ref string LongFileName,
                                       ref ushort LongFileNameLength);

      public abstract void EnumerateDirectory(CallbackFileSystem Sender, CbFsFileInfo DirectoryInfo, ref IntPtr EnumerationContext,
                                              string Mask, int Index, bool Restart, ref bool FileFound, ref string FileName,
                                              ref ushort FileNameLength, ref string ShortFileName, ref byte ShortFileNameLength,
                                              ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime,
                                              ref long EndOfFile, ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId,
                                              ref uint FileAttributes);

      public abstract void SetAllocationSize(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext, long AllocationSize);
      public abstract void SetEndOfFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext, long EndOfFile);

      public abstract void SetFileAttributes(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext, DateTime CreationTime,
                                             DateTime LastAccessTime, DateTime LastWriteTime, uint FileAttributes);

      public abstract void CanFileBeDeleted(CallbackFileSystem Sender, CbFsFileInfo FileInfo, ref bool CanBeDeleted);
      public abstract void DeleteFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo);
      public abstract void RenameOrMoveFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo, string NewFileName);

      public abstract void ReadFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext, long Position, byte[] Buffer,
                                    int BytesToRead, ref int BytesRead);

      public abstract void WriteFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext, long Position, byte[] Buffer,
                                     int BytesToWrite, ref int BytesWritten);

      public abstract void CloseEnumeration(CallbackFileSystem Sender, CbFsFileInfo DirectoryInfo, IntPtr EnumerationContext);
      public abstract void IsDirectoryEmpty(CallbackFileSystem Sender, string FileInfo, ref bool IsEmpty);

      public abstract void EnumerateNamedStreams(CallbackFileSystem Sender, CbFsFileInfo FileInfo, ref IntPtr NamedStreamContext,
                                                 ref string StreamName, ref int StreamNameLength, ref long StreamSize,
                                                 ref long StreamAllocationSize, ref bool NamedStreamFound);

      public abstract void SetFileSecurity(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext,
                                           uint SecurityInformation, IntPtr SecurityDescriptor, uint Length);

      public abstract void GetFileSecurity(CallbackFileSystem Sender, CbFsFileInfo FileInfo, IntPtr FileHandleContext,
                                           uint SecurityInformation, IntPtr SecurityDescriptor, uint Length, ref uint LengthNeeded);

      public abstract void GetFileNameByFileId(CallbackFileSystem Sender, long FileId, ref string FilePath, ref ushort FilePathLength);
      public abstract void FlushFile(CallbackFileSystem Sender, CbFsFileInfo FileInfo);
      public abstract void StorageEjected(CallbackFileSystem Sender);
   }
   // ReSharper restore RedundantAssignment
}
