﻿using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class XPathWriterTests
    {
        [TestCase("<a><b><c d=\"value\" /></b></a>", "/a/b/c/@d")]
        public void AttributePath(string xml, string expectedXPath)
        {
            // Arrange
            XDocument document = XDocument.Parse(xml);
            IEnumerator enumerator = ((IEnumerable) document.Root.XPathEvaluate(expectedXPath)).GetEnumerator();
            enumerator.MoveNext();
            var testNode = enumerator.Current as XAttribute;
            INodeFilter includeAll = Substitute.For<INodeFilter>();
            includeAll.IsIncluded(Arg.Any<XObject>()).Returns(true);

            // Act
            string actualXPath = new XPathWriter(new[]{includeAll}).Write(testNode);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}