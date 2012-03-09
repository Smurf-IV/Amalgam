﻿#region Copyright (C)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AmalgamClientTray.ClientForms;
using AmalgamClientTray.Dokan;
using NLog;
using Application = System.Windows.Forms.Application;

namespace AmalgamClientTray.GUI
{
   /// <summary>
   /// Interaction logic for HiddenFormToAcceptCloseMessage.xaml
   /// </summary>
   public partial class HiddenFormToAcceptCloseMessage : Window
   {
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();

      public HiddenFormToAcceptCloseMessage()
      {
         InitializeComponent();
      }

      protected override void OnInitialized(EventArgs e)
      {
         base.OnInitialized(e);
         ClientConfigDetails csd;
         bool allowStart = Management.ReadConfigDetails(out csd);
         if (allowStart)
         {
            HandleMappingThread newMapping = new HandleMappingThread();
            Handlers.ClientMappings[csd.SharesToRestore[0].DriveLetter] = newMapping;
            newMapping.Start(csd.SharesToRestore[0]);
         }
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         notificationIcon.Visibility = Visibility.Hidden;
         try
         {
            if (Handlers.ClientMappings != null)
               foreach (KeyValuePair<string, HandleMappingThread> keyValuePair in Handlers.ClientMappings
                                                                  .Where(keyValuePair => keyValuePair.Value != null)
                                                                  )
               {
                  keyValuePair.Value.Stop();
               }
         }
         finally
         {
            Application.Exit();
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
         new Management().ShowDialog();
      }

      private void OnMenuItemExitClick(object sender, EventArgs e)
      {
         Close();
      }

      private void OnMenuItemAboutClick(object sender, EventArgs e)
      {
         new WPFAboutBox1(null).ShowDialog();
      }
      #endregion


   }
}
