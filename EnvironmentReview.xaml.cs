using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using AppLaunchMenu.ViewModels;
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.UI.WindowManagement;
using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EnvironmentReview : PageNotifyPropertyChanged
    {
        private DataModels.Application? m_objApplication;
        private DataModels.Environment? m_objEnvironment;
        private EnvironmentViewModel? m_objEnvironmentViewModel;

        public EnvironmentReview()
        {
            this.InitializeComponent();
        }

        public EnvironmentReview(DataModels.Application p_objApplication)
        {
            m_objApplication = p_objApplication;
            m_objEnvironment = m_objApplication.Environment;
            if (m_objEnvironment != null)
                m_objEnvironmentViewModel = new EnvironmentViewModel(null, m_objEnvironment);

            this.InitializeComponent();
            m_objEnvironmentTable.DataContext = m_objEnvironmentViewModel;
        }

        public ObservableCollection<VariableViewModel> Variables
        {
            get
            {
                if (m_objEnvironmentViewModel != null)
                    return m_objEnvironmentViewModel.Variables;
                return [];
            }
        }
    }
}
