#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="NotifyIconHandler.cs" company="Smurf-IV">
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
using System.Windows.Forms;
using AmalgamClientTray.ClientForms;

namespace AmalgamClientTray.TrayHandlers
{
   public partial class NotifyIconHandler : UserControl
   {
      public NotifyIconHandler()
      {
         InitializeComponent();
         if (Properties.Settings.Default.UpdateRequired)
         {
            // Thanks go to http://cs.rthand.com/blogs/blog_with_righthand/archive/2005/12/09/246.aspx
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.UpdateRequired = false;
            Properties.Settings.Default.Save();
         }
         notifyIcon1.Icon = Properties.Resources.Amalgam;
      }

      private void exitToolStripMenuItem_Click(object sender, EventArgs e)
      {
         notifyIcon1.Visible = false;
         Application.Exit();
      }

      private void managementApp_Click(object sender, EventArgs e)
      {
         new ManagementForm().Show(this);
      }

      private void notifyIcon1_DoubleClick(object sender, EventArgs e)
      {
         managementApp_Click(sender, e);
      }

   }
}
