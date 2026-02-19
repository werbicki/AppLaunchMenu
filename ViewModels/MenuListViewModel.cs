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
    public partial class MenuListViewModel : ViewModelBase
    {
        LaunchMenu m_objLaunchMenu;
        MenuFile m_objMenuFile = new();
        ObservableCollection<MenuViewModel> m_objMenus = new();
        EnvironmentViewModel m_objEnvironmentViewModel;
        private int m_intSelectedMenu = 0;

        public MenuListViewModel(LaunchMenu p_objLaunchMenu)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objEnvironmentViewModel = new EnvironmentViewModel(m_objLaunchMenu, m_objMenuFile.MenuList.Environment);
        }

        public MenuListViewModel(LaunchMenu p_objLaunchMenu, MenuFile p_objMenuFile)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objMenuFile = p_objMenuFile;
            m_objEnvironmentViewModel = new EnvironmentViewModel(m_objLaunchMenu, m_objMenuFile.MenuList.Environment);

            SelectFirstMenu();
        }

        private void SelectFirstMenu()
        {
            m_intSelectedMenu = 0;

            foreach (DataModels.Menu objMenu in m_objMenuFile.MenuList.Menus)
                m_objMenus.Add(new MenuViewModel(m_objLaunchMenu, this, objMenu));

            OnPropertyChanged(nameof(Menus));

            if (m_objMenus.Count > 0)
            {
                SelectedMenu = m_objMenus[m_intSelectedMenu];

                OnPropertyChanged(nameof(SelectedMenu));
            }
        }

        public bool CanEdit
        {
            get { return m_objMenuFile.CanEdit; }
        }

        public ObservableCollection<MenuViewModel> Menus
        {
            get { return m_objMenus; }
        }

        public MenuViewModel? SelectedMenu
        {
            get
            {
                if ((Menus.Count > 0) && (m_intSelectedMenu < Menus.Count))
                    return Menus[m_intSelectedMenu];
                return null;
            }
            set
            {
                if (value != null)
                {
                    m_intSelectedMenu = Menus.IndexOf(value);

                    OnPropertyChanged(nameof(SelectedMenu));
                }
            }
        }

        public MenuViewModel? AddMenu(LaunchMenu p_objLaunchMenu, String p_strMenuName)
        {
            MenuViewModel? objMenuViewModel = null;
            Menu? objMenu = m_objMenuFile.AddMenu(p_strMenuName);
            if (objMenu != null)
            {
                objMenuViewModel = new MenuViewModel(p_objLaunchMenu, this, objMenu);
                m_objMenus.Add(objMenuViewModel);

                this.OnPropertyChanged(nameof(Menus));
            }

            return objMenuViewModel;
        }

        public void RemoveMenu(MenuViewModel p_objMenuViewModel)
        {
            if (m_objMenus.Remove(p_objMenuViewModel))
            {
                m_objMenuFile.RemoveMenu(p_objMenuViewModel.Menu);

                this.OnPropertyChanged(nameof(Menus));
            }
        }

        public void Reload(LaunchMenu p_objLaunchMenu)
        {
            m_objMenuFile.Reload();

            m_objMenus.Clear();
            OnPropertyChanged(nameof(Menus));

            SelectFirstMenu();
        }

        public EnvironmentViewModel Environment
        {
            get { return m_objEnvironmentViewModel; }
        }
    }
}
