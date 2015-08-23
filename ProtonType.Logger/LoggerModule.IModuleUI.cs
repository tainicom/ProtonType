#region License
//   Copyright 2020-2021 Kastellanos Nikolaos
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using tainicom.ProtonType.Framework.Commands;
using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;
using tainicom.ProtonType.Logger.ViewModels;

namespace tainicom.ProtonType.Logger
{
    public partial class LoggerModule : IModuleUI
    {
        private ToolbarViewModel toolbarViewModel;
        
        #region IModule Members

        private void InitializeModuleUI()
        {
            //add Menus
            var iconSource = CreateImageSource(@"Icons/BuildErrorList_16x.png");
            var menuErrorList = new MenuViewModel("ErrorList", "Panels", iconSource);
            menuErrorList.ClickHandler = miErrorList_Click;
            this._menus.Add(menuErrorList);

            var iconSource2 = CreateImageSource(@"Icons/Output_16xLG.png");
            var menuOutput = new MenuViewModel("Output", "Panels", iconSource2);
            menuOutput.ClickHandler = miOutput_Click;
            this._menus.Add(menuOutput);
        }

        #endregion


        internal static ImageSource CreateImageSource(string resourcePath)
        {
            var baseUrl = @"/ProtonType.LoggerModule;component/";
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(baseUrl + resourcePath, UriKind.Relative);
            bmp.EndInit();
            return bmp;
        }

        void miErrorList_Click(MenuViewModel sender)
        {
            var vm = GetdefaultErrorListViewModel();
            var addPaneCmd = new AddPaneCmd(Site, vm);
            Site.Controller.EnqueueAndExecute(addPaneCmd);
        }

        void miOutput_Click(MenuViewModel sender)
        {
            var vm = GetdefaultOutputViewModel();
            var addPaneCmd = new AddPaneCmd(Site, vm);
            Site.Controller.EnqueueAndExecute(addPaneCmd);
        }
        

        #region IModuleUI Members
        ObservableCollection<tainicom.ProtonType.Framework.ViewModels.MenuViewModel> _menus = new ObservableCollection<tainicom.ProtonType.Framework.ViewModels.MenuViewModel>();
        ObservableCollection<tainicom.ProtonType.Framework.ViewModels.ToolbarViewModel> _toolbars = new ObservableCollection<tainicom.ProtonType.Framework.ViewModels.ToolbarViewModel>();
        ObservableCollection<tainicom.ProtonType.Framework.ViewModels.StatusBarItemViewModel> _statusbars = new ObservableCollection<tainicom.ProtonType.Framework.ViewModels.StatusBarItemViewModel>();
        IEnumerable<tainicom.ProtonType.Framework.ViewModels.MenuViewModel> IModuleUI.Menus { get { return _menus; } }
        IEnumerable<tainicom.ProtonType.Framework.ViewModels.ToolbarViewModel> IModuleUI.Toolbars { get { return _toolbars; } }
        IEnumerable<tainicom.ProtonType.Framework.ViewModels.StatusBarItemViewModel> IModuleUI.StatusBars { get { return _statusbars; } }
        #endregion IModuleUI Members
        
    }
}
