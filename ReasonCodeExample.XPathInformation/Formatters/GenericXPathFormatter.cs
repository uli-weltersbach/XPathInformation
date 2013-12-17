﻿using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class GenericXPathFormatter : IXPathFormatter
    {
        public virtual string Format(XObject obj)
        {
            XAttribute attribute = obj as XAttribute;
            if (obj is XAttribute)
            {
                string elementPath = Format(attribute.Parent);
                string attributePath = Format(attribute);
                return string.IsNullOrEmpty(elementPath) ? string.Empty : elementPath + attributePath;
            }
            if (obj is XElement)
            {
                return Format(obj as XElement);
            }
            return string.Empty;
        }

        public virtual string Format(XElement element)
        {
            if (element == null)
                return string.Empty;
            return GetElementXPath(element);
        }

        protected virtual string GetElementXPath(XElement element)
        {
            return element.AncestorsAndSelf().Reverse().Select(GetElementName).Aggregate(string.Empty, ConcatenateXPath);
        }

        protected virtual string GetElementName(XElement element)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
                return element.Name.LocalName;

            string namespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return string.Format("*[local-name()='{0}' and namespace-uri()='{1}']", element.Name.LocalName, element.Name.NamespaceName);

            return namespacePrefix + ":" + element.Name.LocalName;
        }

        protected virtual string ConcatenateXPath(string current, string next)
        {
            return current + "/" + next;
        }

        public virtual string Format(XAttribute attribute)
        {
            string name = GetAttributeName(attribute);
            string value = GetAttributeValue(attribute);
            return string.Format("[{0}='{1}']", name, value);
        }

        protected virtual string GetAttributeName(XAttribute attribute)
        {
            if (attribute == null)
                return string.Empty;

            if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                return string.Format("@{0}", attribute.Name.LocalName);

            if (attribute.Parent == null)
                throw new XmlException(string.Format("Unable to determine namespace prefix for attribute \"{0}\". Parent is null.", attribute.Name));

            string namespacePrefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return string.Format("@{0}", attribute.Name.LocalName);

            return string.Format("@{0}:{1}", namespacePrefix, attribute.Name.LocalName);
        }

        protected virtual string GetAttributeValue(XAttribute attribute)
        {
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}