#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ClientPropertiesDisplay.cs" company="Smurf-IV">
// 
//  Copyright (C) 2012 Smurf-IV
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable RedundantThisQualifier
namespace AmalgamClientTray
{
   public enum BalloonTipIcon
   {
// ReSharper disable UnusedMember.Global
      None = 0,
      Info = 1,
      Warning = 2,
      Error = 3,
// ReSharper restore UnusedMember.Global
   }
   /// <summary>
   /// Represents a thin wrapper for <see cref="Forms.NotifyIcon"/>
   /// </summary>
   [ContentProperty("Text")]
   [DefaultEvent("MouseDoubleClick")]
   public partial class NotificationAreaIcon : FrameworkElement, IAddChild, IDisposable
   {
      public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent(
         "MouseClick",
         RoutingStrategy.Bubble,
         typeof(MouseButtonEventHandler),
         typeof(NotificationAreaIcon));

      public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
         "MouseDoubleClick",
         RoutingStrategy.Bubble,
         typeof(MouseButtonEventHandler),
         typeof(NotificationAreaIcon));

      public static readonly DependencyProperty BalloonTipIconProperty =
         DependencyProperty.Register("BalloonTipIcon", typeof(BalloonTipIcon), typeof(NotificationAreaIcon));

      public static readonly DependencyProperty BalloonTipTextProperty =
         DependencyProperty.Register("BalloonTipText", typeof(string), typeof(NotificationAreaIcon));

