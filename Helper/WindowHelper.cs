//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using ABI.Windows.Foundation;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage;
using WinRT.Interop;

namespace AppLaunchMenu.Helper
{
    // Helper class to allow the app to find the Window that contains an
    // arbitrary UIElement (GetWindowForElement).  To do this, we keep track
    // of all active Windows.  The app code must call WindowHelper.CreateWindow
    // rather than "new Window" so we can keep track of all the relevant
    // windows.  In the future, we would like to support this in platform APIs.
    public class WindowHelper
    {
        static public Window CreateWindow()
        {
            Window newWindow = new Window
            {
                SystemBackdrop = new MicaBackdrop()
            };
            TrackWindow(newWindow);
            return newWindow;
        }

        static public void TrackWindow(Window window)
        {
            window.Closed += (sender,args) => {
                _activeWindows.Remove(window);
            };
            _activeWindows.Add(window);
        }

        static public AppWindow GetAppWindow(Window window)
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(window);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        static public Window? GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (Window window in _activeWindows)
                {
                    if (element.XamlRoot == window.Content.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        // get dpi for an element
        static public double GetRasterizationScaleForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (Window window in _activeWindows)
                {
                    if (element.XamlRoot == window.Content.XamlRoot)
                    {
                        return element.XamlRoot.RasterizationScale;
                    }
                }
            }
            return 0.0;
        }

        static public List<Window> ActiveWindows { get { return _activeWindows; }}

        static private List<Window> _activeWindows = new List<Window>();

        static public StorageFolder GetAppLocalFolder()
        {
            StorageFolder localFolder;
            if (!NativeHelper.IsAppPackaged)
            {
                localFolder = Task.Run(async () => await StorageFolder.GetFolderFromPathAsync(System.AppContext.BaseDirectory)).Result;
            }
            else
            {
                localFolder = ApplicationData.Current.LocalFolder;
            }
            return localFolder;
        }

        static public void Resize(Window p_objWindow, Windows.Foundation.Size p_objSize)
        {
            // Retrieve the window handle (HWND) of the current (XAML) WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(p_objWindow);

            // Retrieve the WindowId that corresponds to hWnd.
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

            // Lastly, retrieve the AppWindow for the current (XAML) WinUI 3 window.
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow?.Resize(new Windows.Graphics.SizeInt32 { Width = (int)p_objSize.Width, Height = (int)p_objSize.Height });
        }

        public static void CenterOnScreen(Window p_objWindow, Windows.Foundation.Size p_objSize)
        {
            var a = DisplayArea.Primary;
            if (a != null)
            {
                var outerBounds = a.WorkArea;

                var midtY = outerBounds.Height / 2;
                var startY = midtY - (p_objSize.Height / 2);

                if (startY < 0)
                    startY = 0;

                var midtX = outerBounds.Width / 2;
                var startX = midtX - (p_objSize.Width / 2);

                if (startX < 0)
                    startX = 0;

                // Retrieve the window handle (HWND) of the current (XAML) WinUI 3 window.
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(p_objWindow);

                // Retrieve the WindowId that corresponds to hWnd.
                Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

                // Lastly, retrieve the AppWindow for the current (XAML) WinUI 3 window.
                Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                appWindow?.MoveAndResize(new RectInt32((int)startX, (int)startY, (int)p_objSize.Width, (int)p_objSize.Height));
            }
        }
    }
}
