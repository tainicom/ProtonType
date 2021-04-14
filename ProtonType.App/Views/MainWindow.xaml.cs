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
using System.ComponentModel;
using System.IO;
using System.Windows;
using tainicom.ProtonType.App.ViewModels;

namespace tainicom.ProtonType.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region menu events
        private void MenuFileNew_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MenuFileNew();
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MenuFileOpen();
        }

        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MenuFileSave();
        }

        private void MenuFileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.MenuFileSaveAs();
        }

        private void RecentFileList_MenuClick(object sender, Common.RecentFileList.MenuClickEventArgs e)
        {
            mainViewModel.FileOpen(e.Filepath);
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        #endregion
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainViewModel = new MainViewModel();
            mainViewModel.Initialize(this);
            this.DataContext = mainViewModel;
            

            //load command line stage file
            String[] cmds = Environment.GetCommandLineArgs();
            
            if (cmds.Length == 1) // no command line arguments
            {
                //mainViewModel.FileNew();
                mainViewModel.OpenWelcome();
            }
            else 
            {
                FileInfo fi = new FileInfo(cmds[1]);
                if (fi.FullName != string.Empty)
                {
                    try
                    {
                        mainViewModel.FileOpen(fi.FullName);
                    }
                    catch (DirectoryNotFoundException dnfex) { MessageBox.Show("filename: " + fi.FullName, "File not found"); }
                    catch (FileNotFoundException fnfex) { MessageBox.Show("filename: " + fi.FullName, "File not found"); }
                    catch (Exception ex) { MessageBox.Show("filename: " + fi.FullName +"/r/n" + ex.Message, "File load failed"); }
                }
            }
            
            return;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.mainViewModel.Dispose();
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            mainViewModel.Window_DragOver(e);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            mainViewModel.Window_Drop(e);
        }

    }
}
