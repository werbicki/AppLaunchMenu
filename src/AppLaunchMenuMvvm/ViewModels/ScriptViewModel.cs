using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public class ScriptViewModel : ViewModelTreeBase<Script>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get { return []; }
        }

        public ScriptViewModel(Script p_objScript, ILaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objScript, p_objLaunchMenu, p_objParent)
        {
        }

        internal Script Script
        {
            get { return DataModel; }
        }

        [DialogContent("Language")]
        public string Language
        {
            get { return DataModel.Language; }
            set
            {
                DataModel.Language = value;
                OnPropertyChanged(nameof(Language));
            }
        }

        [DialogContent("Code")]
        public string Code
        {
            get { return DataModel.Code; }
            set
            {
                DataModel.Code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
    }
}
