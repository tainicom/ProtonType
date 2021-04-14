#region License
//   Copyright 2019-2021 Kastellanos Nikolaos
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
using System.Windows;
using System.Windows.Controls;
using tainicom.ProtonType.App.ViewModels;

namespace tainicom.ProtonType.App.Common
{
    public class RecentFileList : Separator
    {
        public int MaxPathLength { get; set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
        /// Default = "_{0}:  {2}"
        /// </summary>
        public string MenuItemFormatOneToNine { get; set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
        /// Default = "{0}:  {2}"
        /// </summary>
        public string MenuItemFormatTenPlus { get; set; }

        public event EventHandler<MenuClickEventArgs> MenuClick;

        Separator _Separator = null;

        public delegate string GetMenuItemTextDelegate(int index, string filepath);
        public GetMenuItemTextDelegate GetMenuItemTextHandler { get; set; }

        public MenuItem FileMenu { get; private set; }
        
        List<RecentFile> _RecentFiles = null;

        //-----------------------------------------------------------------------------------------

        public RecentFileList()
        {
            MaxPathLength = 100;

            MenuItemFormatOneToNine = "_{0}:  {2}";
            MenuItemFormatTenPlus = "{0}:  {2}";

            //RecentFileList2();

            this.DataContextChanged += RecentFileList_DataContextChanged;
            this.Loaded += RecentFileList_Loaded;
        }

        private void RecentFileList_Loaded(object sender, RoutedEventArgs e)
        {
            HookFileMenu();
        }
        private void RecentFileList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadRecentFiles();
        }

        void HookFileMenu()
        {
            MenuItem parent = Parent as MenuItem;
            if (parent == null) throw new ApplicationException("Parent must be a MenuItem");

            if (FileMenu == parent) return;

            if (FileMenu != null) FileMenu.SubmenuOpened -= _FileMenu_SubmenuOpened;

            FileMenu = parent;
            FileMenu.SubmenuOpened += _FileMenu_SubmenuOpened;
        }

        void _FileMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            SetMenuItems();
        }

        void SetMenuItems()
        {
            RemoveMenuItems();

            LoadRecentFiles();

            InsertMenuItems();
        }

        void RemoveMenuItems()
        {
            if (_Separator != null) FileMenu.Items.Remove(_Separator);

            if (_RecentFiles != null)
                foreach (RecentFile r in _RecentFiles)
                    if (r.MenuItem != null)
                        FileMenu.Items.Remove(r.MenuItem);

            _Separator = null;
            _RecentFiles = null;
        }

        void InsertMenuItems()
        {
            if (_RecentFiles == null) return;
            if (_RecentFiles.Count == 0) return;

            int iMenuItem = FileMenu.Items.IndexOf(this);
            foreach (RecentFile r in _RecentFiles)
            {
                string header = GetMenuItemText(r.Number + 1, r.Filepath, r.DisplayPath);

                r.MenuItem = new MenuItem { Header = header };
                r.MenuItem.Click += MenuItem_Click;

                FileMenu.Items.Insert(++iMenuItem, r.MenuItem);
            }

            //_Separator = new Separator();
            //FileMenu.Items.Insert( ++iMenuItem, _Separator );
        }

        string GetMenuItemText(int index, string filepath, string displaypath)
        {
            GetMenuItemTextDelegate delegateGetMenuItemText = GetMenuItemTextHandler;
            if (delegateGetMenuItemText != null) return delegateGetMenuItemText(index, filepath);

            string format = (index < 10 ? MenuItemFormatOneToNine : MenuItemFormatTenPlus);

            string shortPath = ShortenPathname(displaypath, MaxPathLength);

            return String.Format(format, index, filepath, shortPath);
        }




        private class RecentFile
        {
            public int Number = 0;
            public string Filepath = "";
            public MenuItem MenuItem = null;

            public string DisplayPath
            {
                get
                {
                    return Path.Combine(
                        Path.GetDirectoryName(Filepath),
                        Path.GetFileName(Filepath));
                }
            }

            public RecentFile(int number, string filepath)
            {
                this.Number = number;
                this.Filepath = filepath;
            }
        }


        public class MenuClickEventArgs : EventArgs
        {
            public string Filepath { get; private set; }

            public MenuClickEventArgs(string filepath)
            {
                this.Filepath = filepath;
            }
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            OnMenuClick(menuItem);
        }

        protected virtual void OnMenuClick(MenuItem menuItem)
        {
            string filepath = GetFilepath(menuItem);

            if (String.IsNullOrEmpty(filepath)) return;

            EventHandler<MenuClickEventArgs> dMenuClick = MenuClick;
            if (dMenuClick != null) dMenuClick(menuItem, new MenuClickEventArgs(filepath));
        }

        string GetFilepath(MenuItem menuItem)
        {
            foreach (RecentFile r in _RecentFiles)
                if (r.MenuItem == menuItem)
                    return r.Filepath;

            return String.Empty;
        }


        void LoadRecentFiles()
        {   
            _RecentFiles = LoadRecentFilesCore();
        }

        List<RecentFile> LoadRecentFilesCore()
        {
            var mainViewModel = this.DataContext as MainViewModel;
            if (mainViewModel != null)
            {
                List<tainicom.ProtonType.App.FileDocuments.RecentFilesMgr.RecentFileInfo> recentFileInfoList = mainViewModel.Model.RecentFilesMgr.RecentFiles;

                List<RecentFile> files = new List<RecentFile>(recentFileInfoList.Count);

                int i = 0;
                foreach (var recentFileInfo in recentFileInfoList)
                    files.Add(new RecentFile(i++, recentFileInfo.FullFilename));

                return files;
            }
            return new List<RecentFile>();
        }

        //-----------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------

        // This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx

        /// <summary>
        /// Shortens a pathname for display purposes.
        /// </summary>
        /// <param labelName="pathname">The pathname to shorten.</param>
        /// <param labelName="maxLength">The maximum number of characters to be displayed.</param>
        /// <remarks>Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three elipses (...)
        /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
        /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.</para>
        /// <para>This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)</para>
        /// </remarks>
        /// <returns></returns>
        static public string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname.Length <= maxLength)
                return pathname;

            string root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
                root += Path.DirectorySeparatorChar;

            string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int filenameIndex = elements.GetLength(0) - 1;

            if (elements.GetLength(0) == 1) // pathname is just a root and filename
            {
                if (elements[0].Length > 5) // long enough to shorten
                {
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    else
                    {
                        return pathname.Substring(0, maxLength - 3) + "...";
                    }
                }
            }
            else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
            {
                root += "...\\";

                int len = elements[filenameIndex].Length;
                if (len < 6)
                    return root + elements[filenameIndex];

                if ((root.Length + 6) >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }
                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + "...\\" + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathname.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                        totalLength -= elements[--begin].Length - 1;

                    if (totalLength <= maxLength)
                        break;

                    if (end < filenameIndex)
                        totalLength -= elements[++end].Length - 1;

                    if (begin == 0 && end == filenameIndex)
                        break;
                }

                // assemble final string

                for (int i = 0; i < begin; i++)
                {
                    root += elements[i] + '\\';
                }

                root += "...\\";

                for (int i = end; i < filenameIndex; i++)
                {
                    root += elements[i] + '\\';
                }

                return root + elements[filenameIndex];
            }
            return pathname;
        }
    }
}
