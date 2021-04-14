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
using tainicom.ProtonType.App.Models;
using tainicom.ProtonType.Framework.Modules;

namespace tainicom.ProtonType.App.Modules
{
    class FileDocumentsMgr
    {    
        private Models.MainModel _mainModel;
        
        public FileDocumentsMgr(MainModel mainModel)
        {
            this._mainModel = mainModel;
        }
        
        internal IEnumerable<IModuleFile> FileModules 
        { 
            get { return _mainModel.ModulesMgr.GetModules<IModuleFile>(); } 
        }

        internal IEnumerable<IModuleFileNew> FileCreateModules
        {
            get { return _mainModel.ModulesMgr.GetModules<IModuleFileNew>(); }
        }

        internal IEnumerable<IModuleFileSave> FileSaveModules
        {
            get { return _mainModel.ModulesMgr.GetModules<IModuleFileSave>(); }
        }
        
        internal List<IModuleFile> QueryFileModulesForFile(string filepath)
        {
            string ext = Path.GetExtension(filepath);
            List<IModuleFile> modules = new List<IModuleFile>();
            foreach (var fileModule in FileModules)
            {
                foreach (var fileExtension in fileModule.FileExtensions)
                {
                    if (fileExtension.MatchExtension(ext))
                        modules.Add(fileModule);
                }
            }
            return modules;
        }
        
        internal List<FileExtension> GetSupportedFileExtensions()
        {
            List<FileExtension> fileExtensions = new List<FileExtension>();
            foreach (var fileModule in FileModules)
            {
                foreach (var fileExtension in fileModule.FileExtensions)
                    fileExtensions.Add(fileExtension);
            }

            return fileExtensions;
        }
    }
}
