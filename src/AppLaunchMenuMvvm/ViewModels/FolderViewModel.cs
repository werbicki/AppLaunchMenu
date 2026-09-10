using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using Environment = AppLaunchMenu.DataModels.Environment;

namespace AppLaunchMenu.ViewModels
{
    public partial class FolderViewModel : ViewModelTreeBase<Folder>
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

        public FolderViewModel(Folder p_objFolder, ILaunchMenu p_objLaunchMenu)
            : base(p_objFolder, p_objLaunchMenu)
        {
        }

        public FolderViewModel(Folder p_objFolder, ILaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objFolder, p_objLaunchMenu, p_objParent)
        {
        }

        protected override void OnLoadChildren()
        {
            foreach (EnvironmentViewModel objEnvironmentViewModel in Collection<EnvironmentViewModel, DataModels.Environment>(this))
                Children.Add(objEnvironmentViewModel);

            foreach (FolderViewModel objFolderViewModel in Collection<FolderViewModel, Folder>(this))
                Children.Add(objFolderViewModel);

            foreach (ApplicationViewModel objApplicationViewModel in Collection<ApplicationViewModel, DataModels.Application>(this))
                Children.Add(objApplicationViewModel);
        }

        [DialogContent("Folder Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        [DialogContent("Expanded")]
        public override bool Expanded
        {
            get { return DataModel.Expanded; }
            set
            {
                DataModel.Expanded = value;
                OnPropertyChanged(nameof(Expanded));
            }
        }
    }
}
