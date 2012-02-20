using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AmalgamClientTray
{
   /// <summary>
   /// Interaction logic for Management.xaml
   /// </summary>
   public partial class Management : Window
   {
      WindowState lastWindowState;
      bool shouldClose;

      public Management()
      {
         InitializeComponent();
      }

      protected override void OnInitialized(EventArgs e)
      {
         base.OnInitialized(e);
         lastWindowState = WindowState;
         Hide();
      }

      protected override void OnStateChanged(EventArgs e)
      {
         if (this.WindowState == WindowState.Minimized)
         {
            this.Hide();
         }
         else
         {
            this.lastWindowState = this.WindowState;
         }
      }

      protected override void OnClosing(CancelEventArgs e)
      {
         if (!shouldClose)
         {
            e.Cancel = true;
            Hide();
         }
      }

      private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if (e.ChangedButton == MouseButton.Left)
         {
            Open();
         }
      }

      private void OnMenuItemOpenClick(object sender, EventArgs e)
      {
         Open();
      }

      private void Open()
      {
         Show();
         WindowState = lastWindowState;
      }

      private void OnMenuItemExitClick(object sender, EventArgs e)
      {
         shouldClose = true;
         Close();
      }

      private void button1_Click(object sender, RoutedEventArgs e)
      {
         MessageBox.Show("Hello World");
      }

      private void OnMenuItemAboutClick(object sender, EventArgs e)
      {
         new WPFAboutBox1(null).ShowDialog();
      }


   }
}