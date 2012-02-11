﻿#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="DokanManagement.cs" company="Smurf-IV">
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
using System.Threading;
using AmalgamClientTray.ClientForms;
using AmalgamClientTray.FTP;
using DokanNet;
using NLog;
using Starksoft.Net.Ftp;

namespace AmalgamClientTray.Dokan
{
   internal class DokanManagement
   {
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();
      private char mountedDriveLetter;
      private LiquesceOps dokanOperations;
      private FtpClientExt ftpInstance;

      /// <summary>
      /// Invokes DokanNet.DokanMain function to mount a drive. 
      /// The function blocks until the file system is unmounted.
      /// Administrator privilege is needed to communicate with Dokan driver. 
      /// You need a manifest file for .NET application.
      /// </summary>
      /// <returns></returns>
      public void Start(object obj)
      {
         try
         {
            int repeatWait = 0;
            while (IsRunning
               && (repeatWait++ < 10)
               )
            {
               Log.Warn("Last Dokan is still running");
               Thread.Sleep(250);
            }
            if (!IsRunning)
            {
               Log.Info("Starting up");
               IsRunning = true;

               // TODO: Search all usages of the DriveLetter and make sure they become MountPoint compatible
               string mountPoint = csd.DriveLetter;
               //if (mountPoint.Length == 1)
               //   mountPoint += ":\\"; // Make this into a MountPoint for V 0.6.0
               DokanOptions options = new DokanOptions
               {
                  MountPoint = mountPoint,
                  ThreadCount = Properties.Settings.Default.DokanThreads,
                  DebugMode = Properties.Settings.Default.DokanDebug,
                  //      public bool UseStdErr;
                  // UseAltStream = true, // This needs all sorts of extra API's
                  UseKeepAlive = true,  // When you set TRUE on DokanOptions->UseKeepAlive, dokan library automatically unmounts 15 seconds after user-mode file system hanged up
                  NetworkDrive = true,  // Set this to true to see if it stops the recycler bin question until [workitem:7253] is sorted
                  VolumeLabel = csd.VolumeLabel
               };

               ftpInstance = new FtpClientExt( new FtpClient(csd.TargetMachineName, csd.Port, csd.SecurityProtocol) );
               ftpInstance.Open(csd.UserName, csd.Password);

               dokanOperations = new LiquesceOps(csd, ftpInstance);
               mountedDriveLetter = csd.DriveLetter[0];


               try
               {
                  Log.Info("DokanVersion:[{0}], DokanDriverVersion[{1}]", DokanNet.Dokan.DokanVersion(), DokanNet.Dokan.DokanDriverVersion());
                  DokanNet.Dokan.DokanUnmount(mountedDriveLetter);
               }
               catch (Exception ex)
               {
                  Log.InfoException("Make sure it's unmounted threw:", ex);
               }
               int retVal = DokanNet.Dokan.DokanMain(options, dokanOperations);
               Log.Warn("Dokan.DokanMain has exited");
               IsRunning = false;
               switch (retVal)
               {
                  case DokanNet.Dokan.DOKAN_SUCCESS: // = 0;
                     Log.Info("Dokan is not mounted");
                     break;
                  case DokanNet.Dokan.DOKAN_ERROR:// = -1; // General Error
                     Log.Info("Dokan is not mounted [DOKAN_ERROR] - General Error");
                     break;
                  case DokanNet.Dokan.DOKAN_DRIVE_LETTER_ERROR: // = -2; // Bad Drive letter
                     Log.Info("Dokan is not mounted [DOKAN_DRIVE_LETTER_ERROR] - Bad drive letter");
                     break;
                  case DokanNet.Dokan.DOKAN_DRIVER_INSTALL_ERROR: // = -3; // Can't install driver
                     Log.Info("Dokan is not mounted [DOKAN_DRIVER_INSTALL_ERROR]");
                     Environment.Exit(-1);
                     break;
                  case DokanNet.Dokan.DOKAN_START_ERROR: // = -4; // Driver something wrong
                     Log.Info("Dokan is not mounted [DOKAN_START_ERROR] - Driver Something is wrong");
                     Environment.Exit(-1);
                     break;
                  case DokanNet.Dokan.DOKAN_MOUNT_ERROR: // = -5; // Can't assign drive letter
                     Log.Info("Dokan is not mounted [DOKAN_MOUNT_ERROR] - Can't assign drive letter");
                     break;
                  default:
                     Log.Info("Dokan is not mounted [Uknown Error: {0}]", retVal);
                     Environment.Exit(-1);
                     break;
               }
            }
            else
            {
               Log.Info("Seems like the last exit request into Dokan did not exit in time");
            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("Start has failed in an uncontrolled way: ", ex);
            Environment.Exit(-1);
         }
         finally
         {
            IsRunning = false;
         }
      }
      public void Stop()
      {
         if (IsRunning)
         {
            Log.Info( "Stop has been requested");
            int retVal = DokanNet.Dokan.DokanUnmount(mountedDriveLetter);
            Log.Info("Stop returned[{0}]", retVal);
            if (ftpInstance != null)
            {
               ftpInstance.Close();
               ftpInstance = null;
            }
         }
      }


      public bool IsRunning { get; set; }

      public ClientShareDetail csd { get; set; }
   }
}
