using AppLaunchMenu.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        DataModelCollection<Variable> m_objExpandedVariables;

        public Environment(MenuFile p_objMenuFile, XmlNode p_objEnvironmentNode)
             : base(p_objMenuFile, new Type[] { typeof(Variable) }, p_objEnvironmentNode)
        {
            m_objVariables = new(this, null);
            m_objExpandedVariables = new(this, null);

            UpdateItems();
        }

        internal static string ElementName
        {
            get { return nameof(Environment); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        private void Initialize(List<XmlNode> p_objIncludedNodes, XmlNodeList? p_objNodeList)
        {
            if (p_objNodeList != null)
            {
                foreach (XmlNode objNode in p_objNodeList)
                {
                    String strName = "";
                    bool blnInclude = true;

                    if ((objNode.Attributes != null)
                        && (objNode.Attributes["Name"] != null)
                        )
                        strName = objNode.Attributes["Name"]!.Value;

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
                                p_objIncludedNodes[intIndex] = objNode;

                                blnFound = true;
                            }
                        }

                        if (!blnFound)
                            p_objIncludedNodes.Add(objNode);
                    }
                }
            }
        }

        protected override void UpdateItems()
        {
            m_objVariables.Clear();

            foreach (DataModelBase objObject in Items)
            {
                if (objObject is Variable)
                    m_objVariables.Add((Variable)objObject);
            }

            XmlNode? objRoot = MenuFile.XmlDocument.DocumentElement;
            XmlNode? objParent = MenuFile.XmlDocument.DocumentElement;

            m_objExpandedVariables.Clear();

            if (objRoot != null)
            {
                List<XmlNode> objIncludedNodes = [];
                XmlNodeList? objVariables = objRoot.SelectNodes("./" + Environment.ElementName + "/" + Variable.ElementName);
                Initialize(objIncludedNodes, objVariables);

                if (objParent != null)
                {
                    List<XmlNode> objMenuAndFolders = new List<XmlNode>();
                    XmlNode? objApplication = objParent;
                    XmlNode? objNode = null;

                    if (objApplication != null)
                        objNode = objApplication.ParentNode;

                    while (objNode != null)
                    {
                        if ((objNode.Name == MenuList.ElementName)
                            || (objNode.Name == Menu.ElementName)
                            || (objNode.Name == Folder.ElementName)
                            )
                            objMenuAndFolders.Insert(0, objNode);

                        objNode = objNode.ParentNode;
                    }

                    foreach (XmlNode objFolder in objMenuAndFolders)
                    {
                        objVariables = objFolder.SelectNodes("./" + Environment.ElementName + "/" + Variable.ElementName);
                        Initialize(objIncludedNodes, objVariables);
                    }

                    if (objApplication != null)
                        objVariables = objApplication.SelectNodes("./" + Environment.ElementName + "/" + Variable.ElementName);

                    Initialize(objIncludedNodes, objVariables);
                }

                m_objExpandedVariables = new DataModelCollection<Variable>(this, objIncludedNodes);

                if (XmlNode.ParentNode != null)
                    ExpandVariables(XmlNode.ParentNode);
            }
        }

        internal Variable? CreateVariable(String p_strVariableName)
        {
            Variable objVariable = (Variable)CreateChildNode(typeof(Variable), p_strVariableName);

            return objVariable;
        }

        public Variable[] Variables
        {
            get { return m_objVariables.ToArray(); }
        }

        public string this[string p_strName]
        {
            get
            {
                foreach (Variable objVariable in m_objExpandedVariables)
                {
                    if (objVariable.Name == p_strName)
                        return objVariable.ExpandedValue;
                }

                return string.Empty;
            }
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            return m_objExpandedVariables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_objExpandedVariables.GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (m_objExpandedVariables != null)
                    return m_objExpandedVariables.Count;

                return 0;
            }
        }

        private void ExpandVariables()
        {
            if (XmlNode != null)
                ExpandVariables(XmlNode);
        }

        private void ExpandVariables(XmlNode p_objReferenceNode)
        {
            bool blnExpanding;

            // Reset variable sto non-Expanede state.
            foreach (Variable objVariable in m_objExpandedVariables)
                objVariable.ExpandedValue = objVariable.Value;
        
            do
            {
                blnExpanding = false;

                // Expand Xml Document based references completely first.
                foreach (Variable objVariable in m_objExpandedVariables)
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
                foreach (Variable objVariable in m_objExpandedVariables)
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

            Match objMatch = Regex.Match(strString, "%[^%]+%|{([^}]|}})+}");
            while (objMatch.Success)
            {
                String strMatch = objMatch.Value;
                bool blnMatch = false;

                if (strMatch.StartsWith("%"))
                {
                    string strName = strMatch.Substring(1, strMatch.Length - 2);

                    if (!strName.StartsWith("xpath:", StringComparison.CurrentCultureIgnoreCase))
                    {
                        for (int i = 0; (!blnMatch) && (i < m_objExpandedVariables.Count); i++)
                        {
                            if ((m_objExpandedVariables.Item(i) != null)
                                && (m_objExpandedVariables.Item(i)!.Value != p_strString)
                                && (m_objExpandedVariables.Item(i)!.Name == strName)
                                )
                            {
                                strString = strString.Remove(intOffset + objMatch.Index, objMatch.Length);
                                strString = strString.Insert(intOffset + objMatch.Index, m_objExpandedVariables.Item(i)!.ExpandedValue);

                                blnMatch = true;
                            }
                        }
                    }
                }
                else if (strMatch.StartsWith("{"))
                {
                    string strScript = strMatch.Substring(1, strMatch.Length - 2);

                    Script? objScript = MenuFile.ScriptList.GetScriptByName(strScript);
                    if (objScript != null)
                    {
                        ScriptingHost objScriptingHost = new(this);
                        string strResult = objScript.RunScriptString(objScriptingHost);

                        strString = strString.Remove(intOffset + objMatch.Index, objMatch.Length);
                        strString = strString.Insert(intOffset + objMatch.Index, strResult);

                        blnMatch = true;
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

            foreach (Variable objVariable in m_objExpandedVariables)
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
