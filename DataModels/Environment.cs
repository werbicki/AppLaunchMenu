using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace AppLaunchMenu.DataModels
{
    public class Environment : DataModelBase, IEnumerable<Variable>
    {
        DataModelCollection<Variable> m_objVariables;

        public Environment(XmlDocument p_objXmlDocument, XmlNode p_objEnvironmentNode)
             : base(p_objXmlDocument, p_objEnvironmentNode)
        {
            m_objVariables = new(p_objXmlDocument, null);

            if (p_objXmlDocument != null)
                Initialize(p_objXmlDocument, p_objXmlDocument, p_objEnvironmentNode.ParentNode);
        }

        internal override void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            if (p_objObject is Variable)
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
            if (p_objObject is Variable)
                p_objObject.XmlNode?.ParentNode?.RemoveChild(p_objObject.XmlNode);
            else
                throw new ArgumentException();
        }

        protected override string ElementName
        {
            get { return nameof(Environment); }
        }

        public override DataModelBase[] Items
        {
            get
            {
                List<DataModelBase> objItems = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes("Variable");
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
                                if (objItemNode.Name == "Variable")
                                    objItems.Add(new Variable(m_objXmlDocument, objItemNode));
                            }
                        }
                    }
                }

                return [.. objItems];
            }
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            return m_objVariables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_objVariables.GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (m_objVariables != null)
                    return m_objVariables.Count;

                return 0;
            }
        }

        private static void Initialize(List<XmlNode> p_objIncludedNodes, XmlNodeList? p_objVariables)
        {
            if (p_objVariables != null)
            {
                foreach (XmlNode objVariable in p_objVariables)
                {
                    String strName = "";
                    bool blnInclude = true;

                    if ((objVariable.Attributes != null)
                        && (objVariable.Attributes["Name"] != null)
                        )
                        strName = objVariable.Attributes["Name"]!.Value;

                    if (strName == "AB_OLF_CONFIG_FILE")
                        blnInclude = true;

                    /*
                    XmlNode? objEnvironmentNode = objVariable.ParentNode;
                    if ((!DomainMatches(objEnvironmentNode))
                        || (!DataCenterMatches(objEnvironmentNode))
                        || (!HostnameMatches(objEnvironmentNode))
                        || (!IsServiceMatches(objEnvironmentNode))
                        )
                        blnInclude = false;
                        */

                    if (blnInclude)
                    {
                        bool blnFound = false;

                        for (int intIndex = 0; (!blnFound) && (intIndex < p_objIncludedNodes.Count); intIndex++)
                        {
                            String strExistingName = "";

                            if ((p_objIncludedNodes[intIndex] != null)
                                && (p_objIncludedNodes[intIndex].Attributes != null)
                                && (p_objIncludedNodes[intIndex].Attributes!["Name"] != null)
                                )
                                strExistingName = p_objIncludedNodes[intIndex].Attributes!["Name"]!.Value;

                            if (strExistingName == strName)
                            {
                                p_objIncludedNodes[intIndex] = objVariable;

                                blnFound = true;
                            }
                        }

                        if (!blnFound)
                            p_objIncludedNodes.Add(objVariable);
                    }
                }
            }
        }

        private void Initialize(XmlDocument m_objDocument, XmlNode? p_objReferenceNode, XmlNode? p_objParent)
        {
            XmlNode? objRoot = m_objDocument.DocumentElement;

            if (objRoot != null)
            {
                List<XmlNode> objIncludedNodes = [];
                XmlNodeList? objVariables = objRoot.SelectNodes("./Environment/Variable");
                Initialize(objIncludedNodes, objVariables);

                if (p_objParent != null)
                {
                    List<XmlNode> objMenuAndFolders = new List<XmlNode>();
                    XmlNode? objApplication = p_objParent;
                    XmlNode? objNode = null;

                    if (objApplication != null)
                        objNode = objApplication.ParentNode;

                    while ((objNode != null) && (objNode.Name != "Menus"))
                    {
                        if ((objNode.Name == "Folder") || (objNode.Name == "Menu"))
                            objMenuAndFolders.Insert(0, objNode);

                        objNode = objNode.ParentNode;
                    }

                    foreach (XmlNode objFolder in objMenuAndFolders)
                    {
                        objVariables = objFolder.SelectNodes("./Environment/Variable");
                        Initialize(objIncludedNodes, objVariables);
                    }

                    if (objApplication != null)
                        objVariables = objApplication.SelectNodes("./Environment/Variable");

                    Initialize(objIncludedNodes, objVariables);
                }

                m_objVariables = new DataModelCollection<Variable>(m_objXmlDocument, objIncludedNodes);

                if (p_objReferenceNode != null)
                    ExpandVariables(p_objReferenceNode);
            }
        }

        private void ExpandVariables()
        {
            if (m_objXmlNode != null)
                ExpandVariables(m_objXmlNode);
        }

        private void ExpandVariables(XmlNode p_objReferenceNode)
        {
            bool blnExpanding;

            // Reset variable sto non-Expanede state.
            foreach (Variable objVariable in m_objVariables)
                objVariable.ExpandedValue = objVariable.Value;
        
            do
            {
                blnExpanding = false;

                // Expand Xml Document based references completely first.
                foreach (Variable objVariable in m_objVariables)
                {
                    String strValue = objVariable.ExpandedValue;
                    String strExpandedValue = ExpandVariable(objVariable, p_objReferenceNode);

                    if (strValue != strExpandedValue)
                    {
                        objVariable.ExpandedValue = strExpandedValue;
                        blnExpanding = true;
                    }
                }

                // Expand Environment Variables second.
                foreach (Variable objVariable in m_objVariables)
                {
                    String strValue = objVariable.ExpandedValue;
                    String strExpandedValue = ExpandVariable(objVariable);

                    if (strValue != strExpandedValue)
                    {
                        objVariable.ExpandedValue = strExpandedValue;
                        blnExpanding = true;
                    }
                }
            } while (blnExpanding);
        }

        private static String ExpandVariable(Variable p_objVariable, XmlNode p_objReferenceNode)
        {
            String strString = p_objVariable.ExpandedValue;

            if (p_objReferenceNode != null)
            {
                int intOffset = 0;

                Match objMatch = Regex.Match(strString, "%xpath:[^%]+%");
                while (objMatch.Success)
                {
                    String strName = objMatch.Value.Substring(7, objMatch.Value.Length - 8);
                    XPathExpression objExpression = XPathExpression.Compile(strName);
                    XPathNavigator? objNavigator = p_objReferenceNode.CreateNavigator();

                    if (objNavigator != null)
                    {
                        strString = strString.Remove(intOffset + objMatch.Index, objMatch.Length);

                        switch (objExpression.ReturnType)
                        {
                            case XPathResultType.NodeSet:
                                {
                                    XPathNodeIterator? objNodes = objNavigator.Select(objExpression);
                                    if ((objNodes != null)
                                        && (objNodes.Current != null)
                                        )
                                        strString = strString.Insert(intOffset + objMatch.Index, objNodes.Current.ToString());
                                    break;
                                }
                            case XPathResultType.Number:
                                if (objNavigator.Evaluate(objExpression) != null)
                                    strString = strString.Insert(intOffset + objMatch.Index, objNavigator.Evaluate(objExpression).ToString() ?? "");
                                break;
                            case XPathResultType.Boolean:
                                strString = strString.Insert(intOffset + objMatch.Index, ((bool)objNavigator.Evaluate(objExpression) ? "True" : "False"));
                                break;
                            case XPathResultType.String:
                                strString = strString.Insert(intOffset + objMatch.Index, (String)objNavigator.Evaluate(objExpression));
                                break;
                        }

                        intOffset++;

                        intOffset += objMatch.Index;
                        objMatch = Regex.Match(strString.Substring(intOffset), "%[^%]+%");
                    }
                }
            }

            return strString;
        }

        private String ExpandVariable(Variable p_objVariable)
        {
            return ExpandVariable(p_objVariable.ExpandedValue);
        }

        public String ExpandVariable(String p_strString)
        {
            String strString = System.Environment.ExpandEnvironmentVariables(p_strString);
            int intOffset = 0;

            Match objMatch = Regex.Match(strString, "%[^%]+%");
            while (objMatch.Success)
            {
                String strName = objMatch.Value.Substring(1, objMatch.Value.Length - 2);
                bool blnMatch = false;

                if (!strName.StartsWith("xpath:", StringComparison.CurrentCultureIgnoreCase))
                {
                    for (int i = 0; (!blnMatch) && (i < m_objVariables.Count); i++)
                    {
                        if ((m_objVariables.Item(i) != null)
                            && (m_objVariables.Item(i)!.Value != p_strString)
                            && (m_objVariables.Item(i)!.Name == strName)
                            )
                        {
                            strString = strString.Remove(intOffset + objMatch.Index, objMatch.Length);
                            strString = strString.Insert(intOffset + objMatch.Index, m_objVariables.Item(i)!.ExpandedValue);

                            blnMatch = true;
                        }
                    }
                }

                if (!blnMatch)
                    intOffset++;

                intOffset += objMatch.Index;
                objMatch = Regex.Match(strString.Substring(intOffset), "%[^%]+%");
            }

            return strString;
        }

        public void Validate()
        {
            String strMessage = "";

            foreach (Variable objVariable in m_objVariables)
            {
                if (objVariable.Validation.Equals("directory", StringComparison.CurrentCultureIgnoreCase))
                {
                    DirectoryInfo objDirectoryInfo = new(objVariable.ExpandedValue);

                    if (!objDirectoryInfo.Exists)
                    {
                        if (strMessage.Length > 0)
                            strMessage += "\n";
                        strMessage += "Directory '" + objDirectoryInfo + "' does not exist!";
                    }
                }
                else if (objVariable.Validation.ToLower() == "file")
                {
                    FileInfo objFilenfo = new(objVariable.ExpandedValue);

                    if (!objFilenfo.Exists)
                    {
                        if (strMessage.Length > 0)
                            strMessage += "\n";
                        strMessage += "File '" + objFilenfo + "' does not exist!";
                    }
                }
            }

            if (strMessage.Length > 0)
                throw new InvalidOperationException(strMessage);
        }
    }
}
