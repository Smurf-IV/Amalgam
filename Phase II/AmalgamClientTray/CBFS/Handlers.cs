using System.Collections.Generic;
using CallbackFS;

namespace AmalgamClientTray.CBFS
{
   internal static class Handlers
   {
      public static readonly Dictionary<string, HandleMappingThread> ClientMappings = new Dictionary<string, HandleMappingThread>();

      static Handlers()
      {
         CallbackFileSystem.SetRegistrationKey(@"A536784AC4756C445FBC119EA3601502D78A9C17EEB482E08DBA5FBC42CE3E2C6F6C02081AD7186C2F2C2998E229DBBBD7190BA1AC818E134C212EB3C4F962C7DC11BA5F801526EB28BD4ED314897257F8CDDEE3B0C5165B68DD8AAFCDFAF2D715020B889A3FFDAAA2076784F5E22EB395828956EFCCF5E23522E6AB51DECDFAFA9F84998340F5E2476479C6FED3");

         //CallbackFileSystem.GetModuleStatus( String ProductName, int Module, ref bool Installed, ref int FileVersionHigh, ref int FileVersionLow, ref SERVICE_STATUS ServiceStatus); 
         //CallbackFileSystem.Install(
      }
   }

}
