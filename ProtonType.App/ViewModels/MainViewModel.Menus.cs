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

using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using tainicom.ProtonType.App.Views;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.App.ViewModels
{
    public partial class MainViewModel
    {
        Menu _windowMenu;
        ToolBarTray _toolBarTray;
        StatusBar _statusBar;

        private void InitializeMenus(MainWindow mainWindow)
        {
            _windowMenu = mainWindow.menu;
            _toolBarTray = mainWindow.toolBarTray;
            _statusBar = mainWindow.statusBar;

            Model.ModulesMgr.MenuGroups.CollectionChanged += MenuGroups_CollectionChanged;
            Model.ModulesMgr.ToolbarGroups.CollectionChanged += ToolbarGroups_CollectionChanged;
            Model.ModulesMgr.StatusbarGroups.CollectionChanged += StatusbarGroups_CollectionChanged;
        }
        
        private void MenuGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems)
                    {
                        MenuViewModel menuViewModel = newItem as MenuViewModel;
                        AddMenu(menuViewModel);
                    }
                    break;
            }
        }
        void ToolbarGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems)
                    {
                        ToolbarViewModel toolbarViewModel = newItem as ToolbarViewModel;
                        AddToolbar(toolbarViewModel);
                    }
                    break;
            }
        }
        void StatusbarGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems)
                    {
                        StatusBarItemViewModel statusBarItemViewModel = newItem as StatusBarItemViewModel;
                        AddStatusBarItem(statusBarItemViewModel);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var newItem in e.OldItems)
                    {
                        StatusBarItemViewModel statusBarItemViewModel = newItem as StatusBarItemViewModel;
                        RemoveStatusBarItem(statusBarItemViewModel);
                    }
                    break;
            }
        }

        private void AddMenu(MenuViewModel menuViewModel)
        {
            string path = menuViewModel.Path;

            string[] items = path.Split(new char[] { '/', '|' });

            ItemsControl parentMenu = _windowMenu;

            foreach (string name in items)
            {
                ItemsControl ic = FindName(parentMenu, name);
                if (ic == null)
                {
                    MenuItem mi = new MenuItem();
                    mi.Header = name;
                    mi.Tag = menuViewModel;
                    parentMenu.Items.Add(mi);
                    ic = mi;
                }
                parentMenu = ic;
            }

            //finally add the menuitem
            MenuItem mitem = new MenuItem();
            mitem.DataContext = menuViewModel;
            mitem.Header = menuViewModel.Header;
            mitem.Icon = (menuViewModel.IconSource == null)
                ? null
                : new Image() { SnapsToDevicePixels = true, Source = menuViewModel.IconSource };
            mitem.Click += mitem_Click;
            parentMenu.Items.Add(mitem);

            return;
        }

        private static ItemsControl FindName(ItemsControl parentMenu, string name)
        {
            ItemsControl ic = (ItemsControl)parentMenu.FindName("" + name);
            if (ic == null)
            {
                foreach (var item in parentMenu.Items)
                {
                    var menuItem = item as MenuItem;
                    if (menuItem !=null)
                    {
                        if (menuItem.Header.ToString() == name)
                        {
                            if (menuItem.Tag is MenuViewModel)
                                return menuItem as ItemsControl;
                        }   
                    }
                }
            }

            return ic;
        }
        
        void mitem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var menu = (MenuItem)sender;
            var menuViewModel = (MenuViewModel)menu.DataContext;
            menuViewModel.ClickHandler(menuViewModel);
        }

        private void AddToolbar(ToolbarViewModel toolbarViewModel)
        {
            var toolbar = new ToolBar();
            toolbar.DataContext = toolbarViewModel;
            toolbar.ToolTip = toolbarViewModel.Name;
            toolbar.ItemsSource = toolbarViewModel.ControlItems;

            _toolBarTray.ToolBars.Add(toolbar);
        }

        private void AddStatusBarItem(StatusBarItemViewModel statusBarItemViewModel)
        {
            var statusbaritem = new System.Windows.Controls.Primitives.StatusBarItem();

            // style
            statusbaritem.Background = new SolidColorBrush(Color.FromRgb(0x18, 0x18, 0x18));
            statusbaritem.FontFamily = new FontFamily("Segoe UI Mono");
            statusbaritem.FontSize = 12;
            statusbaritem.Foreground = new SolidColorBrush(Color.FromRgb(0x82, 0x87, 0x90));
            statusbaritem.BorderThickness = new System.Windows.Thickness(1);
            statusbaritem.BorderBrush = new SolidColorBrush(Color.FromRgb(0x82, 0x87, 0x90));
            statusbaritem.Margin = new System.Windows.Thickness(2);
            //statusbaritem.Width = // auto
            statusbaritem.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            statusbaritem.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            statusbaritem.Padding = new System.Windows.Thickness(5, 1, 5, 1);

            statusbaritem.DataContext = statusBarItemViewModel;
            //statusbaritem.Name = statusBarItemViewModel.Name;
            statusbaritem.ToolTip = statusBarItemViewModel.Name;
            //statusbaritem.ItemsSource = toolbarViewModel.ControlItems;
            statusbaritem.Content = statusBarItemViewModel.Content;

            _statusBar.Items.Add(statusbaritem);
        }

        private void RemoveStatusBarItem(StatusBarItemViewModel statusBarItemViewModel)
        {
            for (int i = _statusBar.Items.Count-1; i > 0; i--)
            {
                StatusBarItem statusBarItem = (StatusBarItem)_statusBar.Items[i];
                if (statusBarItem.DataContext == statusBarItemViewModel)                    
                    _statusBar.Items.RemoveAt(i);
            }
        }
    }
}
