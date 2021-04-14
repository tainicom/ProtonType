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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace tainicom.ProtonType.App.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    class BoolToColorBrushConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Bolean value controlling wether to apply color change</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">A CSV string on the format [ColorNameIfTrue;ColorNameIfFalse;OpacityNumber] may be provided for customization, default is [LimeGreen;Transperent;1.0].</param>
        /// <param name="culture"></param>
        /// <returns>A SolidColorBrush in the supplied or default colors depending on the state of value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Setting default values
            var colorIfTrue = new SolidColorBrush(Colors.LimeGreen);
            var colorIfFalse = new SolidColorBrush(Colors.Transparent);
            double opacity = 1;
            // Parsing converter parameter
            if (parameter != null)
            {
                // Parameter format: [ColorNameIfTrue;ColorNameIfFalse;OpacityNumber]
                var parameterstring = parameter.ToString();
                if (!string.IsNullOrEmpty(parameterstring))
                {
                    var parameters = parameterstring.Split(';');
                    var count = parameters.Length;
                    if (count > 0 && !string.IsNullOrEmpty(parameters[0]))
                    {
                        colorIfTrue = StringToColor(parameters[0]);
                    }
                    if (count > 1 && !string.IsNullOrEmpty(parameters[1]))
                    {
                        colorIfFalse = StringToColor(parameters[1]);
                    }
                    if (count > 2 && !string.IsNullOrEmpty(parameters[2]))
                    {
                        double dblTemp;
                        if (double.TryParse(parameters[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out dblTemp))
                            opacity = dblTemp;
                    }
                }
            }
            // Creating Color Brush
            SolidColorBrush brush;
            if ((bool)value)
                brush = colorIfTrue;
            else
                brush = colorIfFalse;
            brush.Opacity = opacity;
            return brush;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static SolidColorBrush StringToColor(string colorString)
        {
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorString);
            var color = Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
            return new SolidColorBrush(color);
        }
    }
}
