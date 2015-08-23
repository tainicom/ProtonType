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
using System.Linq;
using System.Text;
using tainicom.ProtonType.Framework.Attributes;
using tainicom.ProtonType.Framework.ViewModels;
using tainicom.ProtonType.Logger.Views;

namespace tainicom.ProtonType.Logger.ViewModels
{
    [DefaultView(typeof(OutputView), "bottomPane")]
    public class OutputViewModel : ToolViewModel
    {
        internal readonly LoggerModule Module;

         StringBuilder _output = new StringBuilder();

        public String Output 
        {
            get { return _output.ToString(); }
        }
        

        public OutputViewModel(LoggerModule module, System.Windows.Media.ImageSource iconSource) : base(module, "Output")
        {
            this.Module = module;
            this.IconSource = iconSource;
        }

        internal void WriteLine(string description)
        {
            _output.AppendLine(description);
            RaisePropertyChanged(() => Output);
        }
    }
}
