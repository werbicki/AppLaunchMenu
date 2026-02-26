using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class NetworkDriveListing : DataModelBase
    {
        protected Folder m_objFolder;

        public NetworkDriveListing(MenuFile p_objMenuFile, Folder p_objFolder, XmlNode p_objFolderNode)
            : base(p_objMenuFile, p_objFolderNode)
        {
            m_objFolder = p_objFolder;
        }

        public NetworkDriveListing(MenuFile p_objMenuFile, Folder p_objFolder, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
            m_objFolder = p_objFolder;
        }

        internal static string ElementName
        {
            get { return nameof(NetworkDriveListing); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        internal NetworkDrive? CreateNetworkDrive(String p_strNetworkDriveName)
        {
            if (m_objXmlNode != null)
            {
                XmlElement? objNetworkDriveElement = m_objMenuFile.XmlDocument.CreateElement(Application.ElementName);
                XmlAttribute? objNetworkDriveNameAttribute = m_objMenuFile.XmlDocument.CreateAttribute("Name");
                if ((objNetworkDriveElement != null) && (objNetworkDriveNameAttribute != null))
                {
                    objNetworkDriveNameAttribute.Value = p_strNetworkDriveName;
                    objNetworkDriveElement.Attributes.Append(objNetworkDriveNameAttribute);

                    return new NetworkDrive(m_objMenuFile, this, objNetworkDriveElement);
                }
            }

            return null;
        }

        internal override void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            if (p_objObject is NetworkDrive)
            {
                if (p_intIndex >= 0)
                    m_objXmlNode?.InsertBefore(p_objObject.XmlNode, m_objXmlNode?.ChildNodes[p_intIndex]);
                else
                    m_objXmlNode?.AppendChild(p_objObject.XmlNode);
            }
            else
                throw new ArgumentException();
        }

        internal override void Remove(DataModelBase p_objObject)
        {
            if (p_objObject is NetworkDrive)
                p_objObject.XmlNode?.ParentNode?.RemoveChild(p_objObject.XmlNode);

            throw new ArgumentException();
        }

        public Folder Folder
        {
            get { return m_objFolder; }
        }

        public override DataModelBase[] Items
        {
            get
            {
                List<DataModelBase> objItems = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(NetworkDrive.ElementName);
                    if (objNodes != null)
                    {
                        foreach (XmlNode objItemNode in objNodes)
                        {
                            bool blnInclude = true;

                            /*
                            if (!HostnameMatches(objItemNode))
                                    blnInclude = false;
                            */

                            if (blnInclude)
                                objItems.Add(new NetworkDrive(m_objMenuFile, this, objItemNode));
                        }
                    }
                }

                return [.. objItems];
            }
        }

        public NetworkDrive[] Drives
        {
            get
            {
                List<NetworkDrive> objFolders = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(NetworkDrive.ElementName);

                    if (objNodes != null)
                    {
                        foreach (XmlNode objFolderNode in objNodes)
                        {
                            bool blnInclude = true;

                            /*
                            if (!HostnameMatches(objMenuNode))
                                    blnInclude = false;
                            */

                            if (blnInclude)
                                objFolders.Add(new NetworkDrive(m_objMenuFile, this, objFolderNode));
                        }
                    }
                }

                return [.. objFolders];
            }
        }
    }
}
