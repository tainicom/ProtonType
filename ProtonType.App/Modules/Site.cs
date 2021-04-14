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
using tainicom.ProtonType.App.ViewModels;
using tainicom.ProtonType.Framework.Commands;
using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.App.Modules
{
    internal partial class Site : ISiteViewModel, ISiteViewModelReceiver
    {
        MainViewModel _mainViewModel;

        public ICommandController Controller { get { return _mainViewModel.Controller; } }

        public PaneViewModel ActiveContent { get { return _mainViewModel.ActiveContent; } }

        public Site(MainViewModel mainViewModel)
        {
            this._mainViewModel = mainViewModel;
        }

        public IEnumerable<TModule> GetModules<TModule>()
             where TModule : class , IModule
        {
            // enforce Plug in communication via Interfaces
            if (!typeof(TModule).IsInterface)
                throw new InvalidOperationException("TModule is not an interface");

            return _mainViewModel.GetModules<TModule>();
        }
        
        #region ISiteViewModelReceiver
        bool ISiteViewModelReceiver.__AddPane(PaneViewModel pane)
        {
            return _mainViewModel.__AddPane(pane);
        }

        bool ISiteViewModelReceiver.__RemovePane(PaneViewModel pane)
        {
            return _mainViewModel.__RemovePane(pane);
        }
        #endregion ISiteViewModelReceiver
    }
}
