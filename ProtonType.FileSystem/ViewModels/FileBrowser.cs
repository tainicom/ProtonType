#region License
//   Copyright 2017 Kastellanos Nikolaos
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
using System.IO;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.ViewModels
{
    public partial class FileBrowser : ToolViewModel
    {
        private string _absolutePath;
        FolderItem _rootItem;
        public IList<BrowserItem> Items { get { return _rootItem.Items; } }
        
        ObservableCollection<BrowserItem> _selectedItems = new ObservableCollection<BrowserItem>();
        ReadOnlyObservableCollection<BrowserItem> _readonlySelectedItems = null;
        public ReadOnlyObservableCollection<BrowserItem> SelectedItems
        {
            get
            {
                if (_readonlySelectedItems == null) _readonlySelectedItems = new ReadOnlyObservableCollection<BrowserItem>(this._selectedItems);
                return _readonlySelectedItems;
            }
        }

        public FileBrowser(object model, string name, string absolutePath)
            : base(model, name)
        {
            this._absolutePath = absolutePath;
            _rootItem = this.CreateFolderItem("");            
        }

        public virtual FolderItem CreateFolderItem(string relativePath)
        {
            return new FolderItem(this, relativePath);
        }

        public virtual BrowserItem CreateBrowserItem(string relativePath)
        {
            return new BrowserItem(this, relativePath);
        }

        public virtual string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_absolutePath, relativePath);
        }
        
        #region Events
        public event EventHandler<EventArgs> SelectionChanged;
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            var handler = SelectionChanged;
                if (handler == null) return;
            handler(this, e);
        }
        #endregion
    }
}
