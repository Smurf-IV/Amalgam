#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ClientPropertiesDisplay.cs" company="Smurf-IV">
// 
//  Copyright (C) 2012 Simon Coghlan (aka Smurf-IV)
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
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Windows.Controls.PropertyGrid.Attributes;
using NLog;
using Starksoft.Net.Ftp;
using ItemCollection = Microsoft.Windows.Controls.PropertyGrid.Attributes.ItemCollection;

namespace AmalgamClientTray.ClientForms
{
   // ReSharper disable UnusedAutoPropertyAccessor.Global
   // ReSharper disable MemberCanBePrivate.Global
    // Needs to be global to allow the propertgrid reflector to the accessors
   public class ClientPropertiesDisplay
   {
      public ClientPropertiesDisplay()
         : this(new ClientShareDetail())
      {
      }

      public ClientPropertiesDisplay(ClientShareDetail csd)
      {
         if (csd != null)
         {
            TargetMachineName = csd.TargetMachineName;
            port = csd.Port;
            SecurityProtocol = csd.SecurityProtocol;
            UserName = csd.UserName;
            Password = csd.Password;
            TargetShareName = csd.TargetShareName;
            DriveLetter = csd.DriveLetter;
            VolumeLabel = csd.VolumeLabel;
            bufferWireTransferSize = csd.BufferWireTransferSize;
            cacheFileMaxSize = csd.CacheFileMaxSize;
            cacheInfoExpireSeconds = csd.CacheInfoExpireSeconds;
            FileNamesToIgnore = csd.FileNamesToIgnore;
            dokanThreadCount = csd.DokanThreadCount;
            DokanDebugMode = csd.DokanDebugMode;
            ApplicationLogLevel = csd.ApplicationLogLevel;
            TargetIsReadonly = csd.TargetIsReadonly;
            IgnoreSetTimeStampFailure = csd.IgnoreSetTimeStampFailure;
            TargetRequiresSplitDirs = csd.TargetRequiresSplitDirs;
         }
      }


      [DescriptionAttribute("The number of Bytes allocated to be chunked over the Wire.\rRange 4096 <-> near 1GB.\rWarning: during long transfers some FTP servers can log you out!")
      , DisplayName("Buffer Wire Transfer Size")
      , CategoryAttribute("​\u200b​\u200b​\u200bLocal")
      ]
      public int BufferWireTransferSize
      {
         get { return (int) bufferWireTransferSize; }
         set
         {
            if (value >= 1 << 12
                && value <= 1 << 30)
               bufferWireTransferSize = (uint) value;
         }
      }

      [DescriptionAttribute("The number of Bytes allocated to each of the temporary cache files.\rRange 4096 <-> near 32MB.")
      , DisplayName("Cache File Max Size")
      , CategoryAttribute("​\u200b​\u200b​\u200bLocal")
      ]
      public int CacheFileMaxSize
      {
         get { return (int)cacheFileMaxSize; }
         set
         {
            if (value >= 1 << 12
                && value <= 1 << 25)
               cacheFileMaxSize = (uint) value;
         }
      }

      
      [DescriptionAttribute("The number of secounds the file information will be stored before it is removed.\rRange 1 <-> near 12 hours.")
      , DisplayName("Cache Info Expire Seconds")
      , CategoryAttribute("​\u200b​\u200b​\u200bLocal")
      ]
      public int CacheInfoExpireSeconds
      {
         get { return (int)cacheInfoExpireSeconds; }
         set
         {
            if ( (value >= 1)
                && (value <= 12 * 60 *60)
               )
               cacheInfoExpireSeconds = (uint) value;
         }
      }

      class CustomInlineEditor : PropertyValueEditor
      {
         public CustomInlineEditor( double start)
         {
            this.InlineEditorTemplate = new DataTemplate();

            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory slider = new FrameworkElementFactory(typeof(Slider));
            Binding sliderBinding = new Binding("Value");
            sliderBinding.Mode = BindingMode.TwoWay;
            slider.SetValue(Slider.MinimumProperty, start);
            slider.SetValue(Slider.MaximumProperty, 100.0);
            slider.SetValue(Slider.ValueProperty, sliderBinding);
            stack.AppendChild(slider);

            FrameworkElementFactory textb = new FrameworkElementFactory(typeof(TextBox));
            Binding textBinding = new Binding("Value");
            textb.SetValue(TextBox.TextProperty, textBinding);
            textb.SetValue(TextBox.IsEnabledProperty, false);

            stack.AppendChild(textb);

            this.InlineEditorTemplate.VisualTree = stack;

         }
      }
      [DescriptionAttribute("The Name to be used in explorer.")
      , DisplayName("Drive Label")
      , CategoryAttribute("​\u200b​\u200b​\u200bLocal")
      ]
      public string VolumeLabel { get; set; }

      [DescriptionAttribute("The drive letter used in explorer")
      , DisplayName("Drive letter")
      , CategoryAttribute("​\u200b​\u200b​\u200bLocal")
      ]
      public string DriveLetter { get; set; }

      [DescriptionAttribute("The name allocated to the share on the target machine.")
      , DisplayName("Target directory to be used as root")
      , CategoryAttribute("​\u200b​\u200bUser Details")
      , ReadOnly(true)
      ]
      public string TargetShareName { get; set; }

      [DescriptionAttribute("The User to be used as the ACL Check up.")
      , DisplayName("FTP User for the connection")
      , CategoryAttribute("​\u200b​\u200bUser Details")
      ]
      public string UserName { get; set; }


      [DescriptionAttribute("The Password to be used as the ACL Check up.")
      , DisplayName("Password associated with the above user.")
      , CategoryAttribute("​\u200b​\u200bUser Details")
      ]
      public string Password { get; set; }

