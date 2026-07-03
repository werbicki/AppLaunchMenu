using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuFileViewModel : ViewModelBase
    {
        LaunchMenu m_objLaunchMenu;
        MenuFile m_objMenuFile = new();
        NetworkDriveListViewModel m_objNetworkDriveListViewModel;
        ScriptListViewModel m_objScriptListViewModel;
        MenuListViewModel m_objMenuListViewModel;
        EnvironmentViewModel m_objEnvironmentViewModel;

        public MenuFileViewModel(LaunchMenu p_objLaunchMenu)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objNetworkDriveListViewModel = new NetworkDriveListViewModel(m_objLaunchMenu, m_objMenuFile.NetworkDriveList);
            m_objScriptListViewModel = new ScriptListViewModel(m_objLaunchMenu, m_objMenuFile.ScriptList);
            m_objMenuListViewModel = new MenuListViewModel(m_objLaunchMenu, this, m_objMenuFile.MenuList);
            m_objEnvironmentViewModel = new EnvironmentViewModel(m_objLaunchMenu, m_objMenuFile.Environment);
        }

        public MenuFileViewModel(LaunchMenu p_objLaunchMenu, MenuFile p_objMenuFile)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objMenuFile = p_objMenuFile;
            m_objNetworkDriveListViewModel = new NetworkDriveListViewModel(m_objLaunchMenu, m_objMenuFile.NetworkDriveList);
            m_objScriptListViewModel = new ScriptListViewModel(m_objLaunchMenu, m_objMenuFile.ScriptList);
            m_objMenuListViewModel = new MenuListViewModel(m_objLaunchMenu, this, m_objMenuFile.MenuList);
            m_objEnvironmentViewModel = new EnvironmentViewModel(m_objLaunchMenu, m_objMenuFile.Environment);
        }

        public bool CanEdit
        {
            get { return m_objMenuFile.CanEdit; }
        }

        public NetworkDriveListViewModel NetworkDriveListViewModel
        {
            get { return m_objNetworkDriveListViewModel; }
        }

        public ScriptListViewModel ScriptListViewModel
        {
            get { return m_objScriptListViewModel; }
        }

        public MenuListViewModel MenuListViewModel
        {
            get { return m_objMenuListViewModel; }
        }

        public EnvironmentViewModel EnvironmentViewModel
        {
            get { return m_objEnvironmentViewModel; }
        }

        public void Reload(LaunchMenu p_objLaunchMenu)
        {
            m_objMenuFile.Reload();

            OnPropertyChanged(nameof(MenuListViewModel));
        }
    }
}
