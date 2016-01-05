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
        public static Page CreatePage(this PDFWrapper reader, string path, string password, int pagenum)
        {
            PDFPage obj;
            if (!reader.Pages.TryGetValue(pagenum, out obj)) return null;

            var dest = new Page();
            dest.FilePath = path;
            dest.PageNumber = pagenum;
            dest.Password = password;
            dest.OriginalSize = new Size((int)obj.Width, (int)obj.Height);
            dest.Rotation = obj.Rotation;
            return dest;
        }

    }
}
