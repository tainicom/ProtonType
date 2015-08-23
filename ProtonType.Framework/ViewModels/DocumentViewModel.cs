#region License
//   Copyright 2015 Kastellanos Nikolaos
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
using ProtonType.Framework.Helpers;
using System.Windows.Input;

namespace ProtonType.Framework.ViewModels
{
    public class DocumentViewModel : PaneViewModel
    {
        protected readonly object Model;
        public string Name { get; private set; }

        public DocumentViewModel(object model, string name)
        {
            this.Model = model;
            Name = name;
            Title = name;
        }

        #region CloseCommand
        RelayCommand _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand((p) => OnClose(), (p) => CanClose());
                }
                return _closeCommand;
            }
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        protected virtual void OnClose()
        {
        }
        #endregion
        
    }
}
