using AppLaunchMenu.DataAccess;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Menu : Folder
    {
        public Menu(LaunchMenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public Menu(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public new static string ElementName
        {
            get { return nameof(Menu); }
        }
    }
}
