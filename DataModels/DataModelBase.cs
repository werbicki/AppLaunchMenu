using AppLaunchMenu.DataAccess;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Windows.Devices.Power;
using Windows.Media.Audio;

namespace AppLaunchMenu.DataModels
{
    public abstract class DataModelBase : IComparable
    {
        protected MenuFile m_objMenuFile;
        protected XmlNode m_objXmlNode;

        protected DataModelBase(MenuFile p_objMenuFile, XmlNode p_objNode)
        {
            m_objMenuFile = p_objMenuFile;
            m_objXmlNode = p_objNode;
        }

        protected DataModelBase(MenuFile p_objMenuFile, string p_strName)
        {
            m_objMenuFile = p_objMenuFile;

            m_objXmlNode = CreateNode();

            if (!string.IsNullOrEmpty(p_strName))
            {
                XmlAttribute objNameAttribute = m_objMenuFile.XmlDocument.CreateAttribute("Name");
                objNameAttribute.Value = p_strName;
                m_objXmlNode.Attributes?.Append(objNameAttribute);
            }

            Name = p_strName;
        }

        internal XmlNode XmlNode
        {
            get { return m_objXmlNode; }
        }

        protected abstract string _ElementName
        {
            get;
        }

        protected XmlNode CreateNode()
        {
            return m_objMenuFile.XmlDocument.CreateNode("element", _ElementName, "");
        }

        internal virtual void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            throw new NotImplementedException();
        }

        internal virtual void Remove(DataModelBase p_objObject)
        {
            throw new NotImplementedException();
        }

        protected bool HasXmlAttribute(string p_strPropertyName)
        {
            if (m_objXmlNode != null
                && m_objXmlNode.Attributes != null
                && m_objXmlNode.Attributes[p_strPropertyName] != null
                )
                return !string.IsNullOrEmpty(m_objXmlNode.Attributes[p_strPropertyName]!.Value);

            return false;
        }

        protected string GetXmlAttribute(string p_strPropertyName)
        {
            if (m_objXmlNode != null
                && m_objXmlNode.Attributes != null
                && m_objXmlNode.Attributes[p_strPropertyName] != null
                )
                return m_objXmlNode.Attributes[p_strPropertyName]!.Value;

            return "";
        }

