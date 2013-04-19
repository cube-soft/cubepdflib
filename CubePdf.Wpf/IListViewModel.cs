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
using System.Drawing;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// IListViewModel
    /// 
    /// <summary>
    /// ListView に CubePdf.Drawing.BitmapEngine で生成される各 PDF ページ
    /// のサムネイルを表示、および各種操作を行うためのインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IListViewModel : CubePdf.Data.IDocumentReader, CubePdf.Data.IDocumentWriter, IItemsProvider<CubePdf.Drawing.ImageContainer>
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF のメタ情報を取得します。Metadata プロパティは、IDocumentReader
        /// インターフェースでは get のみ、IDocumentWriter インターフェース
        /// では get/set 両方とも許可されています。IListViewModel
        /// インターフェースでは、IDocumentWriter インターフェースのものを
        /// 引き継ぐ事とします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        new CubePdf.Data.IMetadata Metadata { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF の各ページ情報を参照するための反復子を取得します。
        /// Pages プロパティは、IDocumentReader インターフェースでは
        /// IEnumerable(T) クラス、IDocumentWriter インターフェースでは
        /// ICollection(T) クラスを戻り値としています。IListViewModel
        /// インターフェースでは、IDocumentReader インターフェースのものを
        /// 引き継ぐ事とします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        new IEnumerable<CubePdf.Data.IPage> Pages { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// IsModified
        /// 
        /// <summary>
        /// 現在、開かれている PDF ファイルに対して何らかの変更が加えられた
        /// かどうかを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        bool IsModified { get; }

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
        /// ItemVisibility
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの表示方法を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ListViewItemVisibility ItemVisibility { get; set; }

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
        ObservableCollection<CubePdf.Drawing.ImageContainer> Items { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// HistoryLimit
        /// 
        /// <summary>
        /// 記録可能な履歴の最大値を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int HistoryLimit { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// History
        /// 
        /// <summary>
        /// これまでに実行した処理の履歴を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ObservableCollection<CommandElement> History { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoHistory
        /// 
        /// <summary>
        /// 直前に実行した Undo 処理の履歴を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ObservableCollection<CommandElement> UndoHistory { get; }

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

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルを上書き保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Save();

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページをページ末尾に追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Add(CubePdf.Data.IPage item);
        void Add(string path, string password = "");

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
        void Insert(int index, CubePdf.Data.IPage item);
        void Insert(int index, string path, string password = "");

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
        void Extract(IList<CubePdf.Data.IPage> pages, string path);
        void Extract(IList items, string path);

        /* ----------------------------------------------------------------- */
        ///
        /// Split
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページを direcotry 下に
        /// 1 ページずつ別ファイルとして保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Split(IList<CubePdf.Data.IPage> pages, string directory);
        void Split(IList items, string directory);

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトに対応する PDF ページを削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Remove(CubePdf.Data.IPage item);
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
        /// Rotate
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトに対応する PDF ページを degree 度
        /// 回転させます。角度は、現在表示されている画像に対する相対度数で
        /// 指定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Rotate(CubePdf.Data.IPage item, int degree);
        void Rotate(object item, int degree);

        /* ----------------------------------------------------------------- */
        ///
        /// Rotate
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイルに相当する
        /// PDF ページを degree 度回転させます。角度は、現在表示されている
        /// 画像に対する相対度数で指定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void RotateAt(int index, int degree);

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
        /// BeginCommand
        /// 
        /// <summary>
        /// 一連の処理が始まる事を表します。主に Undo の際の処理粒度を調整
        /// する目的で使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void BeginCommand();

        /* ----------------------------------------------------------------- */
        ///
        /// EndCommand
        /// 
        /// <summary>
        /// 一連の処理が終わる事を表します。主に Undo の際の処理粒度を調整
        /// する目的で使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void EndCommand();

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
        /// Redo
        /// 
        /// <summary>
        /// 取り消した操作を再実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Redo();

        /* ----------------------------------------------------------------- */
        ///
        /// PreviewImage
        /// 
        /// <summary>
        /// ListView で表示されているサムネイルに対応するプレビュー用の
        /// イメージを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Image PreviewImage(int index, Size bound);

        /* ----------------------------------------------------------------- */
        ///
        /// IndexOf
        /// 
        /// <summary>
        /// ListView で表示されている項目、またはページ情報に対応する
        /// インデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int IndexOf(object item);
        int IndexOf(CubePdf.Data.IPage page);

        /* ----------------------------------------------------------------- */
        ///
        /// ToPage
        /// 
        /// <summary>
        /// ListView で表示されているサムネイルに対応する PDF ページの情報を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        CubePdf.Data.IPage ToPage(object item);

        #endregion
    }
}
