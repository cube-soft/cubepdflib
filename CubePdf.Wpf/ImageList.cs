/* ------------------------------------------------------------------------- */
///
/// ImageList.cs
///
/// Copyright (c) 2013 CubeSoft, Inc.
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
using System.Collections.Generic;
using System.Drawing;
using CubePdf.Data;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageList
    /// 
    /// <summary>
    /// イメージの作成要求を行う時の各種情報を格納するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ImageList
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Page
        ///
        /// <summary>
        /// ページ情報を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PageBase Page { get; set; } = null;

        /* ----------------------------------------------------------------- */
        ///
        /// ImageList
        ///
        /// <summary>
        /// イメージ一覧を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IList<Image> Images { get; set; } = new List<Image>();
    }
}
