/* ------------------------------------------------------------------------- */
///
/// StringToIntConverter.cs
///
/// Copyright (c) 2013 CubeSoft, Inc. All rights reserved.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
///
/// You should have received a copy of the GNU General Public License
/// along with this program.  If not, see < http://www.gnu.org/licenses/ >.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Windows.Data;
using System.Globalization;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// StringToIntConverter
    /// 
    /// <summary>
    /// 文字列から数値へのコンバータクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class StringToIntConverter : IValueConverter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        /// 
        /// <summary>
        /// string 型から int 型へ変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try { return int.Parse(value as string); }
            catch (Exception /* err */) { return default(int); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ConvertBack
        /// 
        /// <summary>
        /// int 型から string 型へ変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return value.ToString();
        }
    }
}
