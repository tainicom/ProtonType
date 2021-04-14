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
using System.Windows;
using tainicom.ProtonType.Framework.Modules;

namespace tainicom.ProtonType.App.ViewModels
{
    public partial class MainViewModel : IDisposable
    {
        internal void Window_DragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filepath in files)
                {
                    List<IModuleFile> modules = Model.FileDocumentsMgr.QueryFileModulesForFile(filepath);
                    if (modules.Count > 0)
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }
            }

            // disable cursor
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            return;
        }

        internal void Window_Drop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filepath in files)
                {
                    FileOpen(filepath);
                }
                e.Handled = true;
            }
            return;
        }

    }
}
