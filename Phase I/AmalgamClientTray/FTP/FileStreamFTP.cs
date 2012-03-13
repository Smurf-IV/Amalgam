#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="FileStreamFTP.cs" company="Smurf-IV">
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

using System;
using System.Globalization;
using System.IO;
using AmalgamClientTray.ClientForms;
using Starksoft.Net.Ftp;

namespace AmalgamClientTray.FTP
{
   public class FileStreamFTP
   {
      protected readonly ClientShareDetail csd;
      private readonly uint rawCreationDisposition; // http://msdn.microsoft.com/en-us/library/aa363858%28v=vs.85%29.aspx
      private FileFTPInfo fsi { get; set; }
      private bool completedOpen;
      protected FtpClientExt ftpFileClient;

      protected FileStreamFTP(ClientShareDetail csd, uint rawCreationDisposition, FileFTPInfo foundFileInfo)
      {
         this.csd = csd;
         this.rawCreationDisposition = rawCreationDisposition;
         fsi = foundFileInfo;
      }


      private void ConnectFTP()
      {
         try
         {
            ftpFileClient = new FtpClientExt(new FtpClient(csd.TargetMachineName, csd.Port, csd.SecurityProtocol), csd.BufferWireTransferSize);
            ftpFileClient.Open(csd.UserName, csd.Password);
            fsi.Open((FileMode)rawCreationDisposition);
            completedOpen = true;
         }
         catch (Exception ex)
         {
            throw;
         }
      }

      public long Length
      {
         get
         {
            return fsi.Length;
         }
      }

      public string FullName
      {
         get { return fsi.FullName; }
      }

      private void CheckOpened()
      {
         if (!completedOpen)
         {
            ConnectFTP();
         }
      }

      public void Close()
      {
         if (ftpFileClient != null)
         {
            completedOpen = false;
            ftpFileClient.Close();
         }
      }

      virtual public void Seek(long rawOffset, SeekOrigin begin)
      {
         if (begin != SeekOrigin.Begin)
            throw new ArgumentOutOfRangeException("begin", "This function only supports offsets from SeekOrigin.Begin");
         CheckOpened();
         ftpFileClient.Feature(false, Features.REST, false, rawOffset.ToString(CultureInfo.InvariantCulture));
      }

      virtual public uint Read(byte[] buf, uint rawBufferLength)
      {
         CheckOpened();
         using (MemoryStream memStream = new MemoryStream(buf, true))
         {
            ftpFileClient.GetFile(fsi.FullName, memStream);
            return (uint) memStream.Position;
         }
      }

      /// <summary>
      /// Uses the current offset to write this buffer
      /// </summary>
      /// <param name="buf"></param>
      /// <param name="rawNumberOfBytesToWrite"></param>
      /// <returns></returns>
      virtual public bool Write(byte[] buf, uint rawNumberOfBytesToWrite)
      {
         CheckOpened();
         using (MemoryStream memStream = new MemoryStream(buf, 0, (int)rawNumberOfBytesToWrite, false))
         {
            ftpFileClient.PutFile(memStream, fsi.FullName, (rawCreationDisposition == (uint) FileMode.Append));
         }
         return true;
      }

      public void SetFileTime(DateTime ctime, DateTime atime, DateTime mtime)
      {
         if (ctime != DateTime.MinValue)
            fsi.CreationTimeUtc = ctime;

         if (mtime != DateTime.MinValue)
            fsi.LastWriteTimeUtc = mtime;
      }

      public void SetFileAttributes(FileAttributes attr)
      {
         CheckOpened();
         ftpFileClient.SetFileAttributes(fsi.FullName, attr);
      }
   }
}
