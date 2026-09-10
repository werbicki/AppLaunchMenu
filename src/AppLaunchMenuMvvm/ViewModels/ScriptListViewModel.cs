using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System.Collections.ObjectModel;

namespace AppLaunchMenu.ViewModels
{
    public partial class ScriptListViewModel : ViewModelTreeBase<ScriptList>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get
            {
                return
                [
                    new() { DataModelType = typeof(Script), ViewModelType = typeof(ScriptViewModel) },
                ];
            }
        }

        public ScriptListViewModel(ScriptList p_objScriptList, ILaunchMenu p_objLaunchMenu)
            : base(p_objScriptList, p_objLaunchMenu)
        {
        }

        protected override void OnLoadChildren()
        {
            foreach (ScriptViewModel objScriptViewModel in Collection<ScriptViewModel, Script>(this))
                Children.Add(objScriptViewModel);
        }

        [DialogContent("Script List Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<ScriptViewModel> Scripts
        {
            get { return Collection<ScriptViewModel, Script>(); }
        }
    }
}
