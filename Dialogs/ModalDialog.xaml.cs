using AppLaunchMenu.Helper;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu.Dialogs
{
    public partial class ModalDialog : WindowNotifyPropertyChanged
    {
        private OverlappedPresenter? m_objOverlappedPresenter = null;
        private bool m_blnSizeProvided = false;
        private TaskCompletionSource<bool> m_objDialogResultTrigger = new TaskCompletionSource<bool>();
        private ContentDialogResult m_objDialogResult = ContentDialogResult.None;
        private ContentDialogButton m_objDefaultButton = ContentDialogButton.None;
        private String m_strMessage = "";
        private String m_strPrimaryButtonText = "";
        private String m_strSecondaryButtonText = "";
        private String m_strCloseButtonText = "OK";

        public ModalDialog()
        {
            Initialize(new Size(100, 100));
        }

        public ModalDialog(Size p_objSize)
        {
            m_blnSizeProvided = true;
            Initialize(p_objSize);
        }

        private void Initialize(Size p_objSize)
        {
            this.InitializeComponent();

            if (App.MainWindow != null)
                SetWindowOwner(owner: App.MainWindow);

            m_objOverlappedPresenter = OverlappedPresenter.CreateForDialog();
            m_objOverlappedPresenter.IsModal = true;
            m_objOverlappedPresenter.IsResizable = true;
            AppWindow.SetPresenter(m_objOverlappedPresenter);

            ResizeClient(p_objSize);

            RootGrid.Loaded += RootGrid_Loaded;
            RootGrid.DataContext = this;

            Closed += ModalDialog_Closed;
            SizeChanged += ModalDialog_SizeChanged;
        }

        private void ModalDialog_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
        }

        private void ModalDialog_Closed(object sender, WindowEventArgs args)
        {
            App.MainWindow?.Activate();
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_blnSizeProvided)
            {
                // 1. Force the layout engine to measure the required content size
                RootGrid.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));

                // 2. Obtain your intended content dimensions
                double desiredWidth = RootGrid.DesiredSize.Width;
                double desiredHeight = RootGrid.DesiredSize.Height;

                // 3. Get the native window handle and look up display DPI
                System.IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                uint dpi = NativeMethods.GetDpiForWindow(hWnd);
                double scalingFactor = dpi / 96.0;

                // 4. Convert XAML values (DIPs) to raw physical pixels
                int physicalWidth = (int)(desiredWidth * scalingFactor);
                int physicalHeight = (int)(desiredHeight * scalingFactor);

                // 5. Account for the native OS title bar and border sizing metrics
                // These system offsets ensure content isn't clipped by window borders
                int extraWidth = NativeMethods.GetSystemMetricsForDpi(NativeMethods.SystemMetricsIndex.SM_CXSIZEFRAME, dpi) * 2;
                int extraHeight = NativeMethods.GetSystemMetricsForDpi(NativeMethods.SystemMetricsIndex.SM_CYSIZEFRAME, dpi) * 2
                                 + NativeMethods.GetSystemMetricsForDpi(NativeMethods.SystemMetricsIndex.SM_CYCAPTION, dpi);

                int finalWidth = physicalWidth + extraWidth;
                int finalHeight = physicalHeight + extraHeight;

                // 6. Apply dimensions to AppWindow
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                ResizeClient(new Size(finalWidth, finalHeight));
            }
        }

        public bool IsResizable
        {
            set
            {
                OverlappedPresenter objOverlappedPresenter = (OverlappedPresenter)AppWindow.Presenter;
                objOverlappedPresenter.IsResizable = value;
            }
        }

        public Page Page
        {
            set
            {
                InnerFrame.Content = value;
                OnPropertyChanged(nameof(Page));
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

                InnerMessage.Text = m_strMessage;

                RootGrid.InvalidateMeasure();
                RootGrid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                ResizeClient(RootGrid.DesiredSize);
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

        public async Task<ContentDialogResult> ShowAsync()
        {
            AppWindow.Show();

            await m_objDialogResultTrigger.Task;

            return DialogResult;
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

            DialogResult = ContentDialogResult.Primary;
        }

        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

            DialogResult = ContentDialogResult.Secondary;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

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
    }
}
