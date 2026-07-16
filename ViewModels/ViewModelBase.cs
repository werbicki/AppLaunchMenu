using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public abstract class ViewModelBase : ViewModelNotifyBase
    {
        DataModelBase m_objDataModel;

        protected ViewModelBase(DataModelBase p_objDataModel)
        {
            m_objDataModel = p_objDataModel;
        }
    }
}
