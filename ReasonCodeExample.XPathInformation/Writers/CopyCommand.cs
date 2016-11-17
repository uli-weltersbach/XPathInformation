﻿using System;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal abstract class CopyCommand
    {
        protected CopyCommand(int id, XmlRepository repository, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
        {
            Repository = repository;
            Command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, new CommandID(Guid.Parse(Symbols.PackageID), id));
            WriterProvider = writerProvider;
            TextFormatter = textFormatter;
        }

        protected XmlRepository Repository
        {
            get;
            private set;
        }

        protected OleMenuCommand Command
        {
            get;
            private set;
        }

        protected Func<IWriter> WriterProvider
        {
            get;
            private set;
        }

        protected ICommandTextFormatter TextFormatter
        {
            get;
            private set;
        }

        protected string Output
        {
            get;
            set;
        }

        private void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(Output);
        }

        protected abstract void OnBeforeQueryStatus(object sender, EventArgs e);

        public static implicit operator OleMenuCommand(CopyCommand command)
        {
            return command.Command;
        }
    }
}