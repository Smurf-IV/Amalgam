using System;
using System.Windows;
using WindowsRecipes.TaskbarSingleInstance;
using WindowsRecipes.TaskbarSingleInstance.Wpf;

namespace AmalgamClientTray
{
   static class Program
   {
      [STAThread]
      static void Main()
      {
         using (SingleInstanceManager manager = SingleInstanceManager.Initialize(GetSingleInstanceManagerSetup()))
         {
            App app = new App();
            app.InitializeComponent();
            app.Run();
         }
      }

      private static SingleInstanceManagerSetup GetSingleInstanceManagerSetup()
      {
         return new SingleInstanceManagerSetup("AmalgamClientTray")
         {
            //ArgumentsHandler = args => ((App)Application.Current).ProcessCommandLineArgs(args),
            ArgumentsHandlerInvoker = new ApplicationDispatcherInvoker(),
            DelivaryFailureNotification = ex => MessageBox.Show(ex.Message, "An error occured"),
            InstanceNotificationOption = InstanceNotificationOption.NotifyAnyway
         };
      }
   }
}
