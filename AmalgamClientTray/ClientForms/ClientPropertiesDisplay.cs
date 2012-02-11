#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ClientPropertiesDisplay.cs" company="Smurf-IV">
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
using System.ComponentModel;
using Starksoft.Net.Ftp;

namespace AmalgamClientTray.ClientForms
{
   // ReSharper disable UnusedAutoPropertyAccessor.Global
   // ReSharper disable MemberCanBePrivate.Global
    // Needs to be global to allow the propertgrid reflector to the accessors
   public class ClientPropertiesDisplay
   {
      public ClientPropertiesDisplay(ClientShareDetail csd)
      {
         if (csd != null)
         {
            TargetMachineName = csd.TargetMachineName;
            Port = csd.Port;
            SecurityProtocol = csd.SecurityProtocol;
            UserName = csd.UserName;
            Password = csd.Password;
            TargetShareName = csd.TargetShareName;
            DriveLetter = csd.DriveLetter;
            VolumeLabel = csd.VolumeLabel;
            BufferWireTransferSize = csd.BufferWireTransferSize;
            CacheFileMaxSize = csd.CacheFileMaxSize;
         }
      }


      [DescriptionAttribute("The number of Bytes allocated to be chunked over the Wire.\rRange 4096 <-> near 1GB."),
      DisplayName("Buffer Wire Transfer Size")
      , CategoryAttribute("Local")
      ]
      public UInt32 BufferWireTransferSize
      {
         get {
            return bufferWireTransferSize;
         }
         set
         {
            if (value >= 1 << 12
                && value <= 1 << 30)
               bufferWireTransferSize = value;
         }
      }

      [DescriptionAttribute("The number of Bytes allocated to each of the temporary cache files.\rRange 4096 <-> near 32MB."),
      DisplayName("Cache File Max Size")
      , CategoryAttribute("Local")
      ]
      public UInt32 CacheFileMaxSize
      {
         get
         {
            return cacheFileMaxSize;
         }
         set
         {
            if (value >= 1 << 12
                && value <= 1 << 25)
               cacheFileMaxSize = value;
         }
      }

      [DescriptionAttribute("The Name to be used in explorer."),
      DisplayName("Drive Label")
      , CategoryAttribute("Local")
      ]
      public string VolumeLabel { get; set; }

      [DescriptionAttribute("The drive letter used in explorer"),
      DisplayName("Drive letter")
      , CategoryAttribute("Local")
      ]
      public string DriveLetter { get; set; }

      [DescriptionAttribute("The name allocated to the share on the target machine."),
      DisplayName("Target directory to be used as root")
      , CategoryAttribute("User Details")
      ]
      public string TargetShareName { get; set; }

      [DescriptionAttribute("The User to be used as the ACL Check up."),
      DisplayName("FTP User for the connection")
      , CategoryAttribute("User Details")
      ]
      public string UserName { get; set; }


      [DescriptionAttribute("The Password to be used as the ACL Check up."),
      DisplayName("Password associated with the above user.")
      , CategoryAttribute("User Details")
      ]
      public string Password { get; set; }

      [DescriptionAttribute("The machine name or IP address of the Target share.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allow communications."),
      DisplayName("Target FTP Machine")
      , CategoryAttribute("Remote Machine")
      ]
      public string TargetMachineName { get; set; }

      [DescriptionAttribute("The Target port.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allowing communications."),
      DisplayName("Target FTP Port")
      , CategoryAttribute("Remote Machine")
      ]
      public ushort Port { get; set; }


      [DescriptionAttribute("The Target SecurityProtocol.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allow communications."),
      DisplayName("Target FTP SecurityProtocol")
      , CategoryAttribute("Remote Machine")
      , ReadOnly(true)
      ]
      public FtpSecurityProtocol SecurityProtocol { get; set; }

      
      private UInt32 bufferWireTransferSize;
      private UInt32 cacheFileMaxSize;
      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedAutoPropertyAccessor.Global

   }
}