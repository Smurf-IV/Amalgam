#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="HiddenFormToAcceptCloseMessage.cs" company="Smurf-IV">
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AmalgamClientTray.ClientForms;
using AmalgamClientTray.Dokan;

namespace AmalgamClientTray.TrayHandlers
{
   public partial class HiddenFormToAcceptCloseMessage : Form
   {
      public HiddenFormToAcceptCloseMessage()
      {
         InitializeComponent();
      }

      private void HiddenFormToAcceptCloseMessage_Load(object sender, EventArgs e)
      {
         Visible = false;
         string configFile = ManagementForm.configFile;
         FileInfo fi = new FileInfo(configFile);
         if (fi.Exists)
         {
            ClientConfigDetails csd;
            ManagementForm.ReadConfigDetails(out csd);
            if (csd != null)
            {
               HandleMappingThread newMapping = new HandleMappingThread();
               Handlers.ClientMappings[csd.SharesToRestore[0].DriveLetter] = newMapping;
               newMapping.Start(csd.SharesToRestore[0]);
            }
         }
      }

      private void HiddenFormToAcceptCloseMessage_FormClosing(object sender, FormClosingEventArgs e)
      {
         try
         {
            if (Handlers.ClientMappings != null)
               foreach (KeyValuePair<string, HandleMappingThread> keyValuePair in Handlers.ClientMappings.Where(keyValuePair => keyValuePair.Value != null))
               {
                  keyValuePair.Value.Stop();
               }
         }
         finally
         {
            Application.Exit();
         }
      }
   }
}
