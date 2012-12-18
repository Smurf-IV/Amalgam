#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Handlers.cs" company="Smurf-IV">
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

using System.Collections.Generic;
using CallbackFS;
using NLog;
using StringBuffers;

namespace AmalgamClientTray.CBFS
{
   internal static class Handlers
   {
      private static readonly Logger Log = LogManager.GetCurrentClassLogger();

      public static readonly Dictionary<string, HandleMappingThread> ClientMappings = new Dictionary<string, HandleMappingThread>();

      static Handlers()
      {
         CheckStatus();
         CallbackFileSystem.SetRegistrationKey(Properties.Settings.Default.Salt.FromBuffer());

         //CallbackFileSystem.GetModuleStatus( String ProductName, int Module, ref bool Installed, ref int FileVersionHigh, ref int FileVersionLow, ref SERVICE_STATUS ServiceStatus); 
         //CallbackFileSystem.Install(
      }

      private static bool CheckStatus()
        {
            bool isInstalled = false;
         
            int VersionHigh = 0, VersionLow = 0;
          
            SERVICE_STATUS status = new SERVICE_STATUS();
  
            CallbackFileSystem.GetModuleStatus("AmalgamClientTray", CallbackFileSystem.CBFS_MODULE_DRIVER, ref isInstalled, ref VersionHigh, ref VersionLow, ref status);
          
            if(isInstalled)
            {
                Log.Info("Driver (ver {0}.{1}.{2}.{3}) installed, service {4}", VersionHigh >> 16, VersionHigh & 0xFFFF, VersionLow >> 16, VersionLow & 0xFFFF, status.currentState);
            }
            else
            {
                Log.Fatal("Driver not installed");
            }
            return isInstalled;
        }
   }

}
