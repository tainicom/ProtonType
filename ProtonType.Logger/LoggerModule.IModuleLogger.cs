#region License
//   Copyright 2020-2021 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using tainicom.ProtonType.Framework.Commands;
using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;
using tainicom.ProtonType.Logger.Contracts;
using tainicom.ProtonType.Logger.ViewModels;

namespace tainicom.ProtonType.Logger
{
    public partial class LoggerModule : IModuleLogger
    {
        OutputViewModel _defaultOutputViewModel = null;
        ErrorListViewModel _defaultErrorListViewModel = null;

        private OutputViewModel GetdefaultOutputViewModel()
        {
            if (_defaultOutputViewModel == null)
            {
                var iconSource = LoggerModule.CreateImageSource(@"Icons/Output_16xLG.png");
                _defaultOutputViewModel = new OutputViewModel(this, iconSource);
            }
            return _defaultOutputViewModel;
        }

        private ErrorListViewModel GetdefaultErrorListViewModel()
        {
            if (_defaultErrorListViewModel == null)
            {   
                var iconSource = LoggerModule.CreateImageSource(@"Icons/BuildErrorList_16x.png");
                _defaultErrorListViewModel = new ErrorListViewModel(this, iconSource);
            }
            return _defaultErrorListViewModel;
        }

        void IModuleLogger.LogError(IModule source, string errorCode, string description, string filename, string fragmentIdentifier)
        {
            var logMessage = new Message(Message.MessageType.Error, source, errorCode, description, filename, fragmentIdentifier);
            GetdefaultErrorListViewModel().LogMessage(logMessage);

            var output = filename;
            if (!string.IsNullOrEmpty(fragmentIdentifier))
                output += "(" + fragmentIdentifier + ")";
            else
                output += " ";
            output += ": error ";
            if (!String.IsNullOrWhiteSpace(errorCode))
                output += errorCode;
            output += ": " + description;

            GetdefaultOutputViewModel().WriteLine(output);
            System.Diagnostics.Debug.WriteLine(output);
            //Console.Error.WriteLine(output);
        }

        void IModuleLogger.LogWarning(IModule source, string errorCode, string description, string filename, string fragmentIdentifier)
        {
            var logMessage = new Message(Message.MessageType.Warning, source, errorCode, description, filename, fragmentIdentifier);
            GetdefaultErrorListViewModel().LogMessage(logMessage);
            
            var output = filename;
            if (!string.IsNullOrEmpty(fragmentIdentifier))
                output += "(" + fragmentIdentifier + ")";
            else
                output += " ";
            output += ": warning ";
            if (!String.IsNullOrWhiteSpace(errorCode))
                output += errorCode;
            output += ": " + description;

            GetdefaultOutputViewModel().WriteLine(output);
            System.Diagnostics.Debug.WriteLine(output);
            //Console.Out.WriteLine(output);
        }

        void IModuleLogger.LogMessage(IModule source, string message)
        {
            var logMessage = new Message(Message.MessageType.Message, source, null, null, null, null);
            //GetdefaultErrorListViewModel().LogMessage(logMessage);

            string output = message;
            GetdefaultOutputViewModel().WriteLine(output);
            System.Diagnostics.Debug.WriteLine(output);
            //Console.Out.WriteLine(output);
        }
    }
}
