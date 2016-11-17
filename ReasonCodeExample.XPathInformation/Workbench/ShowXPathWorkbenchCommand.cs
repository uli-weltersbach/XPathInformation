﻿using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    internal class ShowXPathWorkbenchCommand
    {
        private readonly Package _package;
        private readonly XmlRepository _repository;

        public ShowXPathWorkbenchCommand(Package package, IMenuCommandService commandService, int id, XmlRepository repository)
        {
            _package = package;
            _repository = repository;
            if(commandService == null)
            {
                throw new ArgumentNullException(nameof(commandService));
            }
            var menuCommandID = new CommandID(Guid.Parse(Symbols.PackageID), id);
            var menuCommand = new OleMenuCommand(ShowToolWindow, null, OnBeforeQueryStatus, menuCommandID, PackageResources.ShowXPathWorkbenchCommandText);
            commandService.AddCommand(menuCommand);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var command = (OleMenuCommand)sender;
            if (!_repository.HasContent)
            {
                command.Visible = false;
                return;
            }
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var isXmlDocument = string.Equals(dte?.ActiveDocument?.Language, "XML", StringComparison.InvariantCultureIgnoreCase);
            command.Visible = isXmlDocument;
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            var toolWindowPane = _package.FindToolWindow(typeof(XPathWorkbenchWindow), 0, true);
            if(toolWindowPane?.Frame == null)
            {
                throw new NotSupportedException(string.Format("{0}: Cannot create tool window.", GetType()));
            }
            var windowFrame = (IVsWindowFrame)toolWindowPane.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}