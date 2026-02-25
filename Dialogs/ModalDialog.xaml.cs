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
        private OverlappedPresenter? m_objOverlappedPresenter = null;
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

            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

            if (App.MainWindow != null)
                SetWindowOwner(owner: App.MainWindow);

            m_objOverlappedPresenter = OverlappedPresenter.CreateForDialog();
            m_objOverlappedPresenter.IsModal = true;
            AppWindow.SetPresenter(m_objOverlappedPresenter);

            CenterWindow();

            OuterFrame.DataContext = this;

            Closed += ModalDialog_Closed;
        }

        public bool IsResizable
        {
            set
            {
                OverlappedPresenter objOverlappedPresenter = (OverlappedPresenter)AppWindow.Presenter;
                objOverlappedPresenter.IsResizable = value;
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
                OuterFrame.Content = value;

                OuterFrame.InvalidateMeasure();
                //OuterFrame.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                OuterFrame.SizeChanged += Frame_SizeChanged;

                ResizeClient(OuterFrame.DesiredSize);
            }
        }

        public Page Page
        {
            set
            {
                InnerFrame.Content = value;
                OnPropertyChanged(nameof(Page));

                OuterFrame.InvalidateMeasure();
                //OuterFrame.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                InnerFrame.SizeChanged += Frame_SizeChanged;

                ResizeClient(OuterFrame.DesiredSize);
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

                OuterFrame.InvalidateMeasure();
                OuterFrame.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                ResizeClient(OuterFrame.DesiredSize);
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

        private void ModalDialog_Closed(object sender, WindowEventArgs args)
        {
            if (App.MainWindow != null)
                App.MainWindow.Activate();
        }

        private void Frame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InnerFrame.SizeChanged -= Frame_SizeChanged;

            ResizeClient(InnerFrame.DesiredSize);
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
