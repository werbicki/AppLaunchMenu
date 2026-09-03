using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Empty : DataModelBase
    {
        public Empty()
            : base(new LaunchMenuFile(), new Type[] { }, "Empty")
        {
        }

        internal static string ElementName
        {
            get { return nameof(Empty); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }
   }
}