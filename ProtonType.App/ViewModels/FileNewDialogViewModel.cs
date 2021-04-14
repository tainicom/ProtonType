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
using System.Windows.Media.Imaging;
using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.App.ViewModels
{

    public class FileNewData

    {
        internal readonly IModuleFileNew FileCreateModule; 
        //internal FileExtension fileExtension;


        public string FileDescription { get; private set; }
        public string FileExtension { get; set; }
        public string ModuleName { get; set; }

        public FileNewData(IModuleFileNew fileCreateModule, FileExtension fileExtension)
        {
            this.FileCreateModule = fileCreateModule;

            this.ModuleName = fileCreateModule.GetType().Name;
            this.FileDescription = fileExtension.Description;
            this.FileExtension = fileExtension.Extension;
        }
    }

    public class FileNewDialogViewModel : DialogViewModel
    {
        private readonly MainViewModel _mainViewModel;
        
        private string _firstName;
        private string _lastName;
        
       

        internal List<FileNewData> _fileNewList = new List<FileNewData>();

        public List<FileNewData> FileNewList { get { return _fileNewList; } }


        public FileNewDialogViewModel(MainViewModel mainViewModel) : base("New")
        {
            _mainViewModel = mainViewModel;

            IconSource = CreateImageSource(@"/ProtonType.Editor;component/icons/NewDocument.png");
            IconSource.Freeze();

            // create list of file extensions
            var fileDocumentsMgr = mainViewModel.Model.FileDocumentsMgr;
            foreach (var fileCreateModule in fileDocumentsMgr.FileCreateModules)
            {
                foreach (var fileExtension in fileCreateModule.FileExtensions)
                {
                    var fileNewList = new FileNewData(fileCreateModule, fileExtension);
                    _fileNewList.Add(fileNewList);
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

        FileNewData _selectedValue;
        public FileNewData SelectedValue
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

        private void OnSelection(FileNewData fileNewData)
        {
            // Hide dialog
            _mainViewModel.DialogContent = null;
            
            _mainViewModel.MenuFileNew2(fileNewData);
        }

    }
}
