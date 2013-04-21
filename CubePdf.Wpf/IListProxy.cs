/* ------------------------------------------------------------------------- */
///
/// IListProxy.cs
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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// IListProxy
    /// 
    /// <summary>
    /// ListBox や ListView 等のコンポーネントにリスト内の要素を表示する
    /// 際に、必要な要素（画面に表示されている要素）のみをメモリー上に確保
    /// して仮想化するためのプロキシークラスのインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IListProxy<T> : IList<T>, IList, INotifyCollectionChanged
    {
        /* ----------------------------------------------------------------- */
        ///
        /// IItemsProvider
        /// 
        /// <summary>
        /// 添え字によるアクセスが発生した際に、経由させるオブジェクトを
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        IItemsProvider<T> ItemsProvider { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// RawCount
        /// 
        /// <summary>
        /// IItemsProvider を経由せずに内部バッファの Count プロパティの
        /// を値を直接取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int RawCount { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// RawAt
        /// 
        /// <summary>
        /// IItemsProvider を経由せずに内部バッファの該当要素を直接取得
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        T RawAt(int index);

        /* ----------------------------------------------------------------- */
        ///
        /// Move
        /// 
        /// <summary>
        /// oldindex 番目の項目を newindex へ移動します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Move(int oldIndex, int newIndex);

        /* ----------------------------------------------------------------- */
        ///
        /// Re-definition methods
        /// 
        /// <summary>
        /// IList(T) と IList のどちらのメソッドを呼び出すか解決できないため、
        /// 曖昧さを解決するために ILIstProxy で再定義します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Re-definition methods
        new int Count { get; }
        new T this[int index] { get; set; }
        new void Clear();
        new void RemoveAt(int index);
        #endregion
    }
}
