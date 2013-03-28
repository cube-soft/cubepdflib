/* ------------------------------------------------------------------------- */
///
/// IListViewModel.cs
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
using System.Collections.ObjectModel;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// IListViewModel
    /// 
    /// <summary>
    /// ListView に CubePdf.Drawing.BitmapEngine で生成される各 PDF ページ
    /// のサムネイルを表示、および各種操作を行うためのインターフェースです。
    /// T には System.Windows.Media.ImageSource クラス、またはその継承
    /// クラスが指定される事を想定しています。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IListViewModel<T> : IItemsProvider<T>
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// パスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string FilePath { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemWidth
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの幅を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int ItemWidth { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemCount
        /// 
        /// <summary>
        /// 現在、開いている（または各種操作を行った結果の）PDF ファイルに
        /// 含まれるページ数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int ItemCount { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Items
        /// 
        /// <summary>
        /// ListView に表示するサムネイル一覧を取得します。Items 中の
        /// 各サムネイルは必ずしも表示可能なデータとなっているとは
        /// 限りませんが、ListView で問題なく表示されるように実装されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ObservableCollection<T> Items { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// UnderItemCreation
        /// 
        /// <summary>
        /// ListView に表示するためのデータを非同期で生成している最中か
        /// どうかを判断します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        bool UnderItemCreation { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// MaxUndoCount
        /// 
        /// <summary>
        /// 連続して Undo（処理の取り消し）を行える最大回数を設定、または
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int MaxUndoCount { get; set; }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルを開き、画面に表示可能な状態にする
        /// ための準備を行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Open(string path, string password);
        void OpenAsync(string path, string password);

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在開いている PDF ファイルを閉じます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Close();

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルに保存します。引数に null が指定された
        /// 場合、Open メソッドにより開いたファイルに対して上書き保存を
        /// 試みます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Save(string path);

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページをページ末尾に追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Add(CubePdf.Data.Page item);
        void Add(string path, string password);
        void AddAsync(string path, string password);

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページを index の位置に挿入
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Insert(int index, CubePdf.Data.Page item);
        void Insert(int index, string path, string password);
        void InsertAsync(int index, string path, string password);

        /* ----------------------------------------------------------------- */
        ///
        /// Extract
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページを新しい PDF ファイル
        /// として path に保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Extract(IList<CubePdf.Data.Page> pages, string path);
        void Extract(IList<T> items, string path);
        void Extract(IList items, string path);

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        /// 
        /// <summary>
        /// 引数に指定されたものに相当する PDF ページを削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Remove(CubePdf.Data.Page item);
        void Remove(T item);
        void Remove(object item);

        /* ----------------------------------------------------------------- */
        ///
        /// RemoveAt
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイルに相当する
        /// PDF ページを削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void RemoveAt(int index);

        /* ----------------------------------------------------------------- */
        ///
        /// Move
        /// 
        /// <summary>
        /// ListView に表示されている oldindex 番目のサムネイルに相当する
        /// PDF ページを同 newindex へ移動させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Move(int oldindex, int newindex);

        /* ----------------------------------------------------------------- */
        ///
        /// Rotate
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイル画像を degree 度
        /// 回転させます。角度は、現在表示されている画像に対する相対度数で
        /// 指定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Rotate(int index, int degree);

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        /// 
        /// <summary>
        /// 直前の操作を取り消します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Undo();

        /* ----------------------------------------------------------------- */
        ///
        /// ToPage
        /// 
        /// <summary>
        /// ListView で表示されている画像に対応するページ情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        CubePdf.Data.Page ToPage(T item);
        CubePdf.Data.Page ToPage(object item);

        #endregion
    }
}
