#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="App.xaml.cs" company="Smurf-IV">
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
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using NLog;

namespace AmalgamClientTray
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      static private readonly Logger Log = LogManager.GetCurrentClassLogger();

      private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
      {
         Exception CurrentException = e.Exception;
         Log.FatalException("Application_DispatcherUnhandledException:\n", CurrentException);
         while (CurrentException != null)
         {
            MessageBox.Show(CurrentException.Message, "Report the logfile to http://amalgam.codeplex.com");
            CurrentException = CurrentException.InnerException;
         }
      }

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         Log.Error("=====================================================================");
         Log.Error("File Re-opened: Ver :" + Assembly.GetExecutingAssembly().GetName().Version);
      }

      private void Application_Exit(object sender, ExitEventArgs e)
      {
         Log.Error("File Closing");
         Log.Error("=====================================================================");
      }
   }
}
