using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CallbackFS;

namespace CBFS
{
   internal static class CBFSWinError
   {
      private static int HiWord(int number)
      {
         return ((number & 0x80000000) == 0x80000000) ? number >> 16 : (number >> 16) & 0xffff;
      }

      private static int LoWord(int number)
      {
         return number & 0xffff;
      }

      public static uint BestAttemptToWin32(Exception ex)
      {
         Win32Exception win32Exception = ex as Win32Exception;
         if (win32Exception != null)
         {
            return (uint) win32Exception.NativeErrorCode;
         }
         SocketException socketException = ex.InnerException as SocketException;
         if (socketException != null)
         {
            return (uint) socketException.ErrorCode;
         }
         int HrForException = Marshal.GetHRForException(ex);
         // Check http://msdn.microsoft.com/en-us/library/ms819772.aspx (WinError.h) for error codes
         return (uint) ((HiWord(HrForException) == -32761/*0x8007*/) ? -LoWord(HrForException) : ERROR_EXCEPTION_IN_SERVICE);
         // #define ERROR_DISK_OPERATION_FAILED 1127L //  While accessing the hard disk, a disk operation failed even after retries.
         // The above might be a better error code ??
      }

      // ReSharper disable InconsistentNaming
      // ReSharper disable MemberCanBePrivate.Global
      // ReSharper disable UnusedMember.Global
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
      // ReSharper restore UnusedMember.Global
      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore InconsistentNaming
   }

   public interface ICBFSFuncs
   {
      void Mounted();
      void Unmounted();
      void GetVolumeSize(ref long TotalNumberOfSectors, ref long NumberOfFreeSectors);
      void OpenVolume();
      void CloseVolume();
      // return the fileHandleContext
      UInt64 CreateFile(string fileName, FileMode desiredAccess, FileAttributes fileAttributes, uint extFlagsAndAttributes, FileShare ShareMode);
      // return the fileHandleContext
      UInt64 OpenFile(string fileName, FileMode desiredAccess, FileShare ShareMode);
      void CloseFile(string fileName, UInt64 fileHandle);
      void GetFileInfo(string fileName, ref bool FileExists, ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize, ref long FileId, ref uint FileAttributes, ref string LongFileName);
      void EnumerateDirectory(string DirectoryName, ref IntPtr EnumerationContext, string Mask, int Index, bool Restart, ref bool FileFound, ref string fileName, ref string ShortFileName, ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize, ref long FileId, ref uint FileAttributes);
      void SetAllocationSize(string fileName, UInt64 fileHandle, long AllocationSize);
      void SetEndOfFile(string fileName, UInt64 fileHandle, long EndOfFile);
      void SetFileAttributes(string fileName, UInt64 fileHandle, DateTime CreationTime, DateTime LastAccessTime, DateTime LastWriteTime, FileAttributes fileAttributes, uint extFlagsAndAttributes);
      bool CanFileBeDeleted(string fileName, UInt64 fileHandle);
      void DeleteFile(string fileName, UInt64 fileHandle);
      void RenameOrMoveFile(string fileName, string NewFileName);
      // return BytesRead
      int ReadFile(string fileName, UInt64 fileHandle, long Position, byte[] Buffer, int BytesToRead);
      // return BytesWritten
      int WriteFile(string fileName, UInt64 fileHandle, long Position, byte[] Buffer, int BytesToWrite);
      void CloseEnumeration(string DirectoryName, IntPtr EnumerationContext);
      bool IsDirectoryEmpty(string FileInfo);
      void EnumerateNamedStreams(string fileName, UInt64 fileHandle, ref IntPtr NamedStreamContext, ref string StreamName, ref long StreamSize, ref long StreamAllocationSize, ref bool NamedStreamFound);
      void SetFileSecurity(string fileName, UInt64 fileHandle, uint SecurityInformation, IntPtr SecurityDescriptor, uint Length);
      void GetFileSecurity(string fileName, UInt64 fileHandle, uint SecurityInformation, IntPtr SecurityDescriptor, uint Length, ref uint LengthNeeded);
      // return FilePath, assuming FileID is the QuadPart
      string GetFileNameByFileId(long FileId);
      void FlushFile(string fileName, UInt64 fileHandle);
      void StorageEjected();
      string VolumeLabel { get; set; }
      uint VolumeID { get; }

   }

