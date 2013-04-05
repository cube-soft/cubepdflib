/* ------------------------------------------------------------------------- */
///
/// EnumToIntConverter.cs
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
    /// EnumToIntConverter
    /// 
    /// <summary>
    /// Enum 型と int の相互変換を行います。主に、Enum 型の値を ComboBox 等の
    /// インデックスに使用する際に利用するコンバータクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class EnumToIntConverter<T> : IValueConverter where T : struct, IConvertible
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        /// 
        /// <summary>
        /// Enum 型から int へ変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var id = (T)value;
                return id.ToInt32(null);
            }
            catch (InvalidCastException /* err */) { return default(T).ToInt32(null); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ConvertBack
        /// 
        /// <summary>
        /// int から Enum 型へ変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var index = (int)value;
                return (T)Enum.Parse(typeof(T), index.ToString(), true);
            }
            catch (InvalidCastException /* err */) { return default(T); }
        }
    }
}
