using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using NLog;

namespace AmalgamClientTray
{
   static class Program
   {
      private static readonly Logger Log = LogManager.GetCurrentClassLogger();

      [STAThread]
      static void Main()
      {
         try
         {
            AppDomain.CurrentDomain.UnhandledException += logUnhandledException;
         }
         catch (Exception ex)
         {
            try
            {
               Log.FatalException("Failed to attach unhandled exception handler...\n", ex);
            }
            catch
            {
            }
         }
         try
         {
            Log.Error("=====================================================================");
            Log.Error("File Re-opened: Ver :" + Assembly.GetExecutingAssembly().GetName().Version);
            Mutex AppUserMutex;
            if (CheckAndRunSingleApp(out AppUserMutex))
            {
               App app = new App();
               app.InitializeComponent();
               app.Run();
            }
         }
         catch (Exception ex)
         {
            Log.Fatal("Exception has not been caught by the rest of the application!", ex);
            MessageBox.Show(ex.Message, "Uncaught Exception - Exiting !");
         }
         finally
         {
            Log.Error("File Closing");
            Log.Error("=====================================================================");
         }
      }

      private static bool CheckAndRunSingleApp(out Mutex AppUserMutex)
      {
         string appName = Assembly.GetExecutingAssembly().ManifestModule.ScopeName;
         bool GrantedOwnership;
         AppUserMutex = new Mutex(true, appName, out GrantedOwnership);
         if (!GrantedOwnership)
         {
            MessageBox.Show(string.Format("{0} [{1}] is already running", appName, Environment.UserName), appName,
               MessageBoxButton.OK, MessageBoxImage.Stop);
         }
         return GrantedOwnership;
      }


      private static void logUnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         try
         {
            Log.Fatal("Unhandled exception.\r\n{0}", e.ExceptionObject);
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
               Log.FatalException("Exception details\n", ex);
            }
            else
            {
               Log.Fatal("Unexpected exception.");
            }
         }
         catch
         {
         }
      }
   }
}
