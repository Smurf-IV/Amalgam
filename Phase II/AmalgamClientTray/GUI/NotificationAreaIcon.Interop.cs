#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="NotificationAreaIcon.cs" company="Smurf-IV">
// 
//  Copyright (C) 2012 Simon Coghlan (aka Smurf-IV)
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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace AmalgamClientTray
{
   public partial class NotificationAreaIcon
   {
      private const int MouseGlobal = 14;
      private const int LeftButtonDown = 0x201;
      private const int RightButtonDown = 0x204;

      private int mouseHookHandle;
      private HookProc hookProcRef;

      private delegate int HookProc(int code, int wParam, IntPtr structPointer);

      [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      private static extern int CallNextHookEx(int hookId, int code, int param, IntPtr dataPointer);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      private static extern IntPtr GetModuleHandle(string moduleName);

      [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      private static extern int SetWindowsHookEx(int hookId, HookProc function, IntPtr instance, int threadId);

      [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      private static extern int UnhookWindowsHookEx(int hookId);

      private static Rect GetContextMenuRect(ContextMenu menu)
      {
         var begin = menu.PointToScreen(new Point(0, 0));
         var end = menu.PointToScreen(new Point(menu.ActualWidth, menu.ActualHeight));
         return new Rect(begin, end);
      }

      private static Point GetHitPoint(IntPtr structPointer)
      {
         MouseLLHook mouseHook = (MouseLLHook)Marshal.PtrToStructure(structPointer, typeof(MouseLLHook));
         return new Point(mouseHook.X, mouseHook.Y);
      }

      partial void AttachContextMenu()
      {
         this.ContextMenu.Opened += this.OnContextMenuOpened;
         this.ContextMenu.Closed += this.OnContextMenuClosed;
      }

      partial void InitializeNativeHooks()
      {
         this.hookProcRef = this.OnMouseEventProc;
      }

      private void OnContextMenuClosed(object sender, RoutedEventArgs e)
      {
         UnhookWindowsHookEx(this.mouseHookHandle);

         this.ContextMenu.Opened -= this.OnContextMenuOpened;
         this.ContextMenu.Closed -= this.OnContextMenuClosed;
      }

      private void OnContextMenuOpened(object sender, RoutedEventArgs e)
      {
         using (var process = Process.GetCurrentProcess())
         using (var module = process.MainModule)
         {
            this.mouseHookHandle = SetWindowsHookEx(
               MouseGlobal,
               this.hookProcRef,
               GetModuleHandle(module.ModuleName),
               0);
         }

         if (this.mouseHookHandle == 0)
         {
            throw new Win32Exception(Marshal.GetLastWin32Error());
         }
      }

      private int OnMouseEventProc(int code, int button, IntPtr dataPointer)
      {
         if (button == LeftButtonDown || button == RightButtonDown)
         {
            var contextMenuRect = GetContextMenuRect(this.ContextMenu);
            var hitPoint = GetHitPoint(dataPointer);

            if (!contextMenuRect.Contains(hitPoint))
            {
               this.ContextMenu.IsOpen = false;
            }
         }

         return CallNextHookEx(this.mouseHookHandle, code, button, dataPointer);
      }

      [StructLayout(LayoutKind.Sequential)]
      private struct MouseLLHook
      {
         internal int X;
         internal int Y;
         internal int MouseData;
         internal int Flags;
         internal int Time;
         internal int ExtraInfo;
      }
   }
}