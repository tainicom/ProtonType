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
using System.Diagnostics;
using System.IO;
using System.Windows;
using tainicom.ProtonType.App.CommandMgr;
using tainicom.ProtonType.App.Models;
using tainicom.ProtonType.App.Views;
using tainicom.ProtonType.Framework.Commands;
using tainicom.ProtonType.Framework.Helpers;
using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;
using Win32 = Microsoft.Win32;

namespace tainicom.ProtonType.App.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private MainModel model;

        
        internal MainModel Model { get { return model; } }

        internal ISiteViewModel Site { get { return model.ModulesMgr.Site; } }
        internal ICommandController Controller { get { return model.Controller; } }  
        internal IEnumerable<TModule> GetModules<TModule>() where TModule : class , IModule
        {
            return model.ModulesMgr.GetModules<TModule>();
        }
        


        //file commands


        // undo/redo command
        public RelayCommand UndoCommand { get; set; }
        public RelayCommand RedoCommand { get; set; }

        // TODO: get title from active Pane/Model
        private string _title = "ProtonType";
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register("DialogContent", typeof(FrameworkElement), typeof(MainViewModel));
        public FrameworkElement DialogContent
        {
            get { return (FrameworkElement)GetValue(DialogContentProperty); }
            set { SetValue(DialogContentProperty, value); }
        }
        

        internal MainViewModel()
        {
        }

        internal void Initialize(MainWindow mainWindow)
        {
            this.model = new MainModel();
           
            InitializeMenus(mainWindow);
            InitializePanels(mainWindow);
            
            
            model.ModulesMgr.Initialize(this);

            CommandController controller = (CommandController)this.Controller;
            UndoCommand = new RelayCommand(par => controller.UndoCommand(), canEx => controller.UndoCount > 0);
            RedoCommand = new RelayCommand(par => controller.ExecuteCommand(), canEx => controller.RedoCount > 0);
            
            return;
        }
        
        internal void OpenWelcome()
        {
            var welcomeViewModel = new WelcomeViewModel(this);
            Controller.EnqueueAndExecute(new AddPaneCmd(this.Site, welcomeViewModel));
        }

        internal void MenuFileNew()
        {
            var dataContext = new FileNewDialogViewModel(this);
            DialogContent = new FileNewDialogView { DataContext = dataContext };
        }


        internal void MenuFileNew2(FileNewData fileNewData)
        {
            IModuleFileNew moduleFile = null;

            moduleFile = fileNewData.FileCreateModule;
            
            if (moduleFile == null) return;

            if (((IModuleFileProject)moduleFile).SpawnNewMainWindow)
            {
                // get executable path
                String[] cmds = Environment.GetCommandLineArgs();
                FileInfo fi = new FileInfo(cmds[0]);
                string exefilename = cmds[0];
                exefilename = exefilename.Replace(".vshost.", ".");

                // spawn a new process
                string docfilename = "";
                ProcessStartInfo pinfo = new ProcessStartInfo(exefilename, docfilename);
                System.Diagnostics.Process.Start(pinfo);
                return;
            }

            var fileViewModel = moduleFile.FileNew();

            return;
        }

        internal void MenuFileOpen()
        {
            string filename = String.Empty;
            var initialDirectory = String.Empty;

            if (Model.RecentFilesMgr.RecentFiles.Count > 0)
            {
                var recentFile = Model.RecentFilesMgr.RecentFiles[0];
                filename = recentFile.Filename;
                initialDirectory = recentFile.FullPath;
            }

            // build filter
            List<FileExtension> fileExtensions = Model.FileDocumentsMgr.GetSupportedFileExtensions();

            string ext = Path.GetExtension(filename);
            fileExtensions.Sort((a, b) =>
            {
                // bring extension that match 'ext' on top
                if (a.Extension.Equals(ext)) return -1;
                else if (b.Extension.Equals(ext)) return +1;
                // sort by Description
                return a.Description.CompareTo(b.Description);
            });

            List<string> extensions = new List<string>();
            foreach (var fileExtension in fileExtensions)
                extensions.Add(fileExtension.ToString());

            extensions.Add("All files (*.*)|*.*");
            string filter = string.Join("|", extensions);

            // open file dialog
            Win32.OpenFileDialog ofd = new Win32.OpenFileDialog();
            ofd.Filter = string.Join("|", extensions);
            ofd.FilterIndex = 1;
            ofd.AddExtension = true;
            ofd.FileName = filename;
            ofd.InitialDirectory = initialDirectory;
            if (ofd.ShowDialog() != true)
                return;
            var fdFilenameResult = ofd.FileName;

            FileOpen(fdFilenameResult);
        }

        internal void MenuFileSave()
        {
            var dataContext = new FileSaveDialogViewModel(this);
            DialogContent = new FileSaveDialogView { DataContext = dataContext };
        }

        internal void MenuFileSave2(FileSaveData fileSaveData)
        {
            IFileViewModel fileViewModel = fileSaveData.FileViewModel;
            var moduleFileSave = fileSaveData.ModuleFileSave;

            if (fileViewModel.Filename == null || !System.IO.Path.IsPathRooted(fileViewModel.Filename))
            {
                MenuFileSaveAs();
            }
            else
            {
                moduleFileSave.FileSave(fileViewModel);
            }
        }

        internal void MenuFileSaveAs()
        {
            var dataContext = new FileSaveAsDialogViewModel(this);
            DialogContent = new FileSaveAsDialogView { DataContext = dataContext };
        }

        internal void MenuFileSaveAs2(FileSaveAsData fileSaveAsData)
        {
            IFileViewModel fileViewModel = fileSaveAsData.FileViewModel;
            var moduleFileSave = fileSaveAsData.ModuleFileSave;

            string filename = Path.GetFileName(fileViewModel.Filename);
            var initialDirectory = Path.GetDirectoryName(fileViewModel.Filename);

            // Get active moduleFile
            var moduleFile = moduleFileSave as IModuleFileSaveAs;
            IEnumerable<FileExtension> fileExtensions = moduleFile.FileExtensions;

            List<string> extensions = new List<string>();
            foreach (var fileExtension in fileExtensions)
                extensions.Add(fileExtension.ToString());

            extensions.Add("All files (*.*)|*.*");
            string filter = string.Join("|", extensions);
            
            Win32.SaveFileDialog sfd = new Win32.SaveFileDialog();
            sfd.Filter = filter;
            sfd.FilterIndex = 1;
            sfd.AddExtension = true;
            sfd.FileName = filename;
            sfd.InitialDirectory = initialDirectory;
            if (sfd.ShowDialog() != true)
                return;
            var fdFilenameResult = sfd.FileName;

            moduleFile.FileSaveAs(fileViewModel, fdFilenameResult);
            RegisterRecentFile(fdFilenameResult);

            return;
        }

        internal bool FileOpen(string filepath)
        {
            string ext = Path.GetExtension(filepath).ToLower();
            string baseDirectory = Path.GetDirectoryName(filepath);

            // find file module
            List<IModuleFile> modules = Model.FileDocumentsMgr.QueryFileModulesForFile(filepath);
            System.Diagnostics.Debug.Assert(modules.Count<=1, "More than one modules found for "+ Path.GetExtension(filepath));
            IModuleFile moduleFile = modules[0];

            if (moduleFile is IModuleFileProject)
            {
                if (((IModuleFileProject)moduleFile).SpawnNewMainWindow)
                {
                    // get executable path
                    String[] cmds = Environment.GetCommandLineArgs();
                    FileInfo fi = new FileInfo(cmds[0]);
                    string exefilename = cmds[0];
                    exefilename = exefilename.Replace(".vshost.", ".");

                    // spawn a new process
                    string docfilename = "\"" + filepath + "\"";
                    ProcessStartInfo pinfo = new ProcessStartInfo(exefilename, docfilename);
                    System.Diagnostics.Process.Start(pinfo);
                    return false;
                }

                // load modules for the ProjectFile
                this.model.ModulesMgr.LoadDocumentModules(baseDirectory);
            }
            
            var fileViewModel = moduleFile.FileOpen(filepath);
            RegisterRecentFile(filepath);

            return true;
        }
        
        internal void RegisterRecentFile(string filepath)
        {
            try
            {
                Model.RecentFilesMgr.InsertFile(filepath);
                System.Windows.Shell.JumpList.AddToRecentCategory(filepath);
                System.Windows.Shell.JumpList.GetJumpList(App.Current).Apply();
            }
            catch (ArgumentException aex) { }
        }
                
    }

}
