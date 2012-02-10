using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmalgamClientTray.FTP
{
   class FTPTempFile : TemporaryFile
   {

      public int Position
      {
         get { return (int) IO.Position; }
         set 
         { 
            if ( value > IO.Length )
               throw new ArgumentOutOfRangeException("value", "Requested length greater than cached temp contents");
            IO.Position = value;
         }
      }



      public FTPTempFile(bool shortLived)
         : base(shortLived)
      {
      }
   }
}
