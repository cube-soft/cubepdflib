/* ------------------------------------------------------------------------- */
///
/// Page.cs
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

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// Page
    /// 
    /// <summary>
    /// PDF のページ毎の情報を保持するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [Serializable]
    public class Page : PageBase
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Page (constructor)
        ///
        /// <summary>
        /// 規定の値で Page クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Page() : base(PageType.Pdf) { }

        /* ----------------------------------------------------------------- */
        ///
        /// Page (constructor)
        /// 
        /// <summary>
        /// ファイルパス、ページ番号を指定して Page クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Page(string path, int pagenum)
            : base(PageType.Pdf)
        {
            FilePath = path;
            PageNumber = pagenum;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Password
        /// 
        /// <summary>
        /// PDF ファイルのパスワードを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Password { get; set; } = string.Empty;

        #endregion
    }
}
