/* ------------------------------------------------------------------------- */
///
/// DragDropPages.cs
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
using System.Collections.Generic;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// DragDropPages
    /// 
    /// <summary>
    /// マウスのドラッグ&ドロップ操作で他のプロセスへ PDF のページを移動する
    /// 際に使用するデータ格納クラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [Serializable]
    public class DragDropPages
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// DragDropPages (constructor)
        /// 
        /// <summary>
        /// 引数に指定されたプロセス ID を用いてオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DragDropPages(int id)
        {
            _id = id;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// ProccessId
        /// 
        /// <summary>
        /// プロセス ID を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ProccessId
        {
            get { return _id; }
            set { _id = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// ページ情報の一覧を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IList<CubePdf.Data.IPage> Pages
        {
            get { return _pages; }
        }

        #endregion

        #region Variables
        public int _id = -1;
        IList<CubePdf.Data.IPage> _pages = new List<CubePdf.Data.IPage>();
        #endregion
    }
}
