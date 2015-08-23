#region License
//   Copyright 2019 Kastellanos Nikolaos
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
using System.Windows.Controls;
using System.Windows.Media;

namespace tainicom.ProtonType.Framework.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private string _header = null;
        private string _path = null;
        private bool _isActive = false;
        private ImageSource _iconSource = null;

        public readonly object UserData;

        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    RaisePropertyChanged(() => Header);                    
                }
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RaisePropertyChanged(() => Path);
                }
            }
        }

        public ImageSource IconSource
        {
            get { return _iconSource; }
            set
            {
                if (_iconSource != value)
                {
                    _iconSource = value;
                    RaisePropertyChanged(() => IconSource);
                }
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged(() => IsActive);
                }
            }
        }


        public MenuViewModel(string header, string path, object userData = null)
        {
            this.Header = header;
            this.Path = path;
            this.UserData = userData;
        }

        public MenuViewModel(string header, string path, ImageSource iconSource, object userData = null)
        {
            this.Header = header;
            this.Path = path;
            this.IconSource = iconSource;
            this.UserData = userData;
        }
        

        public delegate void ClickEventHandler(MenuViewModel sender);
        public ClickEventHandler ClickHandler { get; set; }
        
    }
}
