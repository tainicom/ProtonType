﻿#region License
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
using System.Windows.Controls;
using tainicom.ProtonType.App.ViewModels;

namespace tainicom.ProtonType.App.Views
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void TouchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = e.Source as Button;
            var welcomeViewModel = this.DataContext as WelcomeViewModel;            
            welcomeViewModel.OpenCommand.Execute((object)button.DataContext);
        }


        


    }
}
