﻿using System;
using System.IO;
using AmalgamClientTray.ClientForms;

namespace AmalgamClientTray.FTP
{
   public class OptimizedFTPFileReadHandler : FileStreamFTP
   {
      private FTPTempFile ftpReadBuffer;
      private long lastRawOffset { get; set; }

      public OptimizedFTPFileReadHandler(ClientShareDetail csd, uint rawCreationDisposition, FileFTPInfo foundFileInfo)
         : base(csd, rawCreationDisposition, foundFileInfo)
      {
      }

      void CheckAndReadIntoTemp()
      {
         if (ftpReadBuffer == null)
         {
            // Set to true to allow windows caching to take place
            ftpReadBuffer = new FTPTempFile(true);
         }
         long readLength = Length;
         if (Length > csd.CacheFileMaxSize)
            readLength = csd.CacheFileMaxSize;
         if (readLength > ftpReadBuffer.IO.Length)
            FillInternalTempFile(readLength);
      }

      private void FillInternalTempFile(long readLength)
      {
         using (MemoryStream memStream = new MemoryStream(new byte[readLength]))
         {
            base.Seek(0, SeekOrigin.Begin);
            ftpFileClient.GetFile(FullName, memStream);
            ftpReadBuffer.Position = 0;
            memStream.WriteTo(ftpReadBuffer.IO);
         }
      }

      override public void Seek(long rawOffset, SeekOrigin begin)
      {
         CheckAndReadIntoTemp();
         if (begin != SeekOrigin.Begin)
            throw new ArgumentOutOfRangeException("begin", "This function only supports offsets from SeekOrigin.Begin");
         if (rawOffset > ftpReadBuffer.IO.Length)
            base.Seek(rawOffset, begin);
         else
            ftpReadBuffer.Position = (int) rawOffset;
         lastRawOffset = rawOffset;
      }

      override public uint Read(byte[] buf, uint rawBufferLength)
      {
         // TODO: Eventually use a temp file and use a continuous Asynch Filestream into it via
         // internal void TransferData(TransferDirection direction, FtpRequest request, Stream data, long restartPosition)
         if ((rawBufferLength > Length)
            && (rawBufferLength > ftpReadBuffer.IO.Length)
            )
         {
            return (uint)ftpReadBuffer.IO.Read(buf, 0, (int)rawBufferLength);
         }
         if ( (rawBufferLength > (ftpReadBuffer.IO.Length - ftpReadBuffer.Position))
            )
         {
            base.Seek(lastRawOffset, SeekOrigin.Begin);
            return base.Read(buf, rawBufferLength);
         }
         return (uint) ftpReadBuffer.IO.Read(buf, 0, (int) rawBufferLength);
      }

      override public bool Write(byte[] buf, uint rawNumberOfBytesToWrite)
      {
         // TODO: Eventually use a temp file and use a continuous Asynch Filestream into it via
         // internal void TransferData(TransferDirection direction, FtpRequest request, Stream data, long restartPosition)
         if (lastRawOffset < ftpReadBuffer.IO.Length)
            ftpReadBuffer.IO.Write(buf, 0, (int)Math.Min(rawNumberOfBytesToWrite, ftpReadBuffer.IO.Length - lastRawOffset));
         return base.Write(buf, rawNumberOfBytesToWrite);
      }
   }
}