      [DescriptionAttribute("The machine name or IP address of the Target share.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allow communications.")
      , DisplayName("Target FTP Machine")
      , CategoryAttribute("​\u200bRemote Machine")
      ]
      public string TargetMachineName { get; set; }

      [DescriptionAttribute("The Target port.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allowing communications.")
      , DisplayName("Target FTP Port")
      , CategoryAttribute("​\u200bRemote Machine")
      ]
      public int Port 
      {
         get { return port; }
         set
         {
            if (value >= 0
                && value <= 32)
               port = (ushort)value;
         }
      }
      
      [DescriptionAttribute("The Target SecurityProtocol.\rPress test to ensure that it is connectable.\rNOTE: If the share is not visible then try disabling (temporarily) the firewalls between the machines to see if the applications are not allow communications.")
      , DisplayName("Target FTP SecurityProtocol")
      , CategoryAttribute("​\u200bRemote Machine")
      , ReadOnly(true)
      , Browsable(true)
      ]
      public FtpSecurityProtocol SecurityProtocol { get; set; }

      [DescriptionAttribute( "Do not attempt to retrieve details about the following filenames.\rThis will speed up Explorers discovery.\rNote: These are case sensitive, and will be applied to directories as well")
      , DisplayName("FileNames to ignore")
      , CategoryAttribute("​\u200bRemote Machine")
      ]
      public List<string> FileNamesToIgnore { get; set; }

      [DescriptionAttribute("Enforces readonly attributes to all files and directories returned from the target.\rWill prevent writing to the target as well.")
      , DisplayName("Target Is Readonly")
      , CategoryAttribute("​\u200bRemote Machine")
      ]
      public bool TargetIsReadonly { get; set; }

      [DescriptionAttribute("Some FTP Servers do not implement the API's necessary to set time stamps.\rThis prevents these failures being reported back to the calling application.")
      , DisplayName("Ignore SetTimeStamp Failure")
      , CategoryAttribute("​\u200bRemote Machine")
      ]
      public bool IgnoreSetTimeStampFailure { get; set; }

      [DescriptionAttribute("Some FTP Servers do not accept full paths when traveresing through their filesystems.\rThis can only be found via trail and error.\rIf you cannot get file details by going stright to an \"N-depth\" child, then set this.\rNote: There may be a performnce penalty.")
      , DisplayName("Target Requires Split Dirs")
      , CategoryAttribute("​\u200bRemote Machine")
      ]
      public bool TargetRequiresSplitDirs { get; set; }


      [DescriptionAttribute("Turn Dokan Debug information on, to all it to be captured in an appropriate app.")
      , DisplayName("Dokan Debug Mode")
      , CategoryAttribute("Advanced Settings")
      ]
      public bool DokanDebugMode { get; set; }


      [DescriptionAttribute("0 is automatic, use 1 for problem finding scenario's.\rRange 0 <-> 32")
      , DisplayName("Dokan Thread Count")
      , CategoryAttribute("Advanced Settings")
      ]
      public int DokanThreadCount
      {
         get { return dokanThreadCount; }
         set
         {
            if (value >= 0
                && value <= 32)
               dokanThreadCount = (ushort) value;
         }
      }

      [DescriptionAttribute("The amount of information that will be placed into the Log files (Trace means slower performance!).")
      , DisplayName("Application Logging Level")
      //, TypeConverter(typeof(ApplicationLogLevelValues))
      , CategoryAttribute("Advanced Settings")
      ]
      [ItemsSource(typeof(ApplicationLogLevelValues))]
      public string ApplicationLogLevel { get; set; }

      public class ApplicationLogLevelValues : IItemsSource
      {
         public ItemCollection GetValues()
         {
            ItemCollection sizes = new ItemCollection();
            sizes.Add(LogLevel.Warn.ToString());
            sizes.Add(LogLevel.Debug.ToString());
            sizes.Add(LogLevel.Trace.ToString() );
            return sizes;
         }
      }
      private UInt32 bufferWireTransferSize;
      private UInt32 cacheFileMaxSize;
      private UInt32 cacheInfoExpireSeconds;
      private ushort dokanThreadCount;
      private ushort port;

      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedAutoPropertyAccessor.Global

   }

   //public class FileNamesToIgnoreConverter : StringConverter
   //{
   //   public override bool GetStandardValuesSupported(
   //                              ITypeDescriptorContext context)
   //   {
   //      return false;
   //   }

   //   //public override StandardValuesCollection
   //   //               GetStandardValues(ITypeDescriptorContext context)
   //   //{
   //   //   return new StandardValuesCollection(new string[]{"New File", 
   //   //                                               "File1", 
   //   //                                               "Document1"});
   //   //}
   //   public override bool GetStandardValuesExclusive(
   //                        ITypeDescriptorContext context)
   //   {
   //      return false;
   //   }

   //   public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
   //   {
   //      return new StandardValuesCollection((context.Instance as List<string>));
   //   }

   //   // Overrides the ConvertTo method of TypeConverter.
   //   public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
   //   {
   //      List<String> v = value as List<String>;
   //      return (destinationType == typeof(string)) ? String.Join(": ", v.ToArray()) : base.ConvertTo(context, culture, value, destinationType);
   //   }
   //}

   //public class ApplicationLogLevelValues : StringConverter
   //{
   //   public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
   //   {
   //      //true means show a combobox
   //      return true;
   //   }
   //   public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
   //   {
   //      //true will limit to list. false will show the list, but allow free-form entry
   //      return true;
   //   }

   //   public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
   //   {
   //      return new StandardValuesCollection(new[] { LogLevel.Warn.ToString(), LogLevel.Debug.ToString(), LogLevel.Trace.ToString() });
   //   }
   //}

}