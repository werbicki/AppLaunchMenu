using AppLaunchMenu;
using AppLaunchMenu.Helper;
using AppLaunchMenu.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu.Dialogs
{
    public partial class ModalDialog : WindowNotifyPropertyChanged
    {
        private Window? m_objParentWindow = null;
        private OverlappedPresenter? m_objOverlappedPresenter = null;
        private bool m_blnCentered = true;
        private Page? m_objInnerPage = null;
        private TaskCompletionSource<bool> m_objDialogResultTrigger = new TaskCompletionSource<bool>();
        private ContentDialogResult m_objDialogResult = ContentDialogResult.None;
        private ContentDialogButton m_objDefaultButton = ContentDialogButton.None;
        private String m_strMessage = "";
        private String m_strPrimaryButtonText = "";
        private String m_strSecondaryButtonText = "";
        private String m_strCloseButtonText = "OK";

        public ModalDialog()
        {
            this.InitializeComponent();
            RootElement.DataContext = this;

            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

            m_objOverlappedPresenter = OverlappedPresenter.CreateForDialog();

            // Set this modal window's owner (the main application window).
            // The main window can be retrieved from App.xaml.cs if it's set as a static property.
            if (App.MainWindow != null)
            {
                m_objParentWindow = App.MainWindow;
                SetWindowOwner(owner: App.MainWindow);
            }

            // Make the window modal (blocks interaction with the owner window until closed).
            m_objOverlappedPresenter.IsModal = true;

            // Apply the presenter settings to the AppWindow.
            AppWindow.SetPresenter(m_objOverlappedPresenter);

            // Center the window on the screen.
            CenterWindow();

            Closed += ModalWindow_Closed;
        }

        // Sets the owner window of the modal window.
        private void SetWindowOwner(Window owner)
        {
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

        private void ResizeClient(Size p_objSize)
        {
            AppWindow.ResizeClient(new SizeInt32((int)p_objSize.Width, (int)p_objSize.Height));

            if (m_blnCentered)
                CenterWindow();
        }

        private void CenterWindow()
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

        public async Task<ContentDialogResult> ShowAsync()
        {
            AppWindow.Show();
            Activate();

            await m_objDialogResultTrigger.Task;

            return DialogResult;
        }

        public bool IsResizable
        {
            set
            {
                OverlappedPresenter objOverlappedPresenter = (OverlappedPresenter)AppWindow.Presenter;
                objOverlappedPresenter.IsResizable = value;
            }
        }

        public bool IsCentered
        {
            set
            {
                m_blnCentered = value;
            }
        }

        public new UIElement Content
        {
            get
            {
                return base.Content;
            }
            set
            {
                base.Content = value;

                FrameworkElement objElement = (FrameworkElement)value;
                objElement.SizeChanged += RootElement_SizeChanged;

                value.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                AppWindow.ResizeClient(new SizeInt32((int)value.DesiredSize.Width, (int)value.DesiredSize.Height));
            }
        }

        public Page Page
        {
            set
            {
                m_objInnerPage = value;
                InnerFrame.Content = m_objInnerPage;
                OnPropertyChanged(nameof(Page));

                RootElement.InvalidateMeasure();
                AppWindow.ResizeClient(new SizeInt32((int)RootElement.DesiredSize.Width, (int)RootElement.DesiredSize.Height));
            }
        }

        public string Message
        {
            get
            {
                return m_strMessage;
            }
            set
            {
                m_strMessage = value;
                OnPropertyChanged(nameof(Message));

                RootElement.InvalidateMeasure();
                AppWindow.ResizeClient(new SizeInt32((int)RootElement.DesiredSize.Width, (int)RootElement.DesiredSize.Height));
            }
        }

        //
        // Summary:
        //     Gets or sets an instance Style that is applied for this object during layout
        //     and rendering.
        //
        // Returns:
        //     The applied style for the object, if present; otherwise, null. The default for
        //     a default-constructed FrameworkElement is null.
        public Style Style
        {
            get
            {
                return RootElement.Style;
            }
            set
            {
                RootElement.Style = value;
            }
        }

        public ContentDialogResult DialogResult
        {
            get
            {
                return m_objDialogResult;
            }
            private set
            {
                m_objDialogResult = value;
                m_objDialogResultTrigger.TrySetResult(true);
            }
        }

        public ContentDialogButton DefaultButton
        {
            get
            {
                return m_objDefaultButton;
            }
            set
            {
                m_objDefaultButton = value;
            }
        }

        private Style PrimaryButtonStyle
        {
            get
            {
                return (DefaultButton == ContentDialogButton.Primary ? (Style)RootElement.Resources["AccentButtonStyle"] : (Style)RootElement.Resources["DefaultButtonStyle"]);
            }
        }

        private Visibility PrimaryButtonVisible
        {
            get
            {
                return (m_strPrimaryButtonText.Length > 0 ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        private bool PrimaryButtonDefault
        {
            get
            {
                return (DefaultButton == ContentDialogButton.Primary);
            }
        }

        public string PrimaryButtonText
        {
            get
            {
                return m_strPrimaryButtonText;
            }
            set
            {
                m_strPrimaryButtonText = value;
                OnPropertyChanged(nameof(PrimaryButtonText));
                OnPropertyChanged(nameof(PrimaryButtonVisible));
            }
        }

        private Visibility SecondaryButtonVisible
        {
            get
            {
                return (m_strSecondaryButtonText.Length > 0 ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        private bool SecondaryButtonDefault
        {
            get
            {
                return (DefaultButton == ContentDialogButton.Secondary);
            }
        }

        public string SecondaryButtonText
        {
            get
            {
                return m_strSecondaryButtonText;
            }
            set
            {
                m_strSecondaryButtonText = value;
                OnPropertyChanged(nameof(SecondaryButtonText));
                OnPropertyChanged(nameof(SecondaryButtonVisible));
            }
        }

        private Visibility CloseButtonVisible
        {
            get
            {
                return (m_strCloseButtonText.Length > 0 ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        private bool CloseButtonDefault
        {
            get
            {
                return (DefaultButton == ContentDialogButton.Close);
            }
        }

        public string CloseButtonText
        {
            get
            {
                return m_strCloseButtonText;
            }
            set
            {
                m_strCloseButtonText = value;
                OnPropertyChanged(nameof(CloseButtonText));
                OnPropertyChanged(nameof(CloseButtonVisible));
            }
        }

        private void ModalWindow_Closed(object sender, WindowEventArgs args)
        {
            // Reactivate the main application window when the modal window closes.
            if (App.MainWindow != null)
                App.MainWindow.Activate();
        }

        private void ModalWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //InnerFrame.Height = InnerPage.ActualHeight;
            //InnerFrame.Width = InnerPage.ActualWidth;
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            DialogResult = ContentDialogResult.Primary;
        }

        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            DialogResult = ContentDialogResult.Secondary;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            DialogResult = ContentDialogResult.None;
        }

        private void RootElement_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                switch (DefaultButton)
                {
                    case ContentDialogButton.Primary:
                        PrimaryButton_Click(PrimaryButton, new RoutedEventArgs());
                        break;
                    case ContentDialogButton.Secondary:
                        SecondaryButton_Click(SecondaryButton, new RoutedEventArgs());
                        break;
                    case ContentDialogButton.Close:
                        CloseButton_Click(CloseButton, new RoutedEventArgs());
                        break;
                }

                e.Handled = true;
            }
        }

        private void RootElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeClient(Content.DesiredSize);
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {

        }
    }
}
