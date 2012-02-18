#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="LogDisplay.cs" company="Smurf-IV">
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
using System.IO;
using System.Windows.Forms;
using NLog;

namespace AmalgamClientTray.ClientForms
{
   /// <summary>
   /// 
   /// </summary>
   public partial class LogDisplay : Form
   {
      private readonly string LogLocation;
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();

      /// <summary>
      /// 
      /// </summary>
      public LogDisplay(string logLocation)
      {
         InitializeComponent();
         LogLocation = logLocation;
      }


      private void OpenFile()
      {
         try
         {
            OpenFileDialog openFileDialog = new OpenFileDialog
                                               {
                                                  InitialDirectory =
                                                     Path.Combine(
                                                        Environment.GetFolderPath(
                                                           Environment.SpecialFolder.CommonApplicationData), LogLocation),
                                                  Filter = "Log files (*.log)|*.log|Archive logs (*.*)|*.*",
                                                  FileName = "*.log",
                                                  FilterIndex = 2,
                                                  Title = "Select name to view contents"
                                               };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
               UseWaitCursor = true;
               using (StreamReader reader = new StreamReader(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
               {
                  string line;
                  while (null != (line = reader.ReadLine()))
                  {
                     textBox1.Items.Add(line);
                  }
               }
               int count = textBox1.Items.Count - 1;
               if (count > 1)
                  textBox1.SetSelected(count, true);
            }
            else
            {
               Close();
            }
         }
         catch (Exception ex)
         {
            Log.ErrorException("OpenFile has an exception: ", ex);
            textBox1.Items.Add(ex.Message);
         }
         finally
         {
            UseWaitCursor = false;
         }
      }

      private void done_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void LogDisplay_Shown(object sender, EventArgs e)
      {
         OpenFile();
      }

   }
}