using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class ScriptViewModel : ViewModelTreeBase<Script>
    {
        public ScriptViewModel(Script p_objScript, LaunchMenu p_objLaunchMenu)
            : base(p_objScript, p_objLaunchMenu)
        {
        }

        internal Script Script
        {
            get { return DataModel; }
        }

        public override string Name
        {
            get { return DataModel.Name; }
            set
            {
                DataModel.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Language
        {
            get { return DataModel.Language; }
            set
            {
                DataModel.Language = value;
                OnPropertyChanged(nameof(Language));
            }
        }

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
