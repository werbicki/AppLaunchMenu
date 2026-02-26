using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Menu : Folder
    {
        public Menu(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public Menu(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal static new string ElementName
        {
            get { return nameof(Menu); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        internal NetworkDriveListing? CreateNetworkDrives(String p_strNetworkDrivesName)
        {
            if (m_objXmlNode != null)
            {
                XmlElement? objNetworkDrivesElement = m_objMenuFile.XmlDocument.CreateElement(Folder.ElementName);
                XmlAttribute? objNetworkDrivesNameAttribute = m_objMenuFile.XmlDocument.CreateAttribute("Name");
                if ((objNetworkDrivesElement != null) && (objNetworkDrivesNameAttribute != null))
                {
                    objNetworkDrivesNameAttribute.Value = p_strNetworkDrivesName;
                    objNetworkDrivesElement.Attributes.Append(objNetworkDrivesNameAttribute);

                    return new NetworkDriveListing(m_objMenuFile, this, objNetworkDrivesElement);
                }
            }

            return null;
        }
    }
}
