#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="DirectoryFTPInfo.cs" company="Smurf-IV">
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
using System.IO;

namespace AmalgamClientTray.FTP
{
   class DirectoryFTPInfo : FileSystemFTPInfo
   {
      public DirectoryFTPInfo(FtpClientExt ftpCmdInstance, string path)
         :base( ftpCmdInstance, path )
      {
      }

      #region Overrides of FileSystemFTPInfo

      /// <summary>
      /// Deletes a file or directory.
      /// </summary>
      /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception><filterpriority>2</filterpriority>
      public override void Delete()
      {
         FtpCmdInstance.DeleteDirectory(path);
      }

      /// <summary>
      /// Gets a value indicating whether the file or directory exists.
      /// </summary>
      /// <returns>
      /// true if the file or directory exists; otherwise, false.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public override bool Exists
      {
         get 
         {
            return (base.Exists
               && ((FileAttributes.Directory & Attributes ) == FileAttributes.Directory)
               );
         }
      }

      #endregion

      public void Create()
      {
         FtpCmdInstance.MakeDirectory(path);
         attributes = 0;
      }

      public FileSystemFTPInfo[] GetFileSystemInfos(string pattern, SearchOption topDirectoryOnly)
      {
         if (pattern != "*")
            throw new ArgumentOutOfRangeException("pattern", "Cannot be anything but *");
         if (topDirectoryOnly != SearchOption.TopDirectoryOnly)
            throw new ArgumentOutOfRangeException("topDirectoryOnly", "Cannot be anything but topDirectoryOnly");

         return FtpCmdInstance.GetDirList(path).ToArray();
      }
   }
}
