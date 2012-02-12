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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using NLog;
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
            CacheInfoExpireSeconds = csd.CacheInfoExpireSeconds;
            FileNamesToIgnore = csd.FileNamesToIgnore;
            DokanThreadCount = csd.DokanThreadCount;
            DokanDebugMode = csd.DokanDebugMode;
            ApplicationLogLevel = csd.ApplicationLogLevel;

         }
      }


      [DescriptionAttribute("The number of Bytes allocated to be chunked over the Wire.\rRange 4096 <-> near 1GB.\rWarning: during long transfers some FTP servers can log you out!")
      , DisplayName("Buffer Wire Transfer Size")
      , CategoryAttribute("\t\t\tLocal")
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

      [DescriptionAttribute("The number of Bytes allocated to each of the temporary cache files.\rRange 4096 <-> near 32MB.")
      , DisplayName("Cache File Max Size")
      , CategoryAttribute("\t\t\tLocal")
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

      
      [DescriptionAttribute("The number of secounds the file information will be stored before it is removed.\rRange 1 <-> near 12 hours.")
      , DisplayName("Cache Info Expire Seconds")
      , CategoryAttribute("\t\t\tLocal")
      ]
      public UInt32 CacheInfoExpireSeconds
      {
         get
         {
            return cacheInfoExpireSeconds;
         }
         set
         {
            if ( (value >= 1)
                && (value <= 12 * 60 *60)
               )
               cacheInfoExpireSeconds = value;
         }
      }

      [DescriptionAttribute("The Name to be used in explorer.")
      , DisplayName("Drive Label")
      , CategoryAttribute("\t\t\tLocal")
      ]
      public string VolumeLabel { get; set; }

      [DescriptionAttribute("The drive letter used in explorer")
      , DisplayName("Drive letter")
      , CategoryAttribute("\t\t\tLocal")
      ]
      public string DriveLetter { get; set; }

      [DescriptionAttribute("The name allocated to the share on the target machine.")
      , DisplayName("Target directory to be used as root")
      , CategoryAttribute("\t\tUser Details")
      , ReadOnly(true)
      ]
      public string TargetShareName { get; set; }

      [DescriptionAttribute("The User to be used as the ACL Check up.")
      , DisplayName("FTP User for the connection")
      , CategoryAttribute("\t\tUser Details")
      ]
      public string UserName { get; set; }


      [DescriptionAttribute("The Password to be used as the ACL Check up.")
      , DisplayName("Password associated with the above user.")
      , CategoryAttribute("\t\tUser Details")
      ]
      public string Password { get; set; }

      [DescriptionAttribute("The machine name or IP address of the Target share.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allow communications.")
      , DisplayName("Target FTP Machine")
      , CategoryAttribute("\tRemote Machine")
      ]
      public string TargetMachineName { get; set; }

      [DescriptionAttribute("The Target port.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allowing communications.")
      , DisplayName("Target FTP Port")
      , CategoryAttribute("\tRemote Machine")
      ]
      public ushort Port { get; set; }


      [DescriptionAttribute("The Target SecurityProtocol.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allow communications.")
      , DisplayName("Target FTP SecurityProtocol")
      , CategoryAttribute("\tRemote Machine")
      , ReadOnly(true)
      ]
      public FtpSecurityProtocol SecurityProtocol { get; set; }

      [DescriptionAttribute( "Do not attempt to retrieve details about the following filenames.\rThis will speed up Explorers discovery.\rNote: These are case sensitive, and will be applied to directories as well")
      , DisplayName("FileNames to ignore")
      , CategoryAttribute("\tRemote Machine")
      , Editor( @"System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
      , typeof (System.Drawing.Design.UITypeEditor))
      , TypeConverter(typeof (FileNamesToIgnoreConverter))
      ]
      public List<String> FileNamesToIgnore { get; set; }



      [DescriptionAttribute("Turn Dokan Debug information on, to all it to be captured in an appropriate app.")
      , DisplayName("Dokan Debug Mode")
      , CategoryAttribute("Advanced Settings")
      ]
      public bool DokanDebugMode { get; set; }


      [DescriptionAttribute("0 is automatic, use 1 for problem finding scenario's.\rRange 0 <-> 32")
      , DisplayName("Dokan Thread Count")
      , CategoryAttribute("Advanced Settings")
      ]
      public ushort DokanThreadCount
      {
         get { return dokanThreadCount; }
         set
         {
            if (value >= 0
                && value <= 32)
               dokanThreadCount = value;
         }
      }

      [DescriptionAttribute("The amount of information that will be placed into the Log files (Trace means slower performance!).")
      , DisplayName("Application Logging Level")
      , TypeConverter(typeof(ApplicationLogLevelValues))
      , CategoryAttribute("Advanced Settings")
      ]
      public string ApplicationLogLevel { get; set; }

      private UInt32 bufferWireTransferSize;
      private UInt32 cacheFileMaxSize;
      private UInt32 cacheInfoExpireSeconds;
      private ushort dokanThreadCount;

      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedAutoPropertyAccessor.Global

   }

   public class FileNamesToIgnoreConverter : TypeConverter
   {
      // Overrides the ConvertTo method of TypeConverter.
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
         List<String> v = value as List<String>;
         return (destinationType == typeof(string)) ? String.Join(": ", v.ToArray()) : base.ConvertTo(context, culture, value, destinationType);
      }
   }

   public class ApplicationLogLevelValues : StringConverter
   {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
         //true means show a combobox
         return true;
      }
      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
         //true will limit to list. false will show the list, but allow free-form entry
         return true;
      }

      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
         return new StandardValuesCollection(new[] { LogLevel.Warn.ToString(), LogLevel.Debug.ToString(), LogLevel.Trace.ToString() });
      }
   }

}