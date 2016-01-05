/* ------------------------------------------------------------------------- */
///
/// ImageCreator.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Cube;
using CubePdf.Data;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageCreator
    /// 
    /// <summary>
    /// PDF の各ページのサムネイルを作成するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ImageCreator
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// ImageCreator
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageCreator() { }

        #endregion

        #region Events

        /* ----------------------------------------------------------------- */
        ///
        /// Creating
        /// 
        /// <summary>
        /// サムネイルが作成される直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EventHandler<DataCancelEventArgs<PageBase>> Creating;

        /* ----------------------------------------------------------------- */
        ///
        /// Created
        /// 
        /// <summary>
        /// サムネイルが作成された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EventHandler<DataEventArgs<KeyValuePair<PageBase, Image>>> Created;

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Register
        /// 
        /// <summary>
        /// サムネイルの作成用キューに登録します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Register(int index, PageBase page)
        {
            lock (_queue)
            {
                if (_queue.ContainsKey(index)) _queue[index] = page;
                else _queue.Add(index, page);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        /// 
        /// <summary>
        /// サムネイルの作成用キューに登録されている項目を全て削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Clear()
        {
            lock (_queue) _queue.Clear();
        }

        #endregion

        #region Virtual methods

        /* ----------------------------------------------------------------- */
        ///
        /// OnCreating
        /// 
        /// <summary>
        /// サムネイルが作成される直前に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnCreating(DataCancelEventArgs<PageBase> e)
        {
            if (Creating != null) Creating(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnCreated
        /// 
        /// <summary>
        /// サムネイルが作成された後に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnCreated(DataEventArgs<KeyValuePair<PageBase, Image>> e)
        {
            if (Created != null) Created(this, e);
        }

        #endregion

        #region Fields
        private SortedList<int, PageBase> _queue = new SortedList<int, PageBase>();
        #endregion
    }
}
