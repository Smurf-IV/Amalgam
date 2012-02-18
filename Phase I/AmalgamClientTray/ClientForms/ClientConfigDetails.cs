#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ClientConfigDetails.cs" company="Smurf-IV">
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

namespace AmalgamClientTray.ClientForms
{
   [Serializable]
   public class ClientConfigDetails
   {
      // ReSharper disable UnusedAutoPropertyAccessor.Global
      // ReSharper disable MemberCanBePrivate.Global

      public List<ClientShareDetail> SharesToRestore = new List<ClientShareDetail>();
      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedAutoPropertyAccessor.Global
   }
}
