using AppLaunchMenu.DataAccess;
using AppLaunchMenu.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class DataCenter : DataModelBase
    {
        public DataCenter(LaunchMenuFile p_objMenuFile, XmlNode p_objDataCenterNode)
            : base(p_objMenuFile, new Type[] { }, p_objDataCenterNode)
        {
        }

        public DataCenter(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { }, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(DataCenter); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }
    }
}
