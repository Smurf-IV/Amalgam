#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ClientShareDetail.cs" company="Smurf-IV">
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
using System.Collections.Generic;
using Starksoft.Net.Ftp;

namespace AmalgamClientTray.ClientForms
{
   [Serializable]
   public class ClientShareDetail
   {
      // ReSharper disable UnusedAutoPropertyAccessor.Global
      // ReSharper disable MemberCanBePrivate.Global

      // Make this is a string so that the XML looks better (Rather than exporting 72 for 'N')
      // Also the V 0.6 of Dokan is supposed to be able to use Mount points so this can then be reused for that..
      public string DriveLetter = @"S";

      public string TargetMachineName = @"ftp.kernel.org";
      public ushort Port = 21;
      public FtpSecurityProtocol SecurityProtocol = FtpSecurityProtocol.None;

      public string UserName = @"anonymous";

      public string Password = @"me@me.com";

      public string TargetShareName = @"";

      public string VolumeLabel = @"AmalgamClientTray";

      // Used to send data over the wire, this is not recommended to be over int.maxvalue / 2
      // In here as different targets may have different capabilities
      // Set the minimum to be 4096 bytes
      public UInt32 BufferWireTransferSize = 8192;

      public UInt32 CacheFileMaxSize = 8192;

      public UInt32 CacheInfoExpireSeconds = 300;

      public List<string> FileNamesToIgnore = new List<string> { "AutoRun.inf", "Desktop.ini", "desktop.ini", "MAPI32.DLL" };

      public bool DokanDebugMode = false;
      public ushort DokanThreadCount = 5;
      public string ApplicationLogLevel = "Warn"; // NLog's LogLevel.Debug.ToString()

      public bool TargetIsReadonly = false;

      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedAutoPropertyAccessor.Global
   }
}