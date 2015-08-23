#region License
//   Copyright 2019 Kastellanos Nikolaos
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
using tainicom.ProtonType.Framework.Commands;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.Framework.Modules
{
    public interface IModuleFile : IModule
    {
        // a list of supported file types
        IEnumerable<FileExtension> FileExtensions { get; }
        
        IFileViewModel FileOpen(string filepath);

        void FileClose(IFileViewModel fileViewModel);

        IEnumerable<IFileViewModel> FileViewModels { get; }
    }
    
    public struct FileExtension
    {
        public readonly string Extension;
        public readonly string Description;

        public FileExtension(string extension, string description)
        {
            this.Extension = extension;
            this.Description = description;
        }

        public override string ToString()
        {
            return string.Format("{0} files (*{1})|*{1}", Description, Extension);
        }

        public bool MatchExtension(string ext)
        {
            return ext.Equals(Extension, StringComparison.OrdinalIgnoreCase);
        }
    }

    public interface IModuleFileNew : IModuleFile
    {
        IFileViewModel FileNew();
    }
    
    public interface IModuleFileSave : IModuleFile
    {
        void FileSave(IFileViewModel fileViewModel);
    }

    public interface IModuleFileSaveAs : IModuleFile
    {
        void FileSaveAs(IFileViewModel fileViewModel, string filename);
    }

    public interface IFileViewModel
    {
        string Filename { get; }
    }
    
    public interface IModuleFileProject : IModuleFile
    {
        bool SpawnNewMainWindow { get; }
    }
}
