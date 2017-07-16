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
using System.Windows;
using System.Windows.Threading;

namespace tainicom.ProtonType.ViewModels
{
    public class FolderItem : BrowserItem
    {
        FileSystemWatcher watcher;

        internal protected FolderItem(FileBrowser fileBrowser, string relativePath)
            : base(fileBrowser, relativePath)
        {
            string absolutePath = this.FileBrowser.GetAbsolutePath(relativePath);
            watcher = new FileSystemWatcher();
            watcher.IncludeSubdirectories = false;
            watcher.Path = absolutePath;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Created);
            watcher.Deleted += new FileSystemEventHandler(watcher_Deleted);
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = true;
        }
        
        ObservableCollection<BrowserItem> _items = null;
        IList<BrowserItem> _readonlyItems = null;
        public override IList<BrowserItem> Items
        {
            get
            {
                if (_items == null) _items = InitItems();
                if (_readonlyItems == null)
                {
                    //_readonlyItems = new MergedReadOnlyBrowserItemObservableCollection<BrowserItem>(_items, null);
                    _readonlyItems = new ReadOnlyObservableCollection<BrowserItem>(_items);
                }
                return _readonlyItems;
            }
        }

        internal protected BrowserItem GetItem(string name)
        {
            if (_items == null) return null; 
            foreach (var item in _items)
            {
               	if (item.Name == name)
                   	return item;
            }
            return null;
        }

        protected virtual ObservableCollection<BrowserItem> InitItems()
        {
            ObservableCollection<BrowserItem> items = new ObservableCollection<BrowserItem>();
            ScanFolder(items, _relativePath);
            return items;
        }

        private void ScanFolder(IList<BrowserItem> items, string relativePath)
        {
            string absolutePath = this.FileBrowser.GetAbsolutePath(relativePath);
            if (!Directory.Exists(absolutePath)) return;
            //add directories
            foreach (string childDirectory in Directory.GetDirectories(absolutePath))
            {
                string directoryName = Path.GetFileName(childDirectory);
                string folderRelativePath = Path.Combine(relativePath, directoryName);
                if (FilterFolder(folderRelativePath))
                    continue;

                BrowserItem item = this.FileBrowser.CreateFolderItem(folderRelativePath);
                items.Add(item);
            }
            //add files
            foreach (string file in Directory.GetFiles(absolutePath))
            {
                string fileName = Path.GetFileName(file);
                string ext = Path.GetExtension(file);
                if (ext.Equals(".mgcontent", StringComparison.OrdinalIgnoreCase))
                    continue;
                string fileRelativePath = Path.Combine(relativePath, fileName);
                BrowserItem item = this.FileBrowser.CreateBrowserItem(fileRelativePath);
                items.Add(item);
            }
        }

        protected virtual bool FilterFolder(string folderRelativePath)
        {
            return false;
        }

        protected void AddItem(BrowserItem item)
        {
            if (_items == null)
                _items = InitItems();

            _items.Add(item);
        }

        protected void RemoveItem(string name)
        {
            var item = GetItem(name);
            if (item == null) return;
            _items.Remove(item);
        }

        protected void RenameItem(string name, string newName)
        {
            var item = GetItem(name);
            if (item == null) return;
            item.Rename(newName);
        }

        #region watcher events
        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                OnWatcherChanged(e);
            })).Wait();
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                OnWatcherCreated(e);
            })).Wait();
        }
        
        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                OnWatcherDeleted(e);
            })).Wait();
        }
        
        void watcher_Renamed(object sender, RenamedEventArgs e)
        {            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    OnWatcherRenamed(e);
                })).Wait();
        }
        #endregion watcher events

        protected virtual void OnWatcherChanged(FileSystemEventArgs e)
        {   
        }

        protected virtual void OnWatcherCreated(FileSystemEventArgs e)
        {         
            if (Directory.Exists(e.FullPath))
            {
                string directoryName = Path.GetFileName(e.FullPath);
                string folderRelativePath = Path.Combine(this._relativePath, directoryName);
                if (!FilterFolder(folderRelativePath))
                {
                    string directoryRelativePath = Path.Combine(_relativePath, directoryName);
                    BrowserItem item = this.FileBrowser.CreateFolderItem(directoryRelativePath);
                    AddItem(item);
                }
            }
            if (File.Exists(e.FullPath))
            {
                string fileName = Path.GetFileName(e.Name);
                string fileRelativePath = Path.Combine(this._relativePath, fileName);
                BrowserItem item = this.FileBrowser.CreateBrowserItem(fileRelativePath);
                AddItem(item);
            }
        }

        protected virtual void OnWatcherDeleted(FileSystemEventArgs e)
        {
            RemoveItem(e.Name);
        }

        protected virtual void OnWatcherRenamed(RenamedEventArgs e)
        {            
            RenameItem(e.OldName, e.Name);
        }

    }
}