   // ReSharper disable RedundantAssignment
   /// <summary>
   /// If some exception occurs when the callback is executed, the application must catch this exception 
   /// and throw an instance of ECBFSError instead. If you don't catch some exception in your code, this 
   /// class will catch it for you, and attempt to convert it into a Win32 Error code; If it cannot, CBFS
   /// will report "Internal error" error to the OS, and this doesn't look good for the OS and for the user. 
   /// </summary>
   public class CBFSHandlers
   {
      private readonly CallbackFileSystem CbFs;
      private readonly ICBFSFuncs Handlers;

      // ReSharper disable RedundantThisQualifier
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="handlers">The event redirects for your FS</param>
      /// <param name="handlesStreams">Does this FS handle FileStreams?</param>
      public CBFSHandlers(ICBFSFuncs handlers, bool handlesStreams)
      {
         if (handlers == null) 
            throw new ArgumentNullException("handlers");

         Handlers = handlers;
         CbFs = new CallbackFileSystem
            {
               OnMount = this.OnMount,
               OnUnmount = this.OnUnmount,
               OnGetVolumeSize = this.OnGetVolumeSize,
               OnGetVolumeLabel = this.OnGetVolumeLabel,
               OnSetVolumeLabel = this.OnSetVolumeLabel,
               OnGetVolumeId = this.OnGetVolumeId,
               OnOpenVolume = this.OnOpenVolume,
               OnCloseVolume = this.OnCloseVolume,
               OnCreateFile = this.OnCreateFile,
               OnOpenFile = this.OnOpenFile,
               OnCloseFile = this.OnCloseFile,
               OnGetFileInfo = this.OnGetFileInfo,
               OnEnumerateDirectory = this.OnEnumerateDirectory,
               OnCloseEnumeration = this.OnCloseEnumeration,
               OnSetAllocationSize = this.OnSetAllocationSize,
               OnSetEndOfFile = this.OnSetEndOfFile,
               OnSetFileAttributes = this.OnSetFileAttributes,
               OnCanFileBeDeleted = this.OnCanFileBeDeleted,
               OnDeleteFile = this.OnDeleteFile,
               OnRenameOrMoveFile = this.OnRenameOrMoveFile,
               OnReadFile = this.OnReadFile,
               OnWriteFile = this.OnWriteFile,
               OnIsDirectoryEmpty = this.OnIsDirectoryEmpty,
               OnSetFileSecurity = this.OnSetFileSecurity,
               OnGetFileSecurity = this.OnGetFileSecurity,
               OnGetFileNameByFileId = this.OnGetFileNameByFileId,
               OnFlushFile = this.OnFlushFile,
               OnStorageEjected = this.OnStorageEjected,
               CallAllOpenCloseCallbacks = true
            };
         /*
          * If the file is opened successfully, the opened file is used for the next file open operations, 
          * which happen while the original file handle is opened. Usually this works correctly, but if 
          * the file is opened with different access mode, such scheme is not acceptable. To disable the 
          * described behaviour and have OnOpenFile callback be called every time, 
          * set CallAllOpenCloseCallbacks property to true. 
          * To make use of the security checks you must first set CallAllOpenCloseCallbacks property to true. 
          * 
          * If the file name contains semicolon (":"), this means that the request is made to create a named 
          * stream in a file. The part before the semicolon is the name of the file itself and the name after
          * the semicolon is the name of the named stream. If you don't want to deal with named streams, 
          * don't implement the handler for OnEnumerateNamedStreams event. In this case CBFS API will tell 
          * the OS that the named streams are not supported by the file system. 
          * */
         if (handlesStreams)
            CbFs.OnEnumerateNamedStreams = this.OnEnumerateNamedStreams;

         CbFs.CreateStorage();

      }
      // ReSharper restore RedundantThisQualifier

      private void CheckValidReference(CallbackFileSystem sender)
      {
         // TODO: Check VCB or Tag ??
         if (sender != CbFs)
            throw new ECBFSError("Incorrect driver reference", CBFSWinError.ERROR_IO_DEVICE);
      }

