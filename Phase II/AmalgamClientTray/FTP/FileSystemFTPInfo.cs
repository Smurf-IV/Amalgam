#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="FileSystemFTPInfo.cs" company="Smurf-IV">
// 
//  Copyright (C) 2011-2012 Simon Coghlan (aka Smurf-IV)
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
using System.IO;
using Starksoft.Net.Ftp;

namespace AmalgamClientTray.FTP
{
   public abstract class FileSystemFTPInfo //: FileSystemInfo
   {
      protected readonly string path;
      protected readonly FtpClientExt FtpCmdInstance;
      protected internal FileAttributes attributes = 0;

      protected FileSystemFTPInfo(FtpClientExt ftpCmdInstance, string path)
      {
         this.path = path;
         FtpCmdInstance = ftpCmdInstance;
      }

      #region Overrides of FileSystemInfo

      public string FullName
      {
         get { return path; }
      }

// ReSharper disable UnusedMember.Local
      // Remove unsupported features
      private DateTime CreationTime { set; get; }
      private DateTime LastWriteTime { set; get; }
      private DateTime LastAccessTimeUtc { set; get; }
// ReSharper restore UnusedMember.Local

      internal string name;
      /// <summary>
      /// For files, gets the name of the file. For directories, gets the name of the last directory in the hierarchy if a hierarchy exists. Otherwise, the Name property gets the name of the directory.
      /// </summary>
      /// <returns>
      /// A string that is the name of the parent directory, the name of the last directory in the hierarchy, or the name of a file, including the file name extension.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public string Name
      {
         get
         {
            if (string.IsNullOrEmpty(name))
               name = FtpClient.ExtractPathItemName(path);
            return name;
         }
      }


      internal DateTime lastWriteTimeUtc;
      public DateTime LastWriteTimeUtc
      {
         get
         {
               return lastWriteTimeUtc;
            // TODO: call the MFMT
            return new DateTime();
         }
         set 
         {
            if (lastWriteTimeUtc != value)
            {
               lastWriteTimeUtc = value;
               FtpCmdInstance.SetModifiedDateTime(path, lastWriteTimeUtc);
            }
         }
      }

      internal DateTime creationTimeUtc;
      public DateTime CreationTimeUtc
      {
         get 
         { 
            return creationTimeUtc;
            // TODO: call the MFCT
            return new DateTime();
         }
         set
         {
            if (creationTimeUtc != value)
            {
               creationTimeUtc = value;
               FtpCmdInstance.SetCreatedDateTime(path, creationTimeUtc);
            }
         }
      }

      /// <summary>
      /// Gets a value indicating whether the file or directory exists.
      /// </summary>
      /// <returns>
      /// true if the file or directory exists; otherwise, false.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public virtual bool Exists
      {
         get
         {
            return ((FileAttributes.Device & Attributes) != FileAttributes.Device);
         }
      }

      private long length = -1;

      public long Length
      {
         get
         {
            if (length < 0)
               length = FtpCmdInstance.GetFileSize(path);
            return length;
         }
         internal set
         {
            length = value;
         }
      }

      public FileAttributes Attributes
      {
         get
         {
            if (attributes == 0)
            {
               attributes = FileAttributes.Device;
               try
               {
                  FileSystemFTPInfo info = FtpCmdInstance.GetFileDetails((attributes != FileAttributes.Directory) ? path : string.Empty);
                  if (info != null)
                  {
                     attributes = info.attributes;
                     length = info.length;
                     creationTimeUtc = info.creationTimeUtc;
                     lastWriteTimeUtc = info.lastWriteTimeUtc;
                     name = info.name;
                  }
               }
               catch { }
            }
            return attributes;
         }
         set
         {
            FtpCmdInstance.SetAttributes(path, value);
            attributes = value;
         }
      }


            /// <summary>
      /// Deletes a file or directory.
      /// </summary>
      /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception><filterpriority>2</filterpriority>
      public abstract void Delete();

      #endregion

      public static List<FileSystemFTPInfo> ConvertFrom(FtpItemCollection results, FtpClientExt ftpCmdInstance)
      {
         List<FileSystemFTPInfo> _list = new List<FileSystemFTPInfo>(results.Count);
         foreach (FtpItem item in results)
         {
            FileSystemFTPInfo info = (item.ItemType == FtpItemType.Directory)
               ? (FileSystemFTPInfo) new DirectoryFTPInfo(ftpCmdInstance, item.FullPath)
               : new FileFTPInfo(ftpCmdInstance, item.FullPath);
            // Set it to be offline to allow explorer some extra time to find details before timg out.
            info.attributes = (item.ItemType == FtpItemType.Directory) ? FileAttributes.Directory : FileAttributes.Offline;
            if ( item.Attributes[0] == 'l' )
               info.attributes |= FileAttributes.ReparsePoint;
            // drwx-
            if ( (item.Attributes[1] == 'r' )
               && (item.Attributes[2] != 'w')
               )
               info.attributes |= FileAttributes.ReadOnly;
            info.length = item.Size;
            info.creationTimeUtc = item.Modified;
            info.lastWriteTimeUtc = item.Modified;
            _list.Add(info);
         }
         return _list;
      }
   }
}
