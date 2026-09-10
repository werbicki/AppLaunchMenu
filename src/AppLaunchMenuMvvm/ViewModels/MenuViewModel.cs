using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Application = AppLaunchMenu.DataModels.Application;
using Environment = AppLaunchMenu.DataModels.Environment;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuViewModel : ViewModelTreeBase<Menu>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get
            {
                return
                [
                    new() { DataModelType = typeof(Folder), ViewModelType = typeof(FolderViewModel) },
                    new() { DataModelType = typeof(Environment), ViewModelType = typeof(EnvironmentViewModel) },
                    new() { DataModelType = typeof(Application), ViewModelType = typeof(ApplicationViewModel) },
                ];
            }
        }

        private readonly Menu m_objMenu;
        private readonly IconSource m_objDataIconSource = new SymbolIconSource() { Symbol = Symbol.Placeholder };
        private GridLength m_objTreeViewItemWidth = new GridLength(200.0, GridUnitType.Auto);
        private double m_dblTreeViewItemMinWidth = 200.0;

        public MenuViewModel(Menu p_objMenu, ILaunchMenu p_objLaunchMenu)
            : base(p_objMenu, p_objLaunchMenu)
        {
            m_objMenu = p_objMenu;
        }

        protected override void OnLoadChildren()
        {
            Children.Add(LaunchMenu.MenuFileViewModel);

            foreach (EnvironmentViewModel objEnvironmentViewModel in Collection<EnvironmentViewModel, DataModels.Environment>(this))
                Children.Add(objEnvironmentViewModel);

            foreach (FolderViewModel objFolderViewModel in Collection<FolderViewModel, Folder>(this))
                Children.Add(objFolderViewModel);

            foreach (ApplicationViewModel objApplicationViewModel in Collection<ApplicationViewModel, DataModels.Application>(this))
                Children.Add(objApplicationViewModel);
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

        public new double TreeViewItemMinWidth
        {
            get { return m_dblTreeViewItemMinWidth; }
            set
            {
                if (value >= 200.0)
                {
                    m_dblTreeViewItemMinWidth = value;
                    OnPropertyChanged(nameof(TreeViewItemMinWidth));

                    TreeViewItemWidth = new GridLength(value, GridUnitType.Pixel);
                }
            }
        }

        public override bool Expanded
        {
            get { return true; }
        }
    }
}
