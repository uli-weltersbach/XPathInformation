﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class AttributeFilter : IAttributeFilter
    {
        private readonly IEnumerable<XPathSetting> _settings;

        public AttributeFilter(IEnumerable<XPathSetting> settings)
        {
            _settings = settings;
        }

        public bool IsIncluded(XAttribute attribute)
        {
            return _settings.Any(setting => IsMatch(setting, attribute) && IsMatch(setting, attribute.Parent));
        }

        private bool IsMatch(XPathSetting setting, XAttribute attribute)
        {
            if(string.IsNullOrEmpty(setting.AttributeName) && string.IsNullOrEmpty(setting.AttributeNamespace))
            {
                return true;
            }
            if(string.IsNullOrEmpty(setting.AttributeName))
            {
                return attribute.Name.NamespaceName == setting.AttributeNamespace;
            }
            if(string.IsNullOrEmpty(setting.AttributeNamespace))
            {
                return attribute.Name.LocalName == setting.AttributeName;
            }
            return attribute.Name == XName.Get(setting.AttributeName, setting.AttributeNamespace);
        }

        private bool IsMatch(XPathSetting setting, XElement element)
        {
            if(string.IsNullOrEmpty(setting.ElementName) && string.IsNullOrEmpty(setting.ElementNamespace))
            {
                return true;
            }
            if(string.IsNullOrEmpty(setting.ElementName))
            {
                return element.Name.NamespaceName == setting.ElementNamespace;
            }
            if(string.IsNullOrEmpty(setting.ElementNamespace))
            {
                return element.Name.LocalName == setting.ElementName;
            }
            return element.Name == XName.Get(setting.ElementName, setting.ElementNamespace);
        }
    }
}