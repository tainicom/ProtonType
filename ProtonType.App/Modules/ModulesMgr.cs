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
using System.Reflection;
using tainicom.ProtonType.App.ViewModels;
using tainicom.ProtonType.Framework.Modules;

namespace tainicom.ProtonType.App.Modules
{
    class ModulesMgr
    {
        private readonly List<IModule> _modules = new List<IModule>();
        private readonly Dictionary<Type, IModule> initializedModuleList = new Dictionary<Type, IModule>();
        private Site _site;

        private ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.MenuViewModel> _menuGroups = new ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.MenuViewModel>();
        private ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.ToolbarViewModel> _toolbarGroups = new ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.ToolbarViewModel>();
        private ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.StatusBarItemViewModel> _statusbarGroups = new ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.StatusBarItemViewModel>();
        
        internal ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.MenuViewModel> MenuGroups { get { return _menuGroups; } }
        internal ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.ToolbarViewModel> ToolbarGroups { get { return _toolbarGroups; } }
        internal ObservableEnumerableAggregate<tainicom.ProtonType.Framework.ViewModels.StatusBarItemViewModel> StatusbarGroups { get { return _statusbarGroups; } }
        
        internal ISiteViewModel Site { get { return _site; } }

        internal ModulesMgr()
        {
            LoadModules();
        }

        internal void Initialize(MainViewModel mainViewModel)
        {
            _site = new Site(mainViewModel);
            InitializeModules(_site);
        }
        
        private void LoadModules()
        {
            //Get plugin directory
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string modulesDirectory = "";

            LoadModules(baseDirectory, modulesDirectory);
            DebugDrawModulesList();

            return;
        }

        internal void LoadDocumentModules(string baseDirectory)
        {
            string modulesDirectory = "Modules";

            LoadModules(baseDirectory, modulesDirectory);
            InitializeModules(_site);

            DebugDrawModulesList();
        }

        private void InitializeModules(Site _site)
        {
            foreach (var module in _modules)
            {
                if (!initializedModuleList.ContainsKey(module.GetType()))
                {
                    IModuleUI moduleUI = module as IModuleUI;
                    if (moduleUI !=null)
                    {
                        var menus = moduleUI.Menus;
                        if (menus != null)
                            this._menuGroups.AddAggregatedCollection(menus);

                        var toolbars = moduleUI.Toolbars;
                        if (toolbars != null)
                            this._toolbarGroups.AddAggregatedCollection(toolbars);

                        var statusbars = moduleUI.StatusBars;
                        if (statusbars != null)
                            this._statusbarGroups.AddAggregatedCollection(statusbars);
                    }

                    module.Initialize(_site);
                    initializedModuleList.Add(module.GetType(), module);
                }
            }
        }

        internal IEnumerable<TModule> GetModules<TModule>() where TModule : class , IModule
        {
            var itemType = typeof(TModule);
            for (int i = 0; i < _modules.Count; i++)
            {
                if (itemType.IsInstanceOfType(_modules[i]))
                    yield return (TModule)_modules[i];
            }
        }

        private void LoadModules(string baseDirectory, string modulesDirectory)
        {
            baseDirectory = Path.Combine(baseDirectory, modulesDirectory);
            if (!Directory.Exists(baseDirectory))
                return;

            //Go through all the files in the directory
            var files = Directory.GetFiles(baseDirectory);
            foreach(var filename in files)
                LoadModule(filename);

            //Go through all the directories in the directory
            foreach(var childDirectory in Directory.GetDirectories(baseDirectory))
            {
                string name = Path.GetFileName(childDirectory);
                string filename = Path.Combine(childDirectory, name + ".dll");
                LoadModule(filename);
            }
            return;
        }

        private bool LoadModule(string file)
        {
            if (Path.GetExtension(file).ToLower() != ".dll") return false;
            if (!File.Exists(file)) return false;

            Assembly asm;
            System.Diagnostics.Debug.WriteLine("ModulesMgr: loading assembly: " + Path.GetFileName(file));
            try { asm = Assembly.LoadFrom(file); }
            catch (BadImageFormatException ex) // wrong platform (x86/x64) or not .Net Assembly
            {
                System.Diagnostics.Debug.WriteLine("ModulesMgr: Failed to load Module: " + Path.GetFileName(file));
                return false;
            }

            try
            {
                var types = asm.GetTypes();
                foreach (Type type in types)
                {
                    if (!type.IsPublic) continue;
                    if (type.IsAbstract) continue;

                    Type typeIModule = type.GetInterface(typeof(tainicom.ProtonType.Framework.Modules.IModule).FullName);
                    if (typeIModule != null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine("ModulesMgr: loading module: " + type.Name);
                            Object instance = asm.CreateInstance(type.FullName);
                            IModule module = (IModule)instance;
                            lock (_modules)
                                _modules.Add(module);
                        }
                        finally { }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine("failed to load module: " + ex.FileName);
            }

            return true;
        }

        private void LoadModules_MEF(string baseDirectory, string modulesDirectory)
        {
            string fullModulesDirectory = Path.Combine(baseDirectory, modulesDirectory);
            if (!Directory.Exists(fullModulesDirectory))
                return;

            // Go through all the files in the directory
            //var directoryCatalog = new DirectoryCatalog(fullModulesDirectory, "*.dll");
            //catalog.Catalogs.Add(directoryCatalog);

            //Go through all the directories in the directory
            foreach (string subDirectory in Directory.GetDirectories(fullModulesDirectory))
            {
                string name = Path.GetFileName(subDirectory);
                string filename = name + ".dll";
                string fullFilename = Path.Combine(subDirectory, filename);
                if (!File.Exists(fullFilename))
                    continue;

                //var subDirectoryCatalog = new DirectoryCatalog(subDirectory, filename);
                //catalog.Catalogs.Add(subDirectoryCatalog);
            }

            return;
        }
        
        private void DebugDrawModulesList()
        {
            for(int i = 0; i < _modules.Count; i++)
                System.Diagnostics.Debug.WriteLine("_modules[{0}] = {1:X8}, {2}", i, _modules[i].GetHashCode(), _modules[i].GetType().Name);

            int i2 =0;
            foreach(var key in initializedModuleList.Keys)
            {
                System.Diagnostics.Debug.WriteLine("initializedModuleList[{0}] = {1:X8}, {2}", i2, initializedModuleList[key].GetHashCode(), initializedModuleList[key].GetType().Name);
                i2++;
            }

            //for (int i = 0; i < _contentLibraries.Count; i++)
            //    System.Diagnostics.Debug.WriteLine("_contentLibraries[{0}] = {1:X8}, {2}", i, _contentLibraries[i].GetHashCode(), _contentLibraries[i].GetType().Name);
        }
    }
}
