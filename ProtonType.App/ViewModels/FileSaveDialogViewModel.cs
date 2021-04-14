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
using System.Windows.Media.Imaging;
using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.App.ViewModels
{

    public class FileSaveData

    {
        internal readonly IModuleFileSave ModuleFileSave;
        internal IFileViewModel FileViewModel;


        public string FileName { get; private set; }
        public string ModuleName { get; set; }

        public FileSaveData(IModuleFileNew fileCreateModule, FileExtension fileExtension)
        {
        }

        public FileSaveData(IModuleFileSave moduleFileSave, IFileViewModel fileViewModel)
        {
            this.ModuleFileSave = moduleFileSave;
            this.FileViewModel = fileViewModel;

            this.ModuleName = ModuleFileSave.GetType().Name;
            this.FileName = Path.GetFileName(fileViewModel.Filename);
        }
    }

    public class FileSaveDialogViewModel : DialogViewModel
    {
        private readonly MainViewModel _mainViewModel;
        
        private string _firstName;
        private string _lastName;
        
       

        internal List<FileSaveData> _fileSaveList = new List<FileSaveData>();

        public List<FileSaveData> FileSaveList { get { return _fileSaveList; } }


        public FileSaveDialogViewModel(MainViewModel mainViewModel) : base("Save")
        {
            _mainViewModel = mainViewModel;

            IconSource = CreateImageSource(@"/ProtonType.Editor;component/icons/SaveDocument.png");
            IconSource.Freeze();

            // create list of file extensions
            var fileDocumentsMgr = mainViewModel.Model.FileDocumentsMgr;
            foreach (var fileSaveModule in fileDocumentsMgr.FileSaveModules)
            {
                foreach (var fileViewModel in fileSaveModule.FileViewModels)
                {
                    var fileSaveList = new FileSaveData(fileSaveModule, fileViewModel);
                    _fileSaveList.Add(fileSaveList);
                }
            }

            
        }

        private static BitmapImage CreateImageSource(string resourcePath)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(resourcePath, UriKind.Relative);
            bmp.EndInit();
            return bmp;
        }

        FileSaveData _selectedValue;
        public FileSaveData SelectedValue
        {
            get { return _selectedValue; }
            set 
            {
                if (value != _selectedValue)
                {
                    _selectedValue = value;
                    RaisePropertyChanged(() => SelectedValue);

                    OnSelection(value);
                }
            }
        }

        private void OnSelection(FileSaveData fileSaveData)
        {
            // Hide dialog
            _mainViewModel.DialogContent = null;
            
            _mainViewModel.MenuFileSave2(fileSaveData);
        }

    }
}