        protected void SetXmlAttribute(string p_strPropertyName, string value)
        {
            if ((m_objMenuFile != null)
                && (m_objXmlNode != null)
                && (m_objXmlNode.Attributes != null)
                )
            {
                if ((m_objXmlNode.Attributes[p_strPropertyName] == null)
                    && (!string.IsNullOrEmpty(value))
                    )
                {
                    XmlAttribute objXmlAttribute = m_objMenuFile.XmlDocument.CreateAttribute(p_strPropertyName);
                    objXmlAttribute.Value = value;
                    m_objXmlNode.Attributes.Append(objXmlAttribute);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        XmlElement objXmlElement = (XmlElement)m_objXmlNode;
                        objXmlElement.RemoveAttribute(p_strPropertyName);
                    }
                    else
                        m_objXmlNode.Attributes[p_strPropertyName]!.Value = value;
                }
            }
        }

        protected string GetXmlCData()
        {
            if ((m_objXmlNode != null)
                && (m_objXmlNode.ChildNodes.Count > 0)
                && (m_objXmlNode.ChildNodes[0] is XmlCDataSection)
                )
            {
                XmlCDataSection? objCDataSection = m_objXmlNode.ChildNodes[0] as XmlCDataSection;
                if (objCDataSection != null)
                {
                    string? strValue = objCDataSection.Value;
                    if (strValue != null)
                        return strValue;
                }
            }

            return "";
        }

        public string Name
        {
            get { return GetXmlAttribute(nameof(Name)); }
            set { SetXmlAttribute(nameof(Name), value); }
        }

        public string SecurityGroup
        {
            get { return GetXmlAttribute(nameof(SecurityGroup)); }
            set { SetXmlAttribute(nameof(SecurityGroup), value); }
        }

        public bool Accessible
        {
            get { return MemberOf(SecurityGroup); }
        }

        public string CloudAccount
        {
            get { return GetXmlAttribute(nameof(CloudAccount)); }
            set { SetXmlAttribute(nameof(CloudAccount), value); }
        }

        public string Domain
        {
            get { return GetXmlAttribute(nameof(Domain)); }
            set { SetXmlAttribute(nameof(Domain), value); }
        }

        public string Subnet
        {
            get { return GetXmlAttribute(nameof(Subnet)); }
            set { SetXmlAttribute(nameof(Subnet), value); }
        }

        public string Hostname
        {
            get { return GetXmlAttribute(nameof(Hostname)); }
            set { SetXmlAttribute(nameof(Hostname), value); }
        }

        private static string GetCloudAccountName()
        {
            string strAccountName = "Local";

            try
            {
                if (RoleEnvironment.IsAvailable)
                    strAccountName = RoleEnvironment.CurrentRoleInstance.Id;
                /*
                else
                {
                    AmazonS3Client objAmazonS3Client = new AmazonS3Client();
                    strAccountName = objAmazonS3Client.Config.RegionEndpoint);
                }
                */
            }
            catch (Exception)
            {
            }

            return strAccountName;
        }

        private static string[] GetAllLocalIPv4(NetworkInterfaceType p_objNetworkInterfaceType)
        {
            List<string> arrAddresses = new List<string>();

            foreach (NetworkInterface objNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((objNetworkInterface.NetworkInterfaceType == p_objNetworkInterfaceType) && (objNetworkInterface.OperationalStatus == OperationalStatus.Up))
                {
                    foreach (UnicastIPAddressInformation objUnicastIPAddressInformation in objNetworkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (objUnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            arrAddresses.Add(objUnicastIPAddressInformation.Address.ToString());
                        }
                    }
                }
            }

            return arrAddresses.ToArray();
        }

        private bool Matches(string p_strPattern, string[] p_strValues)
        {
            bool blnResult = false;

            foreach (string strValue in p_strValues)
                blnResult = blnResult || Matches(p_strPattern, strValue);

            return blnResult;
        }

        private bool Matches(string p_strPattern, string p_strValue)
        {
            bool blnResult = true;

            if (!string.IsNullOrWhiteSpace(p_strPattern))
            {
                if (p_strPattern == p_strValue)
                    blnResult = true;
                else
                {
                    try
                    {
                        Regex objRegex = new Regex(p_strPattern, RegexOptions.None);

                        // Assume it is a Regex and try to match first
                        blnResult = objRegex.IsMatch(p_strValue);
                    }
                    catch (ArgumentException)
                    {
                        blnResult = false;
                    }
                }
            }

            return blnResult;
        }

        public bool Visible
        {
            get
            {
                return Matches(CloudAccount, GetCloudAccountName())
                    && Matches(Domain, System.Environment.UserDomainName)
                    && Matches(Hostname, GetAllLocalIPv4(NetworkInterfaceType.Ethernet))
                    && Matches(Hostname, System.Environment.MachineName)
                    ;
            }
        }

        public virtual DataModelBase[] Items
        {
            get { return []; }
        }

        public int CompareTo(object? p_objObject)
        {
            if (p_objObject != null
                && p_objObject.GetType() == typeof(DataModelBase)
                )
            {
                DataModelBase objDataModelBase = (DataModelBase)p_objObject;
                return Name.CompareTo(objDataModelBase.Name);
            }

            throw new ArgumentException("p_objObject is not a ConfigNode");
        }

        public bool MemberOf(string p_strSecurityGroup)
        {
            List<string> objGroups = [];

            WindowsIdentity objWindowsIdentity = WindowsIdentity.GetCurrent();
            if (objWindowsIdentity.Groups != null)
            {
                foreach (var group in objWindowsIdentity.Groups)
                {
                    try
                    {
                        objGroups.Add(group.Translate(typeof(NTAccount)).ToString());
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                return objGroups.Contains(p_strSecurityGroup);
            }

            return false;
        }
    }
}
