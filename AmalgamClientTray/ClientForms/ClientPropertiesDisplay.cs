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
         }
      }


      [DescriptionAttribute("The number of Bytes allocated to be chunked over the Wire.\rRange 4096 <-> near 1GB"),
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
      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedAutoPropertyAccessor.Global

   }
}