      private void OnMount(CallbackFileSystem sender)
      {
         CheckValidReference(sender);
         try
         {
            Handlers.Mounted();
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnUnmount(CallbackFileSystem sender)
      {
         CheckValidReference(sender);
         try
         {
            Handlers.Unmounted();
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnGetVolumeSize(CallbackFileSystem sender, ref long TotalNumberOfSectors, ref long NumberOfFreeSectors)
      {
         CheckValidReference(sender);
         try
         {
            Handlers.GetVolumeSize(ref TotalNumberOfSectors, ref NumberOfFreeSectors);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      private void OnGetVolumeLabel(CallbackFileSystem sender, ref string volumeLabel)
      {
         CheckValidReference(sender);
         volumeLabel = Handlers.VolumeLabel;
      }

      private void OnSetVolumeLabel(CallbackFileSystem sender, string volumeLabel)
      {
         CheckValidReference(sender);
         Handlers.VolumeLabel = volumeLabel;
      }

      private void OnGetVolumeId(CallbackFileSystem sender, ref uint volumeID)
      {
         CheckValidReference(sender);
         volumeID = Handlers.VolumeID;
      }

      private void OnOpenVolume(CallbackFileSystem sender)
      {
         CheckValidReference(sender);
         try
         {
            Handlers.OpenVolume();
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnCloseVolume(CallbackFileSystem sender)
      {
         CheckValidReference(sender);
         try
         {
            Handlers.CloseVolume();
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      private void OnCreateFile(CallbackFileSystem sender, string fileName, uint DesiredAccess,
                                      uint fileAttributes, uint ShareMode,
                                      ref IntPtr fileHandleContext)
      {
         CheckValidReference(sender);
         try
         {
            UInt64 fileHandle = Handlers.CreateFile(fileName, (FileMode)DesiredAccess, (FileAttributes)(fileAttributes & 0x0001FFFF), (fileAttributes & 0xFFFE0000), (FileShare)ShareMode);
            Marshal.WriteInt64(fileHandleContext, (long) fileHandle);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnOpenFile(CallbackFileSystem sender, string fileName, uint DesiredAccess, uint ShareMode,
                                    ref IntPtr fileHandleContext)
      {
         CheckValidReference(sender);
         try
         {
            UInt64 fileHandle = Handlers.OpenFile(fileName, (FileMode)DesiredAccess, (FileShare) ShareMode);
            Marshal.WriteInt64(fileHandleContext, (long) fileHandle);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      private void OnCloseFile(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.CloseFile(fileInfo.FileName, fileHandle);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      private void OnGetFileInfo(CallbackFileSystem sender, string fileName, ref bool FileExists, ref DateTime CreationTime,
                                       ref DateTime LastAccessTime, ref DateTime LastWriteTime, ref long EndOfFile, ref long AllocationSize,
                                       ref CBFS_LARGE_INTEGER FileId, ref uint FileAttributes, ref string LongFileName,
                                       ref ushort LongFileNameLength)
      {
         CheckValidReference(sender);
         try
         {
            long quadPart = 0;
            Handlers.GetFileInfo(fileName, ref FileExists, ref CreationTime,
                                       ref LastAccessTime, ref LastWriteTime, ref EndOfFile, ref AllocationSize,
                                       ref quadPart, ref FileAttributes, ref LongFileName);
            FileId.QuadPart = quadPart;
            FileId.LowPart = (uint) (quadPart & 0xffffffff);
            FileId.HighPart = (int)(quadPart >> 32);
            LongFileNameLength = (ushort) LongFileName.Length;
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      private void OnEnumerateDirectory(CallbackFileSystem sender, CbFsFileInfo directoryInfo, ref IntPtr EnumerationContext,
                                              string Mask, int Index, bool Restart, ref bool FileFound, ref string fileName,
                                              ref ushort FileNameLength, ref string ShortFileName, ref byte ShortFileNameLength,
                                              ref DateTime CreationTime, ref DateTime LastAccessTime, ref DateTime LastWriteTime,
                                              ref long EndOfFile, ref long AllocationSize, ref CBFS_LARGE_INTEGER FileId,
                                              ref uint FileAttributes)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(directoryInfo.Volume);
            long quadPart = 0;
            Handlers.EnumerateDirectory(directoryInfo.FileName, ref EnumerationContext, Mask, Index, Restart,
                                        ref FileFound, ref fileName, ref ShortFileName,
                                        ref CreationTime, ref LastAccessTime, ref LastWriteTime,
                                        ref EndOfFile, ref AllocationSize, ref quadPart, ref FileAttributes);
            FileNameLength = (ushort)fileName.Length;
            ShortFileNameLength = (byte) ShortFileName.Length;
            FileId.QuadPart = quadPart;
            FileId.LowPart = (uint) (quadPart & 0xffffffff);
            FileId.HighPart = (int)(quadPart >> 32);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnSetAllocationSize(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext, long AllocationSize)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.SetAllocationSize(fileInfo.FileName, fileHandle, AllocationSize);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnSetEndOfFile(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext, long EndOfFile)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.SetEndOfFile(fileInfo.FileName, fileHandle, EndOfFile);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnSetFileAttributes(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext, DateTime CreationTime,
                                             DateTime LastAccessTime, DateTime LastWriteTime, uint fileAttributes)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.SetFileAttributes(fileInfo.FileName, fileHandle, CreationTime, LastAccessTime, LastWriteTime, (FileAttributes)(fileAttributes & 0x0001FFFF), (fileAttributes & 0xFFFE0000));
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      private void OnCanFileBeDeleted(CallbackFileSystem sender, CbFsFileInfo fileInfo, ref bool canBeDeleted)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            canBeDeleted = Handlers.CanFileBeDeleted(fileInfo.FileName, fileHandle);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnDeleteFile(CallbackFileSystem sender, CbFsFileInfo fileInfo)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.DeleteFile(fileInfo.FileName, fileHandle);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnRenameOrMoveFile(CallbackFileSystem sender, CbFsFileInfo fileInfo, string NewFileName)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.RenameOrMoveFile(fileInfo.FileName, NewFileName);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnReadFile(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext, long Position, byte[] Buffer,
                                    int BytesToRead, ref int BytesRead)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            BytesRead = Handlers.ReadFile(fileInfo.FileName, fileHandle, Position, Buffer, BytesToRead);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnWriteFile(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext, long Position, byte[] Buffer,
                                     int BytesToWrite, ref int BytesWritten)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            BytesWritten = Handlers.WriteFile(fileInfo.FileName, fileHandle, Position, Buffer, BytesToWrite);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnCloseEnumeration(CallbackFileSystem sender, CbFsFileInfo directoryInfo, IntPtr EnumerationContext)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(directoryInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(directoryInfo.HandleContext);
            Handlers.CloseEnumeration(directoryInfo.FileName, EnumerationContext);
            throw new ECBFSError("Not implemented", CBFSWinError.ERROR_CALL_NOT_IMPLEMENTED);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnIsDirectoryEmpty(CallbackFileSystem sender, string FileInfo, ref bool IsEmpty)
      {
         CheckValidReference(sender);
         try
         {
            IsEmpty = Handlers.IsDirectoryEmpty(FileInfo);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnEnumerateNamedStreams(CallbackFileSystem sender, CbFsFileInfo fileInfo, ref IntPtr NamedStreamContext,
                                                 ref string StreamName, ref int StreamNameLength, ref long StreamSize,
                                                 ref long StreamAllocationSize, ref bool NamedStreamFound)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.EnumerateNamedStreams(fileInfo.FileName, fileHandle, ref NamedStreamContext,
                                           ref StreamName, ref StreamSize, ref StreamAllocationSize, ref NamedStreamFound);
            StreamNameLength = StreamName.Length;
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnSetFileSecurity(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext,
                                           uint SecurityInformation, IntPtr SecurityDescriptor, uint Length)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.SetFileSecurity(fileInfo.FileName, fileHandle, SecurityInformation, SecurityDescriptor, Length);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnGetFileSecurity(CallbackFileSystem sender, CbFsFileInfo fileInfo, IntPtr fileHandleContext,
                                           uint SecurityInformation, IntPtr SecurityDescriptor, uint Length, ref uint LengthNeeded)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.GetFileSecurity(fileInfo.FileName, fileHandle, SecurityInformation, SecurityDescriptor,
                                     Length, ref LengthNeeded);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      // Assuming FileID is the QuadPart
      private void OnGetFileNameByFileId(CallbackFileSystem sender, long FileId, ref string FilePath, ref ushort FilePathLength)
      {
         CheckValidReference(sender);
         try
         {
            FilePath = Handlers.GetFileNameByFileId(FileId);
            FilePathLength = (ushort) FilePath.Length;
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnFlushFile(CallbackFileSystem sender, CbFsFileInfo fileInfo)
      {
         CheckValidReference(sender);
         try
         {
            CheckValidReference(fileInfo.Volume);
            UInt64 fileHandle = (ulong) Marshal.ReadInt64(fileInfo.HandleContext);
            Handlers.FlushFile(fileInfo.FileName, fileHandle);
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }

      private void OnStorageEjected(CallbackFileSystem sender)
      {
         CheckValidReference(sender);
         try
         {
            Handlers.StorageEjected();
         }
         catch (Exception ex)
         {
            throw new ECBFSError(CBFSWinError.BestAttemptToWin32(ex));
         }
      }


      public void MountMedia(ushort apiTimeoutms)
      {
         CbFs.MountMedia(apiTimeoutms);
      }

      public void AddMountingPoint(string driveLetter)
      {
         CbFs.AddMountingPoint(driveLetter);
      }

      public void UnmountMedia()
      {
         CbFs.UnmountMedia();
      }

      public void DeleteStorage(bool ForceUnmount)
      {
         CbFs.DeleteStorage(ForceUnmount);
      }
   }
   // ReSharper restore RedundantAssignment
}
