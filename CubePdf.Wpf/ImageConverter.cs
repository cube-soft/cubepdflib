/* ------------------------------------------------------------------------- */
///
/// ImageConverter.cs
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
using System.Drawing;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageConverter
    /// 
    /// <summary>
    /// System.Drawing.Image から System.Windows.Media.ImageSource への
    /// 変換を行います。WPF の各コントロールに画像を表示させる際に使用する
    /// コンバータクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ImageConverter : IValueConverter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        /// 
        /// <summary>
        /// System.Drawing.Image から System.Windows.Media.ImageSource へ
        /// 変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var src = value as Image;
            if (src == null) return null;

            using (var stream = new System.IO.MemoryStream())
            {
                src.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                src.Dispose();

                var dest = new BitmapImage();
                dest.BeginInit();
                dest.CacheOption = BitmapCacheOption.OnLoad;
                dest.StreamSource = new System.IO.MemoryStream(stream.ToArray());
                dest.EndInit();
                if (dest.CanFreeze) dest.Freeze();
                return dest;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ConvertBack
        /// 
        /// <summary>
        /// System.Windows.Media.ImageSource から System.Drawing.Image へ
        /// 変換します。現在、未実装です。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
