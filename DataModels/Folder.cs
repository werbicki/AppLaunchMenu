using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Folder : DataModelBase
    {
        public Folder(XmlDocument p_objXmlDocument, XmlNode p_objFolderNode)
            : base(p_objXmlDocument, p_objFolderNode)
        {
        }

        public Folder(XmlDocument p_objXmlDocument, string p_strName)
            : base(p_objXmlDocument, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(Folder); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        internal Folder? CreateFolder(String p_strFolderName)
        {
            if (m_objXmlNode != null)
            {
                XmlElement? objFolderElement = m_objXmlDocument.CreateElement(Folder.ElementName);
                XmlAttribute? objFolderNameAttribute = m_objXmlDocument.CreateAttribute("Name");
                if ((objFolderElement != null) && (objFolderNameAttribute != null))
                {
                    objFolderNameAttribute.Value = p_strFolderName;
                    objFolderElement.Attributes.Append(objFolderNameAttribute);

                    return new Folder(m_objXmlDocument, objFolderElement);
                }
            }

            return null;
        }

        internal Application? CreateApplication(String p_strApplicationName)
        {
            if (m_objXmlNode != null)
            {
                XmlElement? objApplicationElement = m_objXmlDocument.CreateElement(Application.ElementName);
                XmlAttribute? objApplicationNameAttribute = m_objXmlDocument.CreateAttribute("Name");
                if ((objApplicationElement != null) && (objApplicationNameAttribute != null))
                {
                    objApplicationNameAttribute.Value = p_strApplicationName;
                    objApplicationElement.Attributes.Append(objApplicationNameAttribute);

                    return new Application(m_objXmlDocument, objApplicationElement);
                }
            }

            return null;
        }

        internal Environment CreateEnvironment()
        {
            XmlElement objEnvironmentElement = m_objXmlDocument.CreateElement(Environment.ElementName);
            return new Environment(m_objXmlDocument, objEnvironmentElement);
        }

        internal Variable? CreateVariable(String p_strVariableName)
        {
            if (m_objXmlNode != null)
            {
                XmlElement? objVariableElement = m_objXmlDocument.CreateElement(Variable.ElementName);
                XmlAttribute? objVariableNameAttribute = m_objXmlDocument.CreateAttribute("Name");
                if ((objVariableElement != null) && (objVariableNameAttribute != null))
                {
                    objVariableNameAttribute.Value = p_strVariableName;
                    objVariableElement.Attributes.Append(objVariableNameAttribute);

                    return new Variable(m_objXmlDocument, objVariableElement);
                }
            }

            return null;
        }

        internal override void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            if ((p_objObject is Folder) || (p_objObject is Environment))
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
            if ((p_objObject is Folder) || (p_objObject is Environment))
                p_objObject.XmlNode?.ParentNode?.RemoveChild(p_objObject.XmlNode);

            throw new ArgumentException();
        }

        public bool Expanded
        {
            get { return Property(nameof(Expanded)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { Property(nameof(Expanded), value ? "true" : "false"); }
        }

        public override DataModelBase[] Items
        {
            get
            {
                List<DataModelBase> objItems = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(Folder.ElementName + "|" + Application.ElementName + "|" + Environment.ElementName);
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
                            {
                                if (objItemNode.Name == Folder.ElementName)
                                    objItems.Add(new Folder(m_objXmlDocument, objItemNode));
                                else if (objItemNode.Name == Application.ElementName)
                                    objItems.Add(new Application(m_objXmlDocument, objItemNode));
                                else if (objItemNode.Name == Environment.ElementName)
                                    objItems.Add(new Environment(m_objXmlDocument, objItemNode));
                            }
                        }
                    }
                }

                return [.. objItems];
            }
        }

        public Folder[] Folders
        {
            get
            {
                List<Folder> objFolders = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(Folder.ElementName);

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
                                objFolders.Add(new Folder(m_objXmlDocument, objFolderNode));
                        }
                    }
                }

                return [.. objFolders];
            }
        }

        public Application[] Applications
        {
            get
            {
                List<Application> objApplications = new List<Application>();

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(Application.ElementName);

                    if (objNodes != null)
                    {
                        foreach (XmlNode objApplicationNode in objNodes)
                        {
                            bool blnInclude = true;

                            /*
                            if (!HostnameMatches(objMenuNode))
                                    blnInclude = false;
                            */

                            if (blnInclude)
                                objApplications.Add(new Application(m_objXmlDocument, objApplicationNode));
                        }
                    }
                }

                return [.. objApplications];
            }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = m_objXmlNode.SelectSingleNode("./" + Environment.ElementName);

                if (objEnvironmentNode == null)
                {
                    Environment objEnvironment = CreateEnvironment();

                    Insert(objEnvironment, 0);

                    return objEnvironment;
                }
                else
                    return new Environment(m_objXmlDocument, objEnvironmentNode);
            }
        }
    }
}
