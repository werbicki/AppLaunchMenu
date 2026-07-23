using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using Environment = AppLaunchMenu.DataModels.Environment;

namespace AppLaunchMenu.ViewModels
{
    public partial class FolderViewModel : ViewModelTreeBase<Folder>
    {
        public FolderViewModel(Folder p_objFolder, LaunchMenu p_objLaunchMenu)
            : base(p_objFolder, p_objLaunchMenu)
        {
        }

        public FolderViewModel(Folder p_objFolder, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
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
