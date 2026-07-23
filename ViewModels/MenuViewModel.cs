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
    public partial class MenuViewModel : ViewModelTreeBase<Menu>
    {
        private readonly Menu m_objMenu;
        private readonly Page m_objMenuPage;
        private readonly IconSource m_objDataIconSource = new SymbolIconSource() { Symbol = Symbol.Placeholder };
        GridLength m_objTreeViewItemWidth = new(200.0);

        public MenuViewModel(Menu p_objMenu, LaunchMenu p_objLaunchMenu)
            : base(p_objMenu, p_objLaunchMenu)
        {
            m_objMenu = p_objMenu;
            m_objMenuPage = new MenuPage(p_objLaunchMenu, this);
        }

        protected override void OnLoadChildren()
        {
            Children.Add(LaunchMenu.MenuFileViewModel.NetworkDriveListViewModel);
            Children.Add(LaunchMenu.MenuFileViewModel.ScriptListViewModel);
            Children.Add(LaunchMenu.MenuFileViewModel.EnvironmentViewModel);

            foreach (EnvironmentViewModel objEnvironmentViewModel in Collection<EnvironmentViewModel, DataModels.Environment>(this))
                Children.Add(objEnvironmentViewModel);

            foreach (FolderViewModel objFolderViewModel in Collection<FolderViewModel, Folder>(this))
                Children.Add(objFolderViewModel);

            foreach (ApplicationViewModel objApplicationViewModel in Collection<ApplicationViewModel, DataModels.Application>(this))
                Children.Add(objApplicationViewModel);
        }

        public Page MenuPage
        {
            get { return m_objMenuPage; }
        }

        public IconSource Icon
        {
            get { return m_objDataIconSource; }
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
        public override bool Expanded
        {
            get { return true; }
        }
    }
}
