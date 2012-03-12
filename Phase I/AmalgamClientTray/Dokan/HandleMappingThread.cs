#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="HandleMappingThread.cs" company="Smurf-IV">
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

using System.Collections.Generic;
using System.Threading;
using AmalgamClientTray.ClientForms;

namespace AmalgamClientTray.Dokan
{
   internal static class Handlers
   {
      public static readonly Dictionary<string, HandleMappingThread> ClientMappings = new Dictionary<string, HandleMappingThread>();
   }

   class HandleMappingThread
   {
      private DokanManagement mapManager;

      public bool Start(ClientShareDetail csd)
      {
         if (mapManager != null)
            mapManager.Stop();
         
         mapManager = new DokanManagement {csd = csd};
         ThreadPool.QueueUserWorkItem(mapManager.Start, mapManager);
         int repeatWait = 10;
         while (!mapManager.IsRunning
            && (repeatWait-- > 0)
            )
         {
            Thread.Sleep(250);
         }

         return mapManager.IsRunning;
      }

      public bool Stop()
      {
         bool runningState;
         if (mapManager != null)
         {
            mapManager.Stop();
            int repeatWait = 10;
            while (mapManager.IsRunning
               && (repeatWait-- > 0)
               )
            {
               Thread.Sleep(250);
               //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
               //                           new Action(delegate { }));
            }
            runningState = mapManager.IsRunning;
         }
         else
         {
            runningState = true;
         }
         return ! runningState;
      }
   }
}
