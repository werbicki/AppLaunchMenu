using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuViewModel : FolderViewModel
    {
        private readonly MenuListViewModel m_objMenusViewModel;
        private readonly Menu m_objMenu;
        private readonly Page m_objMenuPage;
        private readonly IconSource m_objDataIconSource = new SymbolIconSource() { Symbol = Symbol.Placeholder };

        public MenuViewModel(LaunchMenu p_objLaunchMenu, MenuListViewModel p_objMenusViewModel, Menu p_objMenu)
            : base(p_objLaunchMenu, p_objMenu)
        {
            m_objMenusViewModel = p_objMenusViewModel;
            m_objMenu = p_objMenu;
            m_objMenuPage = new MenuPage(p_objLaunchMenu, this);
        }

        internal Menu Menu
        {
            get { return m_objMenu; }
        }

        public IconSource Icon
        { 
            get { return m_objDataIconSource; }
        }

        public Page MenuPage
        {
            get { return m_objMenuPage; }
        }

        public override bool EditMode
        {
            get
            {
                if (LaunchMenu != null)
                    return LaunchMenu.EditMode;
                else 
                    return false;
            }
        }

        protected override void LoadChildren()
        {
            Children.Add(new EnvironmentViewModel(LaunchMenu, m_objMenusViewModel.Environment.Environment, this));

            base.LoadChildren();
        }
    }
}
