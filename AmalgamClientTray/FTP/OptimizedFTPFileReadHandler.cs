using AmalgamClientTray.ClientForms;

namespace AmalgamClientTray.FTP
{
   public class OptimizedFTPFileReadHandler : FileStreamFTP
   {
      private TemporaryFile ftpReadBuffer;

      public OptimizedFTPFileReadHandler(ClientShareDetail csd, uint rawCreationDisposition, FileFTPInfo foundFileInfo)
         : base(csd, rawCreationDisposition, foundFileInfo)
      {
      }
   }
}
