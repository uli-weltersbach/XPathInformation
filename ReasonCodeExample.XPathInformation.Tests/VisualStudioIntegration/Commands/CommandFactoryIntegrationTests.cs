﻿using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using System;
using System.Windows;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    /// <summary>
    /// These tests have to be run using the Microsoft test runner.
    /// </summary>
    [TestClass]
    public class CommandFactoryIntegrationTests
    {
        private const string VisualStudioHostType = "VS IDE";

        private delegate void ThreadInvokerDelegate();

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanCreateServiceProvider()
        {
            // Act
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;

            // Assert
            Assert.IsNotNull(serviceProvider, "VsIdeTestHostContext.ServiceProvider is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanCreateVisualStudioShell()
        {
            // Arrange
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;

            // Act
            IVsShell vistualStudioShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;

            // Assert
            Assert.IsNotNull(vistualStudioShell, "vistualStudioShell is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanLoadPackage()
        {
            UIThreadInvoker.Invoke(new ThreadInvokerDelegate(LoadPackage));
        }

        private void LoadPackage()
        {
            // Arrange
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;
            IVsShell vistualStudioShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            Guid packageGuid = new Guid(CommandFactory.PackageID);
            IVsPackage package;

            // Act
            int actualLoadPackageResult = vistualStudioShell.LoadPackage(ref packageGuid, out package);

            // Assert
            Assert.AreEqual(VSConstants.S_OK, actualLoadPackageResult, "Load package result wasn't S_OK");
            Assert.IsNotNull(package, "Package failed to load");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanCreateDTE()
        {
            // Act
            DTE dte = VsIdeTestHostContext.Dte;

            // Assert
            Assert.IsNotNull(dte, "VsIdeTestHostContext.Dte is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanRetrieveCommands()
        {
            // Arrange
            DTE dte = VsIdeTestHostContext.Dte;

            // Act
            EnvDTE.Commands commands = dte.Commands;

            // Assert
            Assert.IsNotNull(commands, "VsIdeTestHostContext.Dte.Commands is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanExecuteSaveCommand()
        {
            UIThreadInvoker.Invoke(new ThreadInvokerDelegate(ExecuteSaveCommand));
        }

        private void ExecuteSaveCommand()
        {
            object customIn = null;
            object customOut = null;
            string menuGroupID = new Guid(CommandFactory.MenuGroupID).ToString("B").ToUpper();
            DTE dte = VsIdeTestHostContext.Dte;
            dte.Commands.Raise(menuGroupID, CommandFactory.SaveCommandID, ref customIn, ref customOut);
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void SaveCommandSetsClipboardText()
        {
            UIThreadInvoker.Invoke(new ThreadInvokerDelegate(ClipboardTextIsSet));
        }

        private void ClipboardTextIsSet()
        {
            // Arrange
            Clipboard.Clear();
            string expectedText = Guid.NewGuid().ToString();
            new XPathRepository().Put(expectedText);

            // Act
            ExecuteSaveCommand();
            string actualText = Clipboard.GetText();

            // Assert
            Assert.AreEqual(actualText, expectedText);
        }
    }
}