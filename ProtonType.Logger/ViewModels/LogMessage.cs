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
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using tainicom.ProtonType.Framework.Modules;

namespace tainicom.ProtonType.Logger.ViewModels
{
    public class Message
    {
        static ImageSource _errorImageSource;
        static ImageSource _warningImageSource;
        static ImageSource _messageImageSource;

        [Flags]
        public enum MessageType
        {
            None    = 0,
            Error   = 1,
            Warning = 2,
            Message = 4,
            Output  = 8,
            All     = int.MaxValue
        }

        public MessageType Type { get; private set; }
        public IModule Source { get; private set; }
        public string ErrorCode { get; private set; }
        public string Description { get; private set; }
        public string Filename { get; private set; }
        public string FragmentIdentifier { get; private set; }

        public ImageSource ImageSource
        {
            get
            {
                switch(Type)
                {
                    case MessageType.Error:
                        if (Message._errorImageSource == null)
                            Message._errorImageSource = LoggerModule.CreateImageSource(@"Icons/StatusCriticalError_12x.png");
                        return Message._errorImageSource;
                    case MessageType.Warning:
                        if (Message._warningImageSource == null)
                            Message._warningImageSource = LoggerModule.CreateImageSource(@"Icons/StatusWarning_12x.png");
                        return Message._warningImageSource;
                    case MessageType.Message:
                        if (Message._messageImageSource == null)
                            Message._messageImageSource = LoggerModule.CreateImageSource(@"Icons/StatusInformation_12x.png");
                        return Message._messageImageSource;
                    default:
                        return null;
                }
            }
        }

        public Message(MessageType messageType, IModule source, string errorCode, string description, string filename, string fragmentIdentifier)
        {
            this.Type = messageType;
            this.Source = source;
            this.ErrorCode = errorCode;
            this.Description = description;
            this.Filename = filename;
            this.FragmentIdentifier = fragmentIdentifier;
        }



    }
}
