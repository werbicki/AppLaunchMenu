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
    public partial class ScriptListViewModel : ViewModelTreeBase<ScriptList>
    {
        public ScriptListViewModel(ScriptList p_objScriptList, LaunchMenu p_objLaunchMenu)
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
