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
using System.Text;
using tainicom.ProtonType.Framework.Attributes;
using tainicom.ProtonType.Framework.Helpers;
using tainicom.ProtonType.Framework.ViewModels;
using tainicom.ProtonType.Logger.Views;

namespace tainicom.ProtonType.Logger.ViewModels
{
    [DefaultView(typeof(ErrorListView), "bottomPane")]
    public class ErrorListViewModel : ToolViewModel
    {
        internal readonly LoggerModule Module;

        ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        Message.MessageType _enabledMessageTypes = Message.MessageType.All;


        public bool ShowErrors
        {
            get { return _enabledMessageTypes.HasFlag(Message.MessageType.Error); }
            set
            {
                if (value == ShowErrors) return;
                if (value)
                    _enabledMessageTypes |= Message.MessageType.Error;
                else
                    _enabledMessageTypes &= ~Message.MessageType.Error;
                RaisePropertyChanged(() => ShowErrors);
                RaisePropertyChanged(() => Messages);
            }
        }

        public bool ShowWarnings
        {
            get { return _enabledMessageTypes.HasFlag(Message.MessageType.Warning); }
            set
            {
                if (value == ShowWarnings) return;
                if (value)
                    _enabledMessageTypes |= Message.MessageType.Warning;
                else
                    _enabledMessageTypes &= ~Message.MessageType.Warning;
                RaisePropertyChanged(() => ShowWarnings);
                RaisePropertyChanged(() => Messages);
            }
        }

        public RelayCommand ClearMessagesCommand { get; private set; }
        
        public IEnumerable<Message> Messages
        { 
            get { return _messages.Where((item) => (item.Type & _enabledMessageTypes) != 0); }
        }

        
        public ErrorListViewModel(LoggerModule module, System.Windows.Media.ImageSource iconSource) : base(module, "ErrorList")
        {
            this.Module = module;
            this.IconSource = iconSource;

            ClearMessagesCommand = new RelayCommand(ClearMessages);
        }

        private void ClearMessages(object parameter)
        {
            foreach (var message in _messages.ToArray())            
                _messages.Remove(message);
            RaisePropertyChanged(() => Messages);
        }
        
        internal void LogMessage(Message logMessage)
        {
            Dispatcher.Invoke((Action)(() => 
            {
                _messages.Add(logMessage);
                RaisePropertyChanged(() => Messages);
            }));
            
            System.Diagnostics.Debug.WriteLine(logMessage.Description);
        }
    }
}
