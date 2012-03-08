#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ClientPropertiesDisplay.cs" company="Smurf-IV">
// 
//  Copyright (C) 2012 Smurf-IV
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
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using AmalgamClientTray.ClientForms;
using AmalgamClientTray.Dokan;
using NLog;
using Starksoft.Net.Ftp;
using Application = System.Windows.Forms.Application;
using Cursors = System.Windows.Input.Cursors;

namespace AmalgamClientTray
{
   /// <summary>
   /// Interaction logic for Management.xaml
   /// </summary>
   public partial class Management : Window
   {
      WindowState lastWindowState;
      bool shouldClose;

      public ClientPropertiesDisplay csdDisplay = new ClientPropertiesDisplay(new ClientShareDetail());

      static private readonly Logger Log = LogManager.GetCurrentClassLogger();
      private ClientPropertiesDisplay cpd;
      internal static readonly string userAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AmalgamClientTray");
      internal static readonly string configFile = Path.Combine(userAppData, @"Client.Properties.config.xml");

      private ClientConfigDetails csd;

      private ClientConfigDetails ClientConfigDetails
      {
         get { return csd; }
         set
         {
            csd = value;
            if (csd.SharesToRestore.Count == 0)
               csd.SharesToRestore.Add(new ClientShareDetail());

            cpd = new ClientPropertiesDisplay(csd.SharesToRestore[0]);
            PropertyGrid1.SelectedObject = cpd;
         }
      }

      public Management()
      {
         InitializeComponent();
         FileInfo fi = new FileInfo(configFile);
         if (!fi.Exists)
         {
            DirectoryInfo di = fi.Directory;
            if (!di.Exists)
               di.Create();
            // The file will now be created when the ReadConfig is called
         }
         ReadConfigDetails(out csd);
         ClientConfigDetails = csd;
      }

      internal static void ReadConfigDetails(out ClientConfigDetails csd)
      {
         csd = null;
         try
         {
            // Initialise a default to allow type get !
            csd = new ClientConfigDetails();
            XmlSerializer x = new XmlSerializer(csd.GetType());
            Log.Info("Attempting to read ClientConfigDetails from: [{0}]", configFile);
            using (TextReader textReader = new StreamReader(configFile))
            {
               csd = x.Deserialize(textReader) as ClientConfigDetails;
            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("Cannot read the configDetails: ", ex);
            csd = null;
         }
         finally
         {
            if (csd == null)
            {
               Log.Info("Creating new ClientConfigDetails");
               csd = new ClientConfigDetails();
               try
               {
                  if (File.Exists(configFile))
                     File.Move(configFile, configFile + Guid.NewGuid());
               }
               catch (Exception ex)
               {
                  Log.WarnException("ReadConfigDetails", ex);
               }
               WriteOutConfigDetails(csd);
            }
         }
      }

      private static void WriteOutConfigDetails(ClientConfigDetails csd)
      {
         if (csd != null)
            try
            {
               XmlSerializer x = new XmlSerializer(csd.GetType());
               using (TextWriter textWriter = new StreamWriter(configFile))
               {
                  x.Serialize(textWriter, csd);
               }
            }
            catch (Exception ex)
            {
               Log.ErrorException("Cannot save configDetails: ", ex);
            }
      }

      protected override void OnInitialized(EventArgs e)
      {
         base.OnInitialized(e);
         lastWindowState = WindowState;
         Hide();
      }

      protected override void OnStateChanged(EventArgs e)
      {
         if (this.WindowState == WindowState.Minimized)
         {
            this.Hide();
         }
         else
         {
            this.lastWindowState = this.WindowState;
         }
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         if (!shouldClose)
         {
            e.Cancel = true;
            Hide();
         }
      }

      #region Menu clickers
      private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (e.ChangedButton == MouseButton.Left)
         {
            Open();
         }
      }

      private void OnMenuItemOpenClick(object sender, EventArgs e)
      {
         Open();
      }

      private void Open()
      {
         Show();
         WindowState = lastWindowState;
      }

      private void OnMenuItemExitClick(object sender, EventArgs e)
      {
         shouldClose = true;
         notificationIcon.Visibility = Visibility.Hidden;
         Close();
         Application.Exit();
      }

      private void OnMenuItemAboutClick(object sender, EventArgs e)
      {
         new WPFAboutBox1(null).ShowDialog();
      }
      #endregion


