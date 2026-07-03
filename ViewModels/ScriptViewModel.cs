using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class ScriptViewModel : TreeViewItemViewModel
    {
        private readonly ScriptListViewModel m_objScriptListViewModel;
        protected Script m_objScript;

        public ScriptViewModel(LaunchMenu? p_objLaunchMenu, ScriptListViewModel p_objScriptListViewModel, Script p_objScript)
            : base(p_objLaunchMenu)
        {
            m_objScriptListViewModel = p_objScriptListViewModel;
            m_objScript = p_objScript;
        }

        internal Script Script
        {
            get { return m_objScript; }
        }

        public override string Name
        {
            get { return m_objScript.Name; }
            set
            {
                m_objScript.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Language
        {
            get { return m_objScript.Language; }
            set
            {
                m_objScript.Language = value;
                OnPropertyChanged(nameof(Language));
            }
        }

        public string Code
        {
            get { return m_objScript.Code; }
            set
            {
                m_objScript.Code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
    }
}
