/* ------------------------------------------------------------------------- */
///
/// ListProxy.cs
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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListProxy
    /// 
    /// <summary>
    /// ListBox や ListView 等のコンポーネントにリスト内の要素を表示する
    /// 際に、必要な要素（画面に表示されている要素）のみをメモリー上に確保
    /// して仮想化するためのプロキシークラスです。各要素は、IItemsProvider
    /// オブジェクトを効果的に利用する事により、添え字によるアクセスが発生
    /// するまでオブジェクトの生成を遅延させる事ができます。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ListProxy<T> : IListProxy<T>
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// ListProxy
        /// 
        /// <summary>
        /// 既定値でオブジェクトを初期化します。IItemsProvider オブジェクトを
        /// 指定しない場合、ListProxy クラスはただのラッパーコレクション
        /// として振る舞います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListProxy() {
            _buffer.CollectionChanged -= new NotifyCollectionChangedEventHandler(CollectionChangedProxy);
            _buffer.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedProxy);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ListProxy
        /// 
        /// <summary>
        /// 指定された IItemsProvider オブジェクトを利用して、オブジェクトを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListProxy(IItemsProvider<T> provider)
            : this()
        {
            _provider = provider;
        }

        #endregion

        #region Implementations for IListProxy original methods

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
        public IItemsProvider<T> ItemsProvider
        {
            get { return _provider; }
        }

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
        public int RawCount
        {
            get { return _buffer.Count; }
        }

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
        public T RawAt(int index)
        {
            return _buffer[index];
        }

        #endregion

        #region Proxy methods

        /* ----------------------------------------------------------------- */
        ///
        /// Count
        ///
        /// <summary>
        /// リストに本来格納されているべき数を取得します。IItemsProvider
        /// オブジェクトが設定されていない場合は、内部バッファの Count
        /// プロパティの値が直接取得されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int Count
        {
            get { return (_provider != null) ? _provider.ProvideItemsCount() : _buffer.Count; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// this[int index]
        /// 
        /// <summary>
        /// 添え字に対応する要素を取得します。多くの場合、アクセスが発生
        /// した時点で初めて IItemsProvider オブジェクトに該当する要素を
        /// 生成するようにリクエストします。IItemsProvider オブジェクトが
        /// 設定されていない場合は、内部バッファの該当要素が直接取得されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public T this[int index]
        {
            get { return (_provider != null) ? _provider.ProvideItem(index) : _buffer[index]; }
            set { throw new NotSupportedException(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetEnumerator
        ///
        /// <summary>
        /// コレクションを反復処理する列挙子を返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i) yield return this[i];
        }

        #endregion

        #region Implementations for IList<T> methods
        public void Add(T item) { _buffer.Add(item); }
        public bool Contains(T item) { return _buffer.Contains(item); }
        public void Clear() { _buffer.Clear(); }
        public int IndexOf(T item) { return _buffer.IndexOf(item); }
        public void Insert(int index, T item) { _buffer.Insert(index, item); }
        public void RemoveAt(int index) { _buffer.RemoveAt(index); }
        public bool Remove(T item) { return _buffer.Remove(item); }
        public void CopyTo(T[] array, int arrayIndex) { throw new NotSupportedException(); }
        #endregion

        #region Implementations for IList methods
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
        bool IList.Contains(object value) { return this.Contains((T)value); }
        int IList.IndexOf(object value) { return this.IndexOf((T)value); }
        void IList.Insert(int index, object value) { this.Insert(index, (T)value); }
        void IList.Remove(object value) { this.Remove((T)value); }
        int IList.Add(object value) { this.Add((T)value); return this.Count - 1; }
        void ICollection.CopyTo(Array array, int index) { throw new NotSupportedException(); }
        #endregion

        #region Other Methods
        public object SyncRoot { get { return this; } }
        public bool IsSynchronized { get { return false; } }
        public bool IsReadOnly { get { return true; } }
        public bool IsFixedSize { get { return false; } }
        #endregion

        #region Implementations for INotifyCollectionChanged

        /* ----------------------------------------------------------------- */
        ///
        /// CollectionChanged
        /// 
        /// <summary>
        /// リストの要素に何らかの変更が生じた場合に発生するイベントです。
        /// 内部バッファとして使用している ObservableCollection オブジェクト
        /// で発生した CollectionChanged イベントをそのまま伝播します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null) handler(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CollectionChangedProxy
        /// 
        /// <summary>
        /// ObservableCollection(T) オブジェクトで発生した CollectionChanged
        /// イベントを伝播させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CollectionChangedProxy(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        #endregion

        #region Variables
        private IItemsProvider<T> _provider = null;
        private ObservableCollection<T> _buffer = new ObservableCollection<T>();
        #endregion
    }
}
