/* ------------------------------------------------------------------------- */
///
/// VirtualizationListProxy.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// VirtualizationListProxy
    /// 
    /// <summary>
    /// ListBox や ListView 等のコンポーネントにリスト内の要素を表示する
    /// 際に、必要な要素（画面に表示されている要素）のみをメモリー上に確保
    /// して仮想化するためのプロキシークラスです。各要素は、添え字による
    /// アクセスが発生するまでは生成されません。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class VirtualizingListProxy<T> : IList<T>, IList, INotifyCollectionChanged
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        /// Constructor
        /* ----------------------------------------------------------------- */
        public VirtualizingListProxy(IItemsProvider<T> provider)
        {
            _provider = provider;
            // _provider.CollectionChanged -= new NotifyCollectionChangedEventHandler(CollectionChangedProxy);
            // _provider.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedProxy);
        }

        #endregion

        /* ----------------------------------------------------------------- */
        /// ItemsProvicer
        /* ----------------------------------------------------------------- */
        public IItemsProvider<T> ItemsProvicer
        {
            get { return _provider; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Count
        ///
        /// <summary>
        /// リストに本来格納されているべき数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int Count
        {
            get { return _provider.ProvideItemsCount(); }
            set { throw new NotSupportedException(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// this[int index]
        /// 
        /// <summary>
        /// 添え字に対応する要素を取得します。多くの場合、アクセスが発生
        /// した時点で初めて IItemsProvider<T> オブジェクトに該当する要素を
        /// 生成するようにリクエストします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public T this[int index]
        {
            get { return _provider.ProvideItem(index); }
            set { throw new NotSupportedException(); }
        }

        /* ----------------------------------------------------------------- */
        /// GetEnumerator
        /* ----------------------------------------------------------------- */
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i) yield return this[i];
        }

        #region Other Methods
        public object SyncRoot { get { return this; } }
        public bool IsSynchronized { get { return false; } }
        public bool IsReadOnly { get { return true; } }
        public bool IsFixedSize { get { return false; } }
        #endregion

        #region Not Supported Methods for IList<T>
        public void Add(T item) { throw new NotSupportedException(); }
        public bool Contains(T item) { return false; }
        public void Clear() { throw new NotSupportedException(); }
        public int IndexOf(T item) { return -1; }
        public void Insert(int index, T item) { throw new NotSupportedException(); }
        public void RemoveAt(int index) { throw new NotSupportedException(); }
        public bool Remove(T item) { throw new NotSupportedException(); }
        public void CopyTo(T[] array, int arrayIndex) { throw new NotSupportedException(); }
        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// Methods for IList
        /// 
        /// NOTE: IList のメソッドの多くは IList<T> の該当メソッドの
        /// ラッパーとして定義されていますが、いくつかのメソッドは戻り値の
        /// 不一致等の理由から、NotSupportedException() を送出しています。
        /// もし、IList<T> の該当メソッドを実装した場合は、これらの
        /// 独自に NotSupportedException() を送出しているメソッドも修正する
        /// 必要があります。
        ///
        /* ----------------------------------------------------------------- */
        #region Methods for IList
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

        // NOTE: 戻り値の不一致のため NotSupportedException() を送出しているメソッド群
        int IList.Add(object value) { throw new NotSupportedException(); }
        void ICollection.CopyTo(Array array, int index) { throw new NotSupportedException(); }
        #endregion

        #region INotifyCollectionChanged

        /* ----------------------------------------------------------------- */
        ///
        /// CollectionChanged
        /// 
        /// <summary>
        /// リストの要素に何らかの変更が生じた場合に発生するイベントです。
        /// IItemsProvider インターフェースを継承したクラスで発生した
        /// CollectionChanged イベントをそのまま伝播します。
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
        /// IItemsProvider<T> オブジェクトで発生した CollectionChanged
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
        private readonly IItemsProvider<T> _provider;
        #endregion
    }
}
