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
using System.Reflection;
using tainicom.ProtonType.App.CommandMgr;
using tainicom.ProtonType.App.FileDocuments;
using tainicom.ProtonType.App.Modules;
using tainicom.ProtonType.Framework.Commands;

namespace tainicom.ProtonType.App.Models
{   
    public partial class MainModel
    {
        private readonly ICommandController _controller;
        private readonly ModulesMgr modulesMgr;
        private readonly FileDocumentsMgr fileDocumentsMgr;
        private readonly RecentFilesMgr recentFilesMgr;

        internal ICommandController Controller { get { return _controller; } }
        internal ModulesMgr ModulesMgr { get { return modulesMgr; } }
        internal FileDocumentsMgr FileDocumentsMgr { get { return fileDocumentsMgr; } }
        internal RecentFilesMgr RecentFilesMgr { get { return recentFilesMgr; } }
        
        
        public MainModel()
        {
            modulesMgr = new ModulesMgr();
            fileDocumentsMgr = new FileDocumentsMgr(this);
            recentFilesMgr = new FileDocuments.RecentFilesMgr();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // controller handles do/undo history 
            // (design pattern: 'Command')
            _controller = new CommandController();
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].FullName == args.Name)
                    return assemblies[i];
                if (assemblies[i].GetName().Name == args.Name)
                    return assemblies[i];
            }

            return null;
        }


    }
}
