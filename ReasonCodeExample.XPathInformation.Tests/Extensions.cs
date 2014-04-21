﻿using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Tests
{
    internal static class Extensions
    {
        public static XObject SelectSingleNode(this string xml, string xpath)
        {
            var document = XDocument.Parse(xml);
            var enumerator = ((IEnumerable)document.Root.XPathEvaluate(xpath, new SimpleXmlNamespaceResolver(document))).GetEnumerator();
            enumerator.MoveNext();
            return (XObject)enumerator.Current;
        }
    }
}