      #region Button Actions
      private void btnConnect_Click(object sender, RoutedEventArgs e)
      {
         string help = null, system = null, status = null;
         try
         {
            Mouse.OverrideCursor = Cursors.Wait;
            //UseWaitCursor = true;
            IsEnabled = false;
            // create a new ftpclient object with the host and port number to use
            using (FtpClient ftp = new FtpClient(cpd.TargetMachineName, (uint)cpd.Port, cpd.SecurityProtocol))
            {
               ftp.Open(cpd.UserName, cpd.Password);
               // ftp.IsConnected;
               help = ftp.GetHelp();
               system = ftp.GetSystemType();
               status = ftp.GetStatus();
               ftp.Close();
            }
            btnSave.IsEnabled = true;
         }
         catch (Exception ex)
         {
            Log.ErrorException("btnConnect_Click", ex);
            Microsoft.Windows.Controls.MessageBox.Show(this, ex.Message, "Failed to contact Target", MessageBoxButton.OK, MessageBoxImage.Error);
         }
         finally
         {
            Mouse.OverrideCursor = null;
            IsEnabled = true;
         }
         if (!string.IsNullOrWhiteSpace(help))
         {
            Microsoft.Windows.Controls.MessageBox.Show(this, string.Format("Help: {0}\nSystem: {1}\nStatus: {2}", help, system, status), @"Target server information", MessageBoxButton.OK, MessageBoxImage.Information);
         }
      }

      private void btnSave_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            Mouse.OverrideCursor = Cursors.Wait;
            IsEnabled = false;

            Log.Info("Get the details from the page into the share object");
            csd.SharesToRestore[0].TargetMachineName = cpd.TargetMachineName;
            csd.SharesToRestore[0].Port = (ushort)cpd.Port;
            csd.SharesToRestore[0].SecurityProtocol = cpd.SecurityProtocol;
            csd.SharesToRestore[0].UserName = cpd.UserName;
            csd.SharesToRestore[0].Password = cpd.Password;
            csd.SharesToRestore[0].TargetShareName = cpd.TargetShareName;
            string oldDriveLetter = csd.SharesToRestore[0].DriveLetter;
            csd.SharesToRestore[0].DriveLetter = cpd.DriveLetter;
            csd.SharesToRestore[0].VolumeLabel = cpd.VolumeLabel;
            csd.SharesToRestore[0].BufferWireTransferSize = (uint)cpd.BufferWireTransferSize;
            csd.SharesToRestore[0].CacheFileMaxSize = (uint)cpd.CacheFileMaxSize;
            csd.SharesToRestore[0].CacheInfoExpireSeconds = (uint)cpd.CacheInfoExpireSeconds;
            csd.SharesToRestore[0].FileNamesToIgnore = cpd.FileNamesToIgnore;
            csd.SharesToRestore[0].DokanThreadCount = (ushort)cpd.DokanThreadCount;
            csd.SharesToRestore[0].DokanDebugMode = cpd.DokanDebugMode;
            csd.SharesToRestore[0].ApplicationLogLevel = cpd.ApplicationLogLevel;
            csd.SharesToRestore[0].TargetIsReadonly = cpd.TargetIsReadonly;

            Log.Info("Write the values to the Service config file");
            WriteOutConfigDetails(csd);
            if (MessageBoxResult.Yes == Microsoft.Windows.Controls.MessageBox.Show(this, "This is about to stop then start the \"Share Enabler Service\".\nDo you want to this to happen now ?",
               "Stop then Start the Service Now..", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
               try
               {
                  Log.Info("Now toggle the service");
                  if (Handlers.ClientMappings.ContainsKey(oldDriveLetter))
                  {
                     if (Handlers.ClientMappings[oldDriveLetter].Stop())
                        Handlers.ClientMappings.Remove(oldDriveLetter);
                  }
                  HandleMappingThread newMapping = new HandleMappingThread();
                  Handlers.ClientMappings[csd.SharesToRestore[0].DriveLetter] = newMapping;
                  newMapping.Start(csd.SharesToRestore[0]);
               }
               catch (Exception ex)
               {
                  Log.ErrorException("btnSend_Click", ex);
                  Microsoft.Windows.Controls.MessageBox.Show(this, ex.Message, "Failed, Check the logs", MessageBoxButton.OK, MessageBoxImage.Error);
               }
            }
         }
         finally
         {
            Mouse.OverrideCursor = null;
            IsEnabled = true;
         }
      }

      private void btnLogView_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Configure open file dialog wpf wrapped box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
               InitialDirectory = Path.Combine(userAppData, @"Logs"),
               DefaultExt = ".log",
               Filter = "Log files (*.log)|*.log|Archive logs (*.*)|*.*",
               FileName = "*.log",
               FilterIndex = 2,
               Title = "Select name to view contents"
            };

            // Show open file dialog box
            bool? result = dlg.ShowDialog(this);

            // Process open file dialog box results
            if (result == true)
            {
               Process word = Process.Start("Wordpad.exe", '"' + dlg.FileName + '"');
               if (word != null)
               {
                  word.WaitForInputIdle();
                  SendKeys.SendWait("^{END}");
               }
            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("OpenFile has an exception: ", ex);
            Microsoft.Windows.Controls.MessageBox.Show(this, ex.Message, "Failed to open the client log view", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }


      #endregion

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         try
         {
            ReadConfigDetails(out csd);
            ClientConfigDetails = csd;
         }
         catch (Exception ex)
         {
            Log.ErrorException("Unable to ReadConfigDetails", ex);
         }
      }

   }
}