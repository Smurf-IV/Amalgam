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
