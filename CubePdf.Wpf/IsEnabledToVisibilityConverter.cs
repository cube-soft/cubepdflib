/* ------------------------------------------------------------------------- */
///
/// IsEnabledToVisibilityConverter.cs
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
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// IsEnabledToVisibilityConverter
    /// 
    /// <summary>
    /// Control オブジェクトの IsEnabled の値に応じて Visibility を変更する
    /// ためのコンバータクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class IsEnabledToVisibilityConverter : IValueConverter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        /// 
        /// <summary>
        /// 真偽値 (IsEnabled) を基に Visibility を決定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var enabled = (bool)value;
                return enabled ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception /* err */) { return Visibility.Collapsed; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ConvertBack
        /// 
        /// <summary>
        /// Visibility の値を基に真偽値を決定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var visibility = (Visibility)value;
                return (visibility == Visibility.Visible) ? true : false;
            }
            catch (Exception /* err */) { return false; }
        }
    }
}
