using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuViewModel : FolderViewModel
    {
        private readonly Menu m_objMenu;
        private readonly Page m_objMenuPage;
        private readonly IconSource m_objDataIconSource = new SymbolIconSource() { Symbol = Symbol.Placeholder };
        GridLength m_objTreeViewItemWidth = new(200.0);

        public MenuViewModel(LaunchMenu p_objLaunchMenu, Menu p_objMenu)
            : base(p_objLaunchMenu, p_objMenu)
        {
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

        public override GridLength TreeViewItemWidth
        {
            get { return m_objTreeViewItemWidth; }
            set
            {
                m_objTreeViewItemWidth = value;
                OnPropertyChanged(nameof(TreeViewItemWidth));
            }
        }

        protected override void OnLoadChildren()
        {
            Children.Add(LaunchMenu.MenuFileViewModel.NetworkDriveListViewModel);
            Children.Add(LaunchMenu.MenuFileViewModel.ScriptListViewModel);
            Children.Add(LaunchMenu.MenuFileViewModel.EnvironmentViewModel);

            base.OnLoadChildren();
        }
    }
}
