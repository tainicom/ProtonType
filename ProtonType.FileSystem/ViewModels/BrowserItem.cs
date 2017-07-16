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
using System.IO;
using tainicom.ProtonType.Framework.Helpers;

namespace tainicom.ProtonType.ViewModels
{
    public class BrowserItem : NotificationObject, IComparable<BrowserItem>
    {
        public readonly FileBrowser FileBrowser;
        protected string _relativePath;
        
        string _name;
        string _ext;

        public string Testing { get; set; }

        public virtual string Name { get { return _name; } } 
        public virtual string RelativePath { get { return _relativePath; } }

        public virtual IList<BrowserItem> Items { get { return null; } } 
        

        
        internal protected BrowserItem(FileBrowser fileBrowser, string relativePath)
        {
            this.FileBrowser = fileBrowser;
            this._relativePath = relativePath;
        
            this._name = Path.GetFileName(relativePath);
            this._ext = Path.GetExtension(this._name);
        }
        
        internal void Rename(string newFilename)
        {
            this._relativePath = Path.Combine(Path.GetDirectoryName(this._relativePath), newFilename);
            this._name = Path.GetFileName(newFilename);
            this._ext = Path.GetExtension(this._name);
            RaisePropertyChanged(() => Name);
        }
        
        #region IComparable
        public int CompareTo(BrowserItem other)
        {
            int res = 0;
            res = ((other is FolderItem) ? 1 : 0) - ((this is FolderItem) ? 1 : 0);
            if (res == 0) res = _relativePath.CompareTo(other._relativePath);
            if (res == 0) res = _name.CompareTo(other._name);
               
            return res;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{{{0}, Name: {1}}}", GetType().Name, _name);
        }

    }
}
