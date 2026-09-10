using AppLaunchMenu.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppLaunchMenu.DataAccess
{
    public interface ILaunchMenu
    {
        event PropertyChangedEventHandler? PropertyChanged;

        abstract bool EditMode
        {
            get;
        }

        abstract MenuFileViewModel MenuFileViewModel
        {
            get;
        }
    }
}
