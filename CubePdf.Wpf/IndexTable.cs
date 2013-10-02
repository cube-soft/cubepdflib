/* ------------------------------------------------------------------------- */
///
/// IndexTable.cs
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
    /// IndexTable
    /// 
    /// <summary>
    /// 画面に表示されているサムネイルのインデックスを管理するための
    /// クラスです。
    /// </summary>
    /// 
    /// <remarks>
    /// PDF のページ数が多くなると、全てのページのサムネイルを作成し表示する
    /// のはメモリ使用量的に問題となります。IndexTable クラスは、必要な
    /// サムネイルのみを生成するために、現在、どの範囲（インデックス）の
    /// サムネイルを生成しているのかを管理するために使用します。
    /// </remarks>
    ///
    /* --------------------------------------------------------------------- */
    public class IndexTable
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// IndexTable (constructor)
        /// 
        /// <summary>
        /// 既定の値で IndexTable クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IndexTable() { }

        /* ----------------------------------------------------------------- */
        ///
        /// IndexTable (constructor)
        /// 
        /// <summary>
        /// インデックスを管理する対象となる IListProxy オブジェクトを用いて
        /// IndexTable クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IndexTable(IListProxy<CubePdf.Drawing.ImageContainer> items)
            : this()
        {
            _items = items;
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        /// 
        /// <summary>
        /// 管理テーブルを初期状態にクリアします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Clear()
        {
            _indices.Clear();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Update
        /// 
        /// <summary>
        /// 引数に指定されたインデックスを追加します。
        /// </summary>
        /// 
        /// <remarks>
        /// 現在のテーブルが Capacity 以上の場合、引数に指定された
        /// インデックスから最も遠いインデックスを削除します。この際、
        /// Items にオブジェクトが設定されている場合には該当インデックスの
        /// イメージを削除します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Update(int index)
        {
            var capacity = _indices.ContainsKey(index) ? _capacity + 1 : _capacity;
            while (_indices.Count > 0 && _indices.Count >= capacity)
            {
                var first  = _indices.Keys[0];
                var last   = _indices.Keys[_indices.Count - 1];
                var remove = (Math.Abs(index - first) > Math.Abs(index - last)) ? first : last;
                _indices.Remove(remove);
                if (_items != null)
                {
                    lock (_items) _items.RawAt(remove).DeleteImage();
                }
            }
            if (_indices.ContainsKey(index) || _capacity <= 0) return;
            _indices.Add(index, null);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemInserted
        /// 
        /// <summary>
        /// インデックスを管理する対象となる IListProxy オブジェクトに
        /// 新しい項目が追加された時に実行するメソッドです。
        /// </summary>
        /// 
        /// <remarks>
        /// TODO: 将来的には IListProxy オブジェクトの CollectionChanged
        /// イベントに連動させる形が良いか。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void ItemInserted(int index) { ItemInserted(index, 1); }
        public void ItemInserted(int first, int count)
        {
            var indices = new SortedList<int, object>();
            var same = true;
            foreach (var key in _indices.Keys)
            {
                same &= (key < first);
                var index = (key < first) ? key : key + count;
                indices.Add(index, null);
            }

            if (!same)
            {
                _indices.Clear();
                _indices = indices;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemRemoved
        /// 
        /// <summary>
        /// インデックスを管理する対象となる IListProxy オブジェクトから
        /// 項目が削除された時に実行するメソッドです。
        /// </summary>
        /// 
        /// <remarks>
        /// TODO: 将来的には IListProxy オブジェクトの CollectionChanged
        /// イベントに連動させる形が良いか。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void ItemRemoved(int index) { ItemRemoved(index, 1); }
        public void ItemRemoved(int first, int count)
        {
            var indices = new SortedList<int, object>();
            var same = true;

            foreach (var key in _indices.Keys)
            {
                same &= (key < first);
                if (key >= first && key < first + count) continue;
                var index = (key < first) ? key : key - count;
                indices.Add(index, null);
            }

            if (!same)
            {
                _indices.Clear();
                _indices = indices;
            }
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Capacity
        /// 
        /// <summary>
        /// 管理するインデックスの最大数を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Indices
        /// 
        /// <summary>
        /// 現在有効なインデックスの一覧を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IList<int> Indices
        {
            get { return _indices.Keys; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Items
        /// 
        /// <summary>
        /// インデックスを管理する対象となる IListProxy オブジェクトを
        /// 取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IListProxy<CubePdf.Drawing.ImageContainer> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion

        #region Variables
        private int _capacity = 0;
        private SortedList<int, object> _indices = new SortedList<int, object>();
        private IListProxy<CubePdf.Drawing.ImageContainer> _items = null;
        #endregion
    }
}
