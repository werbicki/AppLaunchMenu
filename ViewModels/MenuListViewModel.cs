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
    public partial class MenuListViewModel : ViewModelBase<MenuList>
    {
        private int m_intSelectedMenu = -1;

        public MenuListViewModel(MenuList p_objMenuList, LaunchMenu p_objLaunchMenu)
            : base(p_objMenuList, p_objLaunchMenu)
        {
            if (Menus.Count > 0)
                SelectedMenu = Menus[0];

            OnPropertyChanged(nameof(SelectedMenu));
            OnPropertyChanged(nameof(SelectedMenuIndex));
        }

        public ObservableCollection<MenuViewModel> Menus
        {
            get { return Collection<MenuViewModel, Menu>(); }
        }

        public MenuViewModel SelectedMenu
        {
            get { return Menus[m_intSelectedMenu]; }
            set
            {
                int intSelectedMenu = Menus.IndexOf(value);

                if ((intSelectedMenu >= 0) && (intSelectedMenu < Menus.Count))
                    m_intSelectedMenu = intSelectedMenu;

                OnPropertyChanged(nameof(SelectedMenu));
                OnPropertyChanged(nameof(SelectedMenuIndex));
            }
        }

        public int SelectedMenuIndex
        {
            get { return m_intSelectedMenu; }
            set
            {
                if ((value >= 0) && (value < Menus.Count))
                    m_intSelectedMenu = value;

                OnPropertyChanged(nameof(SelectedMenuIndex));
            }
        }
    }
}
