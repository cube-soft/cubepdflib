/* ------------------------------------------------------------------------- */
///
/// ListViewModel.cs
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
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListViewModel
    /// 
    /// <summary>
    /// ListView で表示される PDF ファイルの各ページの情報、およびイメージ
    /// データ等を管理するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ListViewModel : IListViewModel
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// ListViewModel (constructor)
        ///
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListViewModel()
        {
            _images = new ListProxy<CubePdf.Drawing.ImageContainer>(this);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// destructor
        /// 
        /// <summary>
        /// NOTE: クラスで必要な終了処理は、デストラクタではなく Dispose(bool)
        /// メソッドに記述して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ~ListViewModel()
        {
            this.Dispose(false);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        /// 
        /// <summary>
        /// NOTE: IDisposable で定義されているメソッドの実装部分です。実際に
        /// 必要な処理は Dispose(bool) メソッドに記述して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        /// 
        /// <summary>
        /// 終了時に必要な処理を記述します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing) this.Close();
        }

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// IListViewModel
        /// 
        /// <summary>
        /// IListViewModel インターフェースの（継承元インターフェース以外の）
        /// 各種メソッド、プロパティの実装を行います。IDocumentReader,
        /// IDocumentWriter インターフェースがページ番号ベースのアクセス
        /// 方法を提供しているのに対して、IListViewModel インターフェースで
        /// 提供されるメソッドは（ListView に表示されている項目への）
        /// インデックスベースとなっています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Implementations for IListViewModel original methods

        #region Properties

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
        public bool IsModified
        {
            get { return _modified; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// HistoryLimit
        /// 
        /// <summary>
        /// 記録可能な履歴の最大値を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int HistoryLimit
        {
            get { return _maxundo; }
            set { _maxundo = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// History
        /// 
        /// <summary>
        /// これまでに実行した処理の履歴を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ObservableCollection<CommandElement> History
        {
            get { return _undo; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoHistory
        /// 
        /// <summary>
        /// 直前に実行した Undo 処理の履歴を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ObservableCollection<CommandElement> UndoHistory
        {
            get { return _redo; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemWidth
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの幅を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ItemWidth
        {
            get { return _width; }
            set { _width = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemVisibility
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの表示方法を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListViewItemVisibility ItemVisibility
        {
            get { return _visibility; }
            set { _visibility = value; }
        }

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
        public IListProxy<CubePdf.Drawing.ImageContainer> Items
        {
            get { return _images; }
        }

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
        public bool UnderItemCreation
        {
            get { return GetRunningEngine() != null; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルを上書き保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save()
        {
            Save(_path);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトに対応する PDF ページをページ末尾に
        /// 追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Add(CubePdf.Data.IPage item) { Insert(_pages.Count, item); }
        public void Add(CubePdf.Data.IDocumentReader reader) { Insert(_pages.Count, reader); }
        public void Add(string path, string password = "") { Insert(_pages.Count, path, password); }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された PDF ページオブジェクトを index の位置に挿入
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert(int index, CubePdf.Data.IPage item)
        {
            lock (_pages)
            lock (_images)
            lock (_requests)
            {
                DeleteRequest(index);
                _pages.Insert(index, item);
                _images.Insert(index, new Drawing.ImageContainer());
                UpdateImageText(index);
                UpdateHistory(ListViewCommands.Insert, new KeyValuePair<int, CubePdf.Data.IPage>(index, item));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された IDocumentReader オブジェクトの各ページを index
        /// の位置に挿入します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert(int index, CubePdf.Data.IDocumentReader reader)
        {
            try
            {
                BeginCommand();
                CreateEngine(reader);
                lock (_requests) DeleteRequest(index);
                lock (_pages)
                lock (_images)
                {
                    var first = index;
                    foreach (var page in reader.Pages)
                    {
                        _pages.Insert(index, page);
                        _images.Insert(index, new Drawing.ImageContainer());
                        UpdateHistory(ListViewCommands.Insert, new KeyValuePair<int, CubePdf.Data.IPage>(index, page));
                        ++index;
                    }
                    UpdateImageText(first);
                }
            }
            finally { EndCommand(); }
        }

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
        public void Insert(int index, string path, string password = "")
        {
            using (var reader = new CubePdf.Editing.DocumentReader(path, password))
            {
                Insert(index, reader);
            }
        }

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
        public void Extract(IList<CubePdf.Data.IPage> pages, string path)
        {
            var binder = new CubePdf.Editing.PageBinder();
            foreach (var page in pages) binder.Pages.Add(page);
            binder.Save(path);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Extract
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトい対応する各 PDF ページを新しい
        /// PDF ファイルとして path に保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Extract(IList items, string path)
        {
            IList<CubePdf.Data.IPage> list = new List<CubePdf.Data.IPage>();
            foreach (var item in items)
            {
                var page = ToPage(item);
                if (page != null) list.Add(page);
            }
            Extract(list, path);
        }

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
        public  void Split(IList<CubePdf.Data.IPage> pages, string directory) { foreach (var page in pages) Split(_pages.IndexOf(page), directory); }
        public  void Split(IList items, string directory) { foreach (var obj in items) Split(_images.IndexOf(obj as CubePdf.Drawing.ImageContainer), directory); }
        private void Split(int index, string directory)
        {
            if (index < 0 || index >= _pages.Count) return;

            var page = _pages[index];
            var binder = new CubePdf.Editing.PageBinder();
            binder.Pages.Add(page);

            var format = String.Format("{{0}}-{{1:D{0}}}.pdf", _pages.Count.ToString().Length);
            var filename = String.Format(format, System.IO.Path.GetFileNameWithoutExtension(_path), index + 1);
            var dest = System.IO.Path.Combine(directory, filename);
            binder.Save(dest);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトに対応する PDF ページを削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Remove(object item) { RemoveAt(_images.IndexOf(item as CubePdf.Drawing.ImageContainer)); }
        public void Remove(CubePdf.Data.IPage item) { RemoveAt(_pages.IndexOf(item)); }

        /* ----------------------------------------------------------------- */
        ///
        /// RemoveAt
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイルに相当する
        /// PDF ページを削除します。
        /// 
        /// TODO: リクエストの調整
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _pages.Count) return;

            lock (_pages)
            lock (_images)
            lock (_requests)
            {
                var page = _pages[index];
                _pages.RemoveAt(index);
                var image = _images.RawAt(index);
                _images.RemoveAt(index);
                if (image != null) image.Dispose();
                DeleteRequest(index);
                UpdateImageText(index);
                UpdateHistory(ListViewCommands.Remove, new KeyValuePair<int, CubePdf.Data.IPage>(index, page));
            }
        }

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
        public void Move(int oldindex, int newindex)
        {
            if (oldindex < 0 || oldindex >= _pages.Count ||
                newindex < 0 || newindex >= _pages.Count || oldindex == newindex) return;

            lock (_pages)
            lock (_images)
            {
                var page = _pages[oldindex];
                _pages.RemoveAt(oldindex);
                _pages.Insert(newindex, page);
                _images.Move(oldindex, newindex);

                if (oldindex <= newindex) UpdateImageText(oldindex, newindex);
                else UpdateImageText(newindex, oldindex);
                UpdateHistory(ListViewCommands.Move, new KeyValuePair<int, int>(oldindex, newindex));
            }
        }

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
        public void Rotate(object item, int degree) { RotateAt(_images.IndexOf(item as CubePdf.Drawing.ImageContainer), degree); }
        public void Rotate(CubePdf.Data.IPage item, int degree) { RotateAt(_pages.IndexOf(item), degree); }

        /* ----------------------------------------------------------------- */
        ///
        /// RotateAt
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイルに相当する
        /// PDF ページを degree 度回転させます。角度は、現在表示されている
        /// 画像に対する相対度数で指定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RotateAt(int index, int degree)
        {
            if (index < 0 || index >= _pages.Count) return;

            var item = new CubePdf.Data.Page(_pages[index]);
            item.Rotation += degree;
            if (item.Rotation < 0) item.Rotation += 360;
            if (item.Rotation >= 360) item.Rotation -= 360;
            var image = _engines[item.FilePath].CreateImage(item.PageNumber, GetPower(item));
            if (image == null) return;

            var delta = item.Rotation - _engines[item.FilePath].GetPage(item.PageNumber).Rotation;
            if (delta < 0) delta += 360;
            if (delta >= 360) delta -= 360;

            RotateImage(image, delta);
            _images.RawAt(index).UpdateImage(image, Drawing.ImageStatus.Created);
            _pages[index] = item;
            UpdateHistory(ListViewCommands.Rotate, new KeyValuePair<int, int>(index, degree));
        }

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
        public void BeginCommand() { _status = CommandStatus.Begin; }

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
        public void EndCommand() { _status = CommandStatus.End; }

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        /// 
        /// <summary>
        /// 直前の操作を取り消します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Undo()
        {
            if (_undo.Count == 0) return;

            try
            {
                _undostatus = UndoStatus.Undo;
                var element = _undo[0];
                _undo.Remove(element);
                if (element.Command == ListViewCommands.Insert) UndoInsert(element.Parameters);
                else if (element.Command == ListViewCommands.Remove) UndoRemove(element.Parameters);
                else if (element.Command == ListViewCommands.Move) UndoMove(element.Parameters);
                else if (element.Command == ListViewCommands.Rotate) UndoRotate(element.Parameters);
                else if (element.Command == ListViewCommands.Metadata) UndoMetadata(element.Parameters);
                else if (element.Command == ListViewCommands.Encryption) UndoEncryption(element.Parameters);
            }
            finally { _undostatus = UndoStatus.Normal; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Redo
        /// 
        /// <summary>
        /// 取り消した操作を再実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Redo()
        {
            if (_redo.Count == 0) return;

            try
            {
                _undostatus = UndoStatus.Redo;
                var element = _redo[0];
                _redo.Remove(element);
                if (element.Command == ListViewCommands.Insert) UndoInsert(element.Parameters);
                else if (element.Command == ListViewCommands.Remove) UndoRemove(element.Parameters);
                else if (element.Command == ListViewCommands.Move) UndoMove(element.Parameters);
                else if (element.Command == ListViewCommands.Rotate) UndoRotate(element.Parameters);
                else if (element.Command == ListViewCommands.Metadata) UndoMetadata(element.Parameters);
                else if (element.Command == ListViewCommands.Encryption) UndoEncryption(element.Parameters);
            }
            finally { _undostatus = UndoStatus.Normal; }
        }

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
        public Image PreviewImage(int index, Size bound)
        {
            if (index < 0 || index >= _pages.Count) return null;

            var page = _pages[index];
            var horizontal = bound.Width / (double)page.ViewSize.Width;
            var vertical = bound.Height / (double)page.ViewSize.Height;
            var power = (horizontal < vertical) ? horizontal : vertical;

            lock (_engines)
            {
                var engine = _engines[page.FilePath];
                return engine.CreateImage(page.PageNumber, power);
            }
        }

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
        public int IndexOf(object item) { return IndexOf(item as CubePdf.Drawing.ImageContainer); }
        public int IndexOf(CubePdf.Drawing.ImageContainer item) { return _images.IndexOf(item); }
        public int IndexOf(CubePdf.Data.IPage page) { return _pages.IndexOf(page); }

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
        public CubePdf.Data.IPage ToPage(object item)
        {
            var image = item as CubePdf.Drawing.ImageContainer;
            if (image == null) return null;

            var index = _images.IndexOf(image);
            return (index >= 0 && index < _pages.Count) ? _pages[index] : null;
        }

        #endregion

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// IDocumentReader
        /// 
        /// <summary>
        /// IDocumentReader インターフェースを実装します。Metadata プロパティ
        /// に関しては、IDocumentWriter インターフェースのものを優先します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Implementations for IDocumentReader

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// パスを取得します（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Password
        /// 
        /// <summary>
        /// PDF ファイルを開く際に指定されたパスワードを取得します。
        /// 指定されたパスワードがオーナパスワードなのかユーザパスワード
        /// なのかの判断については、EncryptionStatus の情報から判断
        /// します（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Password
        {
            get { return _password; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// PageCount
        /// 
        /// <summary>
        /// 現在、開いている（または各種操作を行った結果の）PDF ファイルに
        /// 含まれるページ数を取得します（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int PageCount
        {
            get { return _pages.Count; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルの文書プロパティを取得します（IDocumentReader から
        /// 継承されます）。IListViewModel インターフェースでは、
        /// IDocumentWriter.Metadata プロパティが優先されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        CubePdf.Data.IMetadata CubePdf.Data.IDocumentReader.Metadata
        {
            get { return _metadata; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionStatus
        /// 
        /// <summary>
        /// 暗号化されている PDF ファイルへのアクセス（許可）状態を
        /// 取得します（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.EncryptionStatus EncryptionStatus
        {
            get { return _source_status; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionMethod
        /// 
        /// <summary>
        /// 暗号化方式を取得します（IDocumentReader から継承されます）。
        /// EncryptionMethod プロパティでは、常に、Open メソッドで開いた
        /// PDF ファイルの元々の暗号化方式が取得されます。PDF ファイルを
        /// 保存する際に暗号化方式を変更したい場合は、Encryption プロパティ
        /// で設定して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.EncryptionMethod EncryptionMethod
        {
            get { return _source_method; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Permission
        /// 
        /// <summary>
        /// PDF ファイルに設定されている各種操作の権限に関する情報を取得
        /// します（IDocumentReader から継承されます）。Permission プロパティ
        /// では、常に、Open メソッドで開いた PDF ファイルの元々の各種操作
        /// 権限情報が取得されます。PDF ファイルを保存する際に各種操作権限を
        /// 変更したい場合は、Encrytpion プロパティで設定して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IPermission Permission
        {
            get { return _source_permission; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF の各ページ情報へアクセスするための反復子を取得します
        /// （IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<CubePdf.Data.IPage> Pages
        {
            get { return _pages; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// 引数に指定された IDocumentReader オブジェクトからページ情報を
        /// 読み込んで、ListView へ表示可能な状態にします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(CubePdf.Data.IDocumentReader reader)
        {
            if (_pages.Count > 0) Close();

            // Properties for IDocumentReader
            _path = reader.FilePath;
            _password = reader.Password;
            _metadata = reader.Metadata;
            _source_status = reader.EncryptionStatus;
            _source_method = reader.EncryptionMethod;
            _source_permission = reader.Permission;
            _pages.Capacity = reader.PageCount + 1;

            CreateEngine(reader);
            Add(reader);

            // Properties for IDocumentWriter
            var encrypt = new CubePdf.Data.Encryption();
            encrypt.Method = _source_method;
            encrypt.Permission = new CubePdf.Data.Permission(_source_permission);
            _encrypt = encrypt;

            // Properties for others
            _undo.Clear();
            _redo.Clear();
            _modified = false;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルを開き、画面に表示可能な状態にする
        /// ための準備を行います（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(string path, string password = "")
        {
            if (_pages.Count > 0) Close();
            using (var reader = new CubePdf.Editing.DocumentReader(path, password))
            {
                Open(reader);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在開いている PDF ファイルを閉じます（IDocumentReader から
        /// 継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Close()
        {
            _modified = false;
            _path = string.Empty;
            _password = string.Empty;
            _metadata = null;
            _source_status = Data.EncryptionStatus.NotEncrypted;
            _source_method = Data.EncryptionMethod.Unknown;
            _source_permission = null;
            _encrypt = null;
            _undo.Clear();
            _redo.Clear();

            lock (_pages) _pages.Clear();
            lock (_requests) _requests.Clear();
            lock (_images)
            {
                ClearImage();
                _images.Clear();
            }
            DisposeEngine();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPage
        /// 
        /// <summary>
        /// 指定されたページ番号に対応するページ情報を取得します
        /// （IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IPage GetPage(int pagenum)
        {
            return _pages[pagenum - 1];
        }

        #endregion

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// IDocumentWriter
        /// 
        /// <summary>
        /// IDocumentWriter インターフェースを実装します。Pages プロパティに
        /// 関しては、IDocumentReader インターフェースのものを優先します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Implementations for IDocumentWriter

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルの文書プロパティを取得、または設定します
        /// （IDocumentWriter から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IMetadata Metadata
        {
            get { return _metadata; }
            set
            {
                UpdateHistory(ListViewCommands.Metadata, _metadata);
                _metadata = value;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// PDF ファイルのセキュリティに関する情報を取得します
        /// （IDocumentWriter から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IEncryption Encryption
        {
            get { return _encrypt; }
            set
            {
                UpdateHistory(ListViewCommands.Encryption, _encrypt);
                _encrypt = value;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページ情報を取得、または設定します
        /// （IDocumentWriter から継承されます）。
        /// IListViewModel インターフェースでは、IDocumentReader.Pages
        /// プロパティが優先されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ICollection<CubePdf.Data.IPage> CubePdf.Data.IDocumentWriter.Pages
        {
            get { return _pages; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        /// 
        /// <summary>
        /// 初期状態にリセットします（IDocumentWriter から継承されます）。
        /// 表示に関わるオブジェクトがクリアされます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Reset()
        {
            lock (_images)
            lock (_requests)
            {
                ClearImage();
                _requests.Clear();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルに保存します（IDocumentWriter から
        /// 継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save(string path)
        {
            var binder = new CubePdf.Editing.PageBinder();
            SaveDocument(path, binder);
            RestructDocument(path, binder);
        }

        #endregion

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// IItemsProvider(Image)
        /// 
        /// <summary>
        /// IItemsProvider インターフェースの実装を行います。
        /// ListViewModel クラスでは仮想化は行わないので、全てのメソッドで
        /// NotImplementationException が送出されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Implementations for IItemsProvider<Image>

        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItemsCount
        /// 
        /// <summary>
        /// Items の項目数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ProvideItemsCount()
        {
            return _pages.Count;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItem
        /// 
        /// <summary>
        /// 指定されたインデックスに対応する項目を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Drawing.ImageContainer ProvideItem(int index)
        {
            if (index < 0 || index >= _images.RawCount) return null;
            lock (_images)
            {
                var element = _images.RawAt(index);
                if (element.Status == Drawing.ImageStatus.Created) return element;
                if (element.Status == Drawing.ImageStatus.None) element.UpdateImage(GetDummyImage(_pages[index]), Drawing.ImageStatus.Dummy);
                if (element.Status == Drawing.ImageStatus.Dummy)
                {
                    UpdateRequest(index, _pages[index]);
                    FetchRequest();
                }
                return element;
            }
        }

        #endregion

        #region Event handlers for BitmapEngine

        /* ----------------------------------------------------------------- */
        ///
        /// BitmapEngine_ImageCreated
        /// 
        /// <summary>
        /// ListViewModel クラスが保持している BitmapEngine オブジェクトの
        /// いずれかが、画像生成を終了した時に発生するイベントのハンドラ
        /// です。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void BitmapEngine_ImageCreated(object sender, CubePdf.Drawing.ImageEventArgs e)
        {
            var index = _pages.IndexOf((CubePdf.Data.Page)e.Page); // TODO: 修正
            if (e.Image != null && index >= 0)
            {
                lock (_images)
                {
                    _images.RawAt(index).UpdateImage(e.Image, Drawing.ImageStatus.Created);
                    Debug.WriteLine(String.Format("Created[{0}] => {1}", index, e.Page.ToString()));
                }
            }
            FetchRequest();
        }

        #endregion

        #region Private methods for data access and converting

        /* ----------------------------------------------------------------- */
        ///
        /// GetRunningEngine
        /// 
        /// <summary>
        /// 非同期でPDF のページ画像を生成している BitmapEngine が存在する
        /// かどうかを判断します。存在する場合は、最初に見つかった
        /// BitmapEngine オブジェクトを、見つからなかった場合は null を
        /// 返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Drawing.BitmapEngine GetRunningEngine()
        {
            foreach (var engine in _engines.Values)
            {
                if (engine.UnderImageCreation) return engine;
            }
            return null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPower
        /// 
        /// <summary>
        /// Size(ItemWidth, ItemWidth) の正方形に収まるような最大倍率を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private double GetPower(CubePdf.Data.IPage page)
        {
            var horizontal = _width / (double)page.ViewSize.Width;
            var vertical = _width / (double)page.ViewSize.Height;
            return (horizontal < vertical) ? horizontal : vertical;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetDummyImage
        /// 
        /// <summary>
        /// ListView に本来表示される画像と縦横比の等しいダミー画像を取得
        /// します。生成された画像の縦横比は、小数点以下の関係で若干ずれる
        /// 事があります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Image GetDummyImage(CubePdf.Data.IPage page)
        {
            var width  = page.ViewSize.Width;
            var height = page.ViewSize.Height;
            var narrow = Math.Min(Math.Min(10, width), height);
            return (width < height) ?
                new Bitmap(narrow, (int)(narrow * height / (double)width)) :
                new Bitmap((int)(narrow * width / (double)height), narrow);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetCommandText
        /// 
        /// <summary>
        /// 各コマンドを説明するテキストを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetCommandText(CubePdf.Wpf.CommandElement element)
        {
            if (element.Command == ListViewCommands.Insert) return Properties.Resources.InsertText;
            else if (element.Command == ListViewCommands.Remove) return Properties.Resources.RemoveText;
            else if (element.Command == ListViewCommands.Move) return Properties.Resources.MoveText;
            else if (element.Command == ListViewCommands.Rotate) return Properties.Resources.RotateText;
            else if (element.Command == ListViewCommands.Metadata) return Properties.Resources.MetadataText;
            else if (element.Command == ListViewCommands.Encryption) return Properties.Resources.EncryptionText;
            return string.Empty;
        }

        #endregion

        #region Private methods for documents

        /* ----------------------------------------------------------------- */
        ///
        /// SaveDocument
        /// 
        /// <summary>
        /// 現在の構成で指定されたパスに保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void SaveDocument(string path, CubePdf.Editing.PageBinder binder)
        {
            lock (_pages)
            {
                foreach (var page in _pages) binder.Pages.Add(page);
                binder.Metadata = Metadata;
                binder.Encryption = Encryption;

                var tmp = System.IO.Path.GetTempFileName();
                binder.Save(tmp);
                CubePdf.Data.FileIOWrapper.Move(tmp, path);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RestructDocument
        /// 
        /// <summary>
        /// 保存された内容でオブジェクトを再構成します。サムネイル用の
        /// イメージは保存前のものがそのまま利用できるので、それ以外の
        /// 情報を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RestructDocument(string path, CubePdf.Editing.PageBinder binder)
        {
            _path = path;
            _password = binder.Encryption.OwnerPassword;
            _metadata = binder.Metadata;
            _source_method = binder.Encryption.Method;
            _source_permission = binder.Encryption.Permission;
            
            DisposeEngine();
            lock (_requests) _requests.Clear();
            lock (_pages)
            {
                using (var reader = new CubePdf.Editing.DocumentReader(_path, _password))
                {
                    _source_status = reader.EncryptionStatus;
                    CreateEngine(reader);
                    var index = 0;
                    foreach (var page in reader.Pages) _pages[index++] = page;
                }
            }

            _undo.Clear();
            _redo.Clear();
            _modified = false;
        }

        #endregion

        #region Private methods for engines

        /* ----------------------------------------------------------------- */
        ///
        /// CreateEngine
        /// 
        /// <summary>
        /// 新しい BitmapEngine オブジェクトを生成してエンジン一覧に登録
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Drawing.BitmapEngine CreateEngine(string path, string password)
        {
            if (_engines.ContainsKey(path)) return _engines[path];
            var engine = new CubePdf.Drawing.BitmapEngine();
            engine.Open(path, password);
            return RegisterEngine(path, engine);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateEngine
        /// 
        /// <summary>
        /// 新しい BitmapEngine オブジェクトを生成してエンジン一覧に登録
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Drawing.BitmapEngine CreateEngine(CubePdf.Data.IDocumentReader reader)
        {
            if (_engines.ContainsKey(reader.FilePath)) return _engines[reader.FilePath];
            var engine = new CubePdf.Drawing.BitmapEngine();
            engine.Open(reader);
            return RegisterEngine(reader.FilePath, engine);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RegisterEngine
        /// 
        /// <summary>
        /// 新しい BitmapEngine オブジェクトをエンジン一覧に登録します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Drawing.BitmapEngine RegisterEngine(string path, CubePdf.Drawing.BitmapEngine engine)
        {
            engine.ImageCreated -= new CubePdf.Drawing.ImageEventHandler(BitmapEngine_ImageCreated);
            engine.ImageCreated += new CubePdf.Drawing.ImageEventHandler(BitmapEngine_ImageCreated);
            lock (_engines) _engines.Add(path, engine);
            return engine;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// DisposeEngine
        /// 
        /// <summary>
        /// 現在、保持している BitmapEngine オブジェクトを全て削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void DisposeEngine()
        {
            lock (_engines)
            {
                foreach (var engine in _engines.Values)
                {
                    engine.ImageCreated -= new CubePdf.Drawing.ImageEventHandler(BitmapEngine_ImageCreated);
                    engine.Dispose();
                }
                _engines.Clear();
            }
        }

        #endregion

        #region Private methods for images

        /* ----------------------------------------------------------------- */
        ///
        /// ClearImage
        /// 
        /// <summary>
        /// 現在、保持している全ての Image オブジェクトを解放します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ClearImage()
        {
            lock (_images)
            {
                for (int i = 0; i < _images.RawCount; ++i) _images.RawAt(i).DeleteImage();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateImageText
        /// 
        /// <summary>
        /// [first, last] の範囲にある ImageContainer の Text プロパティを
        /// 更新します。Text プロパティには、ページ番号を表す文字列が設定
        /// されます。last が省略された（または、-1 が指定された）場合は、
        /// [first, Items.Count) までの Text プロパティを更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateImageText(int first, int last = -1)
        {
            if (last == -1) last = _images.RawCount - 1;
            for (int i = first; i <= last; ++i) _images.RawAt(i).Text = (i + 1).ToString();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RotateImage
        /// 
        /// <summary>
        /// 引数に指定された image を degree 度だけ回転させます。
        /// 
        /// NOTE: System.Drawing.Image.RotateFlip メソッドは 90 度単位でしか
        /// 回転させる事ができないので、引数に指定された回転度数を 90 度単位
        /// で丸めています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RotateImage(System.Drawing.Image image, int degree)
        {
            var value = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            if (degree >= 90 && degree < 180) value = System.Drawing.RotateFlipType.Rotate90FlipNone;
            else if (degree >= 180 && degree < 270) value = System.Drawing.RotateFlipType.Rotate180FlipNone;
            else if (degree >= 270 && degree < 360) value = System.Drawing.RotateFlipType.Rotate270FlipNone;
            image.RotateFlip(value);
        }

        #endregion

        #region Private methods for requests

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateRequest
        /// 
        /// <summary>
        /// 引数に指定されたインデックスをリクエストキューに追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateRequest(int index, CubePdf.Data.IPage page)
        {
            if (_images.RawAt(index).Status == Drawing.ImageStatus.Created) return;

            lock (_requests)
            {
                if (_visibility == ListViewItemVisibility.Minimum)
                {
                    _requests.Clear();
                    return;
                }

                if (!_requests.ContainsKey(index)) _requests.Add(index, page);
                else _requests[index] = page;
                Debug.WriteLine(String.Format("Register[{0}] => {1}", index, page.ToString()));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// DeleteRequest
        /// 
        /// <summary>
        /// 引数に指定された範囲のリクエストを削除します。第 2 引数が省略
        /// された場合、第 1 引数で指定されたインデックスから最後までを
        /// 対象範囲とします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void DeleteRequest(int first, int last = -1)
        {
            lock (_requests)
            {
                if (last == -1) last = _pages.Count - 1;
                for (int i = first; i <= last; ++i)
                {
                    if (_requests.ContainsKey(i)) _requests.Remove(i);
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FetchRequest
        /// 
        /// <summary>
        /// リクエストキューに格納されている先頭のリクエストを実行します。
        /// 同時に行う処理は 1 つまでです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void FetchRequest()
        {
            if (UnderItemCreation) return;

            lock (_requests)
            {
                if (_visibility == ListViewItemVisibility.Minimum)
                {
                    _requests.Clear();
                    return;
                }

                while (_requests.Count > 0)
                {
                    var key = _requests.Keys[0];
                    var value = _requests[key];
                    _requests.Remove(key);
                    if (key < 0 || key >= _pages.Count || _images.RawAt(key).Status == Drawing.ImageStatus.Created ||
                        value.FilePath != _pages[key].FilePath || value.PageNumber != _pages[key].PageNumber)
                    {
                        Debug.WriteLine(String.Format("Skip[{0}] => {1}", key, value.ToString()));
                        continue;
                    }
                    Debug.WriteLine(String.Format("Fetch[{0}] => {1}", key, value.ToString()));
                    _engines[value.FilePath].CreateImageAsync(value.PageNumber, GetPower(value));
                    break;
                }
            }
        }

        #endregion

        #region Private methods for Undo/Redo

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateHistory
        /// 
        /// <summary>
        /// 履歴を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateHistory(ICommand command, object parameter) { UpdateHistory(command, new ArrayList() { parameter }); }
        private void UpdateHistory(ICommand command, IList parameters)
        {
            var history = (_undostatus == UndoStatus.Undo) ? _redo : _undo;
            if (_status != CommandStatus.Continue) history.Insert(0, new CommandElement(command));
            var element = history[0];
            if (command != element.Command) throw new ArgumentException(Properties.Resources.HistoryCommandException);
            foreach (var param in parameters) element.Parameters.Add(param);
            element.Text = GetCommandText(element);
            if (_status == CommandStatus.Begin) _status = CommandStatus.Continue;
            if (_undo.Count > _maxundo) _undo.RemoveAt(_undo.Count - 1);
            if (_undostatus == UndoStatus.Normal) _redo.Clear();
            _modified = true;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoInsert
        ///
        /// <summary>
        /// 挿入操作を取り消します。
        /// パラメータ (parameters) は、インデックスと挿入された PDF ページ
        /// オブジェクトのペア (KeyValuePair(int, CubePdf.Data.Page))
        /// オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoInsert(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                for (int i = parameters.Count - 1; i >= 0; --i)
                {
                    var param = (KeyValuePair<int, CubePdf.Data.IPage>)parameters[i];
                    RemoveAt(param.Key);
                }
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoRemove
        ///
        /// <summary>
        /// 削除操作を取り消します。
        /// パラメータ (parameters) は、インデックスと削除された PDF ページ
        /// オブジェクトのペア (KeyValuePair(int, CubePdf.Data.Page))
        /// オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoRemove(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                for (int i = parameters.Count - 1; i >= 0; --i)
                {
                    var param = (KeyValuePair<int, CubePdf.Data.IPage>)parameters[i];
                    Insert(param.Key, param.Value);
                }
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoMove
        ///
        /// <summary>
        /// 移動操作を取り消します。
        /// パラメータ (parameters) は、移動元/移動先のインデックスのペア
        /// (KeyValuePair(int, int)) オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoMove(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                for (int i = parameters.Count - 1; i >= 0; --i)
                {
                    var param = (KeyValuePair<int, int>)parameters[i];
                    Move(param.Value, param.Key);
                }
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoRotate
        ///
        /// <summary>
        /// 回転操作を取り消します。
        /// パラメータ (parameters) は、インデックスと回転度数のペア
        /// (KeyValuePair(int, int)) オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoRotate(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                foreach (KeyValuePair<int, int> param in parameters) RotateAt(param.Key, -param.Value);
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoMetadata
        ///
        /// <summary>
        /// Metadata の変更を取り消します。
        /// パラメータ (parameters) の数は 1 つのみで、変更前の Metadata
        /// オブジェクトが格納されています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoMetadata(IList parameters)
        {
            if (parameters == null) return;
            var metadata = parameters[0] as CubePdf.Data.IMetadata;
            if (metadata == null) return;

            Metadata = metadata;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoEncryption
        ///
        /// <summary>
        /// Encryption の変更を取り消します。
        /// パラメータ (parameters) の数は 1 つのみで、変更前の Encryption
        /// オブジェクトが格納されています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoEncryption(IList parameters)
        {
            if (parameters == null) return;
            var encrypt = parameters[0] as CubePdf.Data.IEncryption;
            if (encrypt == null) return;

            Encryption = encrypt;
        }

        #endregion

        #region Internal classes

        /* ----------------------------------------------------------------- */
        ///
        /// CommandStatus
        ///
        /// <summary>
        /// 処理の状態を判別するための Enum 型です。主に、BeginCommand(),
        /// EndCommand() を判別するために使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        internal enum CommandStatus { Begin, Continue, End }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoStatus
        /// 
        /// <summary>
        /// 実行されたコマンドが通常の処理なのか Undo/Redo なのかを判別する
        /// ために使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        internal enum UndoStatus { Normal, Undo, Redo }

        #endregion

        #region Variables

        #region Implementations for IDocumentReader
        private string _path = string.Empty;
        private string _password = string.Empty;
        private CubePdf.Data.IMetadata _metadata = null;
        private CubePdf.Data.EncryptionStatus _source_status = Data.EncryptionStatus.NotEncrypted;
        private CubePdf.Data.EncryptionMethod _source_method = Data.EncryptionMethod.Unknown;
        private CubePdf.Data.IPermission _source_permission = new CubePdf.Data.Permission();
        private List<CubePdf.Data.IPage> _pages = new List<CubePdf.Data.IPage>();
        #endregion

        #region Implementations for IDocumentWriter
        private CubePdf.Data.IEncryption _encrypt = null;
        #endregion

        #region Others
        private bool _disposed = false;
        private int _width = 0;
        private int _maxundo = 30;
        private bool _modified = false;
        private ListViewItemVisibility _visibility = ListViewItemVisibility.Normal;
        private IListProxy<CubePdf.Drawing.ImageContainer> _images = null;
        private SortedList<string, CubePdf.Drawing.BitmapEngine> _engines = new SortedList<string, CubePdf.Drawing.BitmapEngine>();
        private SortedList<int, CubePdf.Data.IPage> _requests = new SortedList<int, CubePdf.Data.IPage>();
        private CommandStatus _status = CommandStatus.End;
        private UndoStatus _undostatus = UndoStatus.Normal;
        private ObservableCollection<CommandElement> _undo = new ObservableCollection<CommandElement>();
        private ObservableCollection<CommandElement> _redo = new ObservableCollection<CommandElement>();
        #endregion

        #endregion
    }
}
