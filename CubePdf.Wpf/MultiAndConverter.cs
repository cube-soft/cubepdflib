/* ------------------------------------------------------------------------- */
///
/// MultiAndConverter.cs
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
using System;
using System.Windows.Data;
using System.Globalization;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// MultiAndConverter
    /// 
    /// <summary>
    /// 複数の論理値の論理積に変換するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class MultiAndConverter : IMultiValueConverter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        /// 
        /// <summary>
        /// values に指定された複数の論理値から論理積へ変換します。
        /// values に指定された bool 型以外の値は無視されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return false;

            var dest = true;
            foreach (var value in values)
            {
                try { dest &= (bool)value; }
                catch (Exception /* err */) { }
            }
            return dest;
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