      public static readonly DependencyProperty BalloonTipTitleProperty =
         DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(NotificationAreaIcon));

      public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
         "Icon",
         typeof(ImageSource),
         typeof(NotificationAreaIcon),
         new FrameworkPropertyMetadata(OnIconChanged));

      public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
         "Text",
         typeof(string),
         typeof(NotificationAreaIcon),
         new PropertyMetadata(OnTextChanged));

      private Forms.NotifyIcon notifyIcon;

      static NotificationAreaIcon()
      {
         VisibilityProperty.OverrideMetadata(typeof(NotificationAreaIcon), new PropertyMetadata(OnVisibilityChanged));
      }

      public event MouseButtonEventHandler MouseClick
      {
         add { this.AddHandler(MouseClickEvent, value); }
         remove { this.RemoveHandler(MouseClickEvent, value); }
      }

      public event MouseButtonEventHandler MouseDoubleClick
      {
         add { this.AddHandler(MouseDoubleClickEvent, value); }
         remove { this.RemoveHandler(MouseDoubleClickEvent, value); }
      }

      public BalloonTipIcon BalloonTipIcon
      {
         get { return (BalloonTipIcon)this.GetValue(BalloonTipIconProperty); }
         set { this.SetValue(BalloonTipIconProperty, value); }
      }

      public string BalloonTipText
      {
         get { return (string)this.GetValue(BalloonTipTextProperty); }
         set { this.SetValue(BalloonTipTextProperty, value); }
      }

      public string BalloonTipTitle
      {
         get { return (string)this.GetValue(BalloonTipTitleProperty); }
         set { this.SetValue(BalloonTipTitleProperty, value); }
      }

      public ImageSource Icon
      {
         get { return (ImageSource)this.GetValue(IconProperty); }
         set { this.SetValue(IconProperty, value); }
      }

      public string Text
      {
         get { return (string)this.GetValue(TextProperty); }
         set { this.SetValue(TextProperty, value); }
      }

      public override void BeginInit()
      {
         base.BeginInit();
         this.InitializeNotifyIcon();
      }

      public void ShowBalloonTip(int timeout)
      {
         this.notifyIcon.BalloonTipTitle = this.BalloonTipTitle;
         this.notifyIcon.BalloonTipText = this.BalloonTipText;
         this.notifyIcon.BalloonTipIcon = (Forms.ToolTipIcon)this.BalloonTipIcon;
         this.notifyIcon.ShowBalloonTip(timeout);
      }

      public void ShowBalloonTip(int timeout, string tipTitle, string tipText, BalloonTipIcon tipIcon)
      {
         this.notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, (Forms.ToolTipIcon)tipIcon);
      }

      #region IAddChild Members

      void IAddChild.AddChild(object value)
      {
         throw new InvalidOperationException();
      }

      void IAddChild.AddText(string text)
      {
         if (text == null)
         {
            throw new ArgumentNullException("text");
         }

         this.Text = text;
      }

      #endregion

      protected override void OnVisualParentChanged(DependencyObject oldParent)
      {
         base.OnVisualParentChanged(oldParent);
         this.AttachToWindowClose();
      }

      private static MouseButtonEventArgs CreateMouseButtonEventArgs(
         RoutedEvent handler,
         Forms.MouseButtons button)
      {
         return new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(button))
         {
            RoutedEvent = handler
         };
      }

      private static Drawing.Icon FromImageSource(ImageSource icon)
      {
         if (icon == null)
         {
            return null;
         }

         Uri iconUri = new Uri(icon.ToString());
         StreamResourceInfo resourceStream = Application.GetResourceStream(iconUri);
         if (resourceStream != null)
            return new Drawing.Icon(resourceStream.Stream);
         return null;
      }

      private static void OnIconChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
      {
         if (!DesignerProperties.GetIsInDesignMode(target))
         {
            NotificationAreaIcon control = (NotificationAreaIcon)target;
            control.notifyIcon.Icon = FromImageSource(control.Icon);
         }
      }

      private static void OnTextChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
      {
         NotificationAreaIcon control = (NotificationAreaIcon)target;
         control.notifyIcon.Text = control.Text;
      }

      private static void OnVisibilityChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
      {
         NotificationAreaIcon control = (NotificationAreaIcon)target;
         if ( control != null )
            control.notifyIcon.Visible = control.Visibility == Visibility.Visible;
      }

      private static MouseButton ToMouseButton(Forms.MouseButtons button)
      {
         switch (button)
         {
            case Forms.MouseButtons.Left:
               return MouseButton.Left;
            case Forms.MouseButtons.Right:
               return MouseButton.Right;
            case Forms.MouseButtons.Middle:
               return MouseButton.Middle;
            case Forms.MouseButtons.XButton1:
               return MouseButton.XButton1;
            case Forms.MouseButtons.XButton2:
               return MouseButton.XButton2;
         }

         throw new InvalidOperationException();
      }

      private void AttachToWindowClose()
      {
         var window = Window.GetWindow(this);
         if (window != null)
         {
            window.Closed += (s, a) => this.notifyIcon.Dispose();
         }
      }

      private void InitializeNotifyIcon()
      {
         this.notifyIcon = new Forms.NotifyIcon
                              {
                                 Text = this.Text,
                                 Icon = FromImageSource(this.Icon),
                                 Visible = this.Visibility == Visibility.Visible
                              };

         this.notifyIcon.MouseDown += this.OnMouseDown;
         this.notifyIcon.MouseUp += this.OnMouseUp;
         this.notifyIcon.MouseClick += this.OnMouseClick;
         this.notifyIcon.MouseDoubleClick += this.OnMouseDoubleClick;

         this.InitializeNativeHooks();
      }

      private void OnMouseDown(object sender, Forms.MouseEventArgs e)
      {
         this.RaiseEvent(CreateMouseButtonEventArgs(MouseDownEvent, e.Button));
      }

      private void OnMouseDoubleClick(object sender, Forms.MouseEventArgs e)
      {
         this.RaiseEvent(CreateMouseButtonEventArgs(MouseDoubleClickEvent, e.Button));
      }

      private void OnMouseClick(object sender, Forms.MouseEventArgs e)
      {
         this.RaiseEvent(CreateMouseButtonEventArgs(MouseClickEvent, e.Button));
      }

      private void OnMouseUp(object sender, Forms.MouseEventArgs e)
      {
         if (e.Button == Forms.MouseButtons.Right)
         {
            this.ShowContextMenu();
         }

         this.RaiseEvent(CreateMouseButtonEventArgs(MouseUpEvent, e.Button));
      }

      private void ShowContextMenu()
      {
         if (this.ContextMenu != null)
         {
            this.AttachContextMenu();
            this.ContextMenu.IsOpen = true;
         }
      }

      partial void AttachContextMenu();

      partial void InitializeNativeHooks();

      #region Dispose
      /// <summary>
      /// Set to true as soon as <see cref="Dispose"/>
      /// has been invoked.
      /// </summary>
      public bool IsDisposed { get; private set; }


      /// <summary>
      /// Checks if the object has been disposed and
      /// raises a <see cref="ObjectDisposedException"/> in case
      /// the <see cref="IsDisposed"/> flag is true.
      /// </summary>
      private void EnsureNotDisposed()
      {
         if (IsDisposed)
            throw new ObjectDisposedException(Name ?? GetType().FullName);
      }


      /// <summary>
      /// Disposes the class if the application exits.
      /// </summary>
      private void OnExit(object sender, EventArgs e)
      {
         Dispose();
      }


      /// <summary>
      /// This destructor will run only if the <see cref="Dispose()"/>
      /// method does not get called. This gives this base class the
      /// opportunity to finalize.
      /// <para>
      /// Important: Do not provide destructors in types derived from
      /// this class.
      /// </para>
      /// </summary>
      ~NotificationAreaIcon()
      {
         Dispose(false);
      }


      /// <summary>
      /// Disposes the object.
      /// </summary>
      /// <remarks>This method is not virtual by design. Derived classes
      /// should override <see cref="Dispose(bool)"/>.
      /// </remarks>
      public void Dispose()
      {
         Dispose(true);

         // This object will be cleaned up by the Dispose method.
         // Therefore, you should call GC.SupressFinalize to
         // take this object off the finalization queue 
         // and prevent finalization code for this object
         // from executing a second time.
         GC.SuppressFinalize(this);
      }


      /// <summary>
      /// Closes the tray and releases all resources.
      /// </summary>
      /// <summary>
      /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
      /// If disposing equals <c>true</c>, the method has been called directly
      /// or indirectly by a user's code. Managed and unmanaged resources
      /// can be disposed.
      /// </summary>
      /// <param name="disposing">If disposing equals <c>false</c>, the method
      /// has been called by the runtime from inside the finalizer and you
      /// should not reference other objects. Only unmanaged resources can
      /// be disposed.</param>
      /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
      /// the method has already been called.</remarks>
      private void Dispose(bool disposing)
      {
         //don't do anything if the component is already disposed
         if (IsDisposed || !disposing)
            return;

         lock (this)
         {
            IsDisposed = true;

            //deregister application event listener
            if (Application.Current != null)
            {
               Application.Current.Exit -= OnExit;
            }

            //remove icon
            notifyIcon.Visible = false;
         }
      }


      #endregion
   }
}
// ReSharper restore RedundantThisQualifier
// ReSharper restore EventNeverSubscribedTo.Global
// ReSharper restore MemberCanBePrivate.Global
