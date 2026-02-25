using AppLaunchMenu.Helper;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using WinRT.Interop;

namespace AppLaunchMenu
{
    public class WindowNotifyPropertyChanged : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            // Log.DebugFormat("{0}.{1} = {2}", this.GetType().Name, propertyName, storage);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Window? m_objParentWindow = null;
        private bool m_blnCentered = true;

        // Sets the owner window of the modal window.
        protected void SetWindowOwner(Window owner)
        {
            m_objParentWindow = App.MainWindow;

            // Get the HWND (window handle) of the owner window (main window).
            IntPtr ownerHwnd = WindowNative.GetWindowHandle(owner);

            // Get the HWND of the AppWindow (modal window).
            IntPtr ownedHwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);

            // Set the owner window using SetWindowLongPtr for 64-bit systems
            // or SetWindowLong for 32-bit systems.
            if (IntPtr.Size == 8) // Check if the system is 64-bit
            {
                NativeMethods.SetWindowLongPtr(ownedHwnd, -8, ownerHwnd); // -8 = GWLP_HWNDPARENT
            }
            else // 32-bit system
            {
                NativeMethods.SetWindowLong(ownedHwnd, -8, ownerHwnd); // -8 = GWL_HWNDPARENT
            }
        }

        protected void ResizeClient(Size p_objSize)
        {
            AppWindow.ResizeClient(new SizeInt32((int)p_objSize.Width, (int)p_objSize.Height));

            if (m_blnCentered)
                CenterWindow();
        }

        protected void CenterWindow()
        {
            if (m_objParentWindow != null)
                AppWindow.Move(new PointInt32(m_objParentWindow.AppWindow.Position.X + (m_objParentWindow.AppWindow.Size.Width - AppWindow.Size.Width) / 2, m_objParentWindow.AppWindow.Position.Y + (m_objParentWindow.AppWindow.Size.Height - AppWindow.Size.Height) / 2));
            else
            {
                RectInt32? objDisplayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
                if (objDisplayArea != null)
                    AppWindow.Move(new PointInt32((objDisplayArea.Value.Width - AppWindow.Size.Width) / 2, (objDisplayArea.Value.Height - AppWindow.Size.Height) / 2));
            }
        }

        public bool Centered
        {
            get {  return m_blnCentered; }
            set { m_blnCentered = value; }
        }
    }
}
