/* ------------------------------------------------------------------------- */
///
/// ControlToTagConverter.cs
///
/// Copyright (c) 2013 CubeSoft, Inc. All rights reserved.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
///
/* ------------------------------------------------------------------------- */
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ControlToTagConverter
    /// 
    /// <summary>
    /// 各コントロールから Tag に設定されているオブジェクトを抽出するための
    /// コンバータクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ControlToTagConverter : IValueConverter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        /// 
        /// <summary>
        /// Control オブジェクトの Tag プロパティに設定されているオブジェクト
        /// を抽出して返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var control = value as Control;
            return (control != null) ? control.Tag : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ConvertBack
        /// 
        /// <summary>
        /// 未実装
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
