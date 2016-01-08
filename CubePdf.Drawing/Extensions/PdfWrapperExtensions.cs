/* ------------------------------------------------------------------------- */
///
/// PdfWrapperExtensions.cs
///
/// Copyright (c) 2010 CubeSoft, Inc. All rights reserved.
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
using PDFLibNet;
using System.Drawing;
using CubePdf.Data;

namespace CubePdf.Drawing.Extensions
{
    /* --------------------------------------------------------------------- */
    ///
    /// CubePdf.Drawing.Extensions.PdfWrapperExtensions
    /// 
    /// <summary>
    /// PDFLibNet の PDFWrapper に関する拡張メソッド群を定義するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    internal static class PdfWrapperExtensions
    {
        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// CreatePage
        /// 
        /// <summary>
        /// Page オブジェクトを生成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static Page CreatePage(this PDFWrapper reader, string path, string password, int pagenum)
        {
            PDFPage obj;
            if (!reader.Pages.TryGetValue(pagenum, out obj)) return null;

            var dest = new Page();
            dest.FilePath = path;
            dest.PageNumber = pagenum;
            dest.Password = password;
            dest.OriginalSize = new Size(Round(SizeHack(obj.Width)), Round(SizeHack(obj.Height)));
            dest.Rotation = obj.Rotation;
            return dest;
        }

        #endregion

        #region Other private methods

        /* ----------------------------------------------------------------- */
        ///
        /// SizeHack
        /// 
        /// <summary>
        /// 幅、高さに関するハック用メソッドです。
        /// </summary>
        /// 
        /// <remarks>
        /// PDFLibNet/AFPDFLib/PDFPageInterop クラスの getPageWidth()、
        /// および getPageHeight() メソッドにて、元の値に対して 254 / 72
        /// と言う値を掛けてしまっているため、暫定的に逆数を掛ける事と
        /// する。254 が何を意味するのか要調査。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private static double SizeHack(double src)
        {
            return src * (72 / 254.0);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Round
        /// 
        /// <summary>
        /// double の小数点第 1 位を四捨五入します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static int Round(double value)
        {
            return (int)(value + 0.5);
        }

        #endregion
    }
}
