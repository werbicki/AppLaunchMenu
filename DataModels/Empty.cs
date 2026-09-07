using AppLaunchMenu.DataAccess;
using System;

namespace AppLaunchMenu.DataModels
{
    public class Empty : DataModelBase, IElementName
    {
        public Empty()
            : base(new LaunchMenuFile(), new Type[] { }, "Empty")
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(Empty); }
        }
   }
}