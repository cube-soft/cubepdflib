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
    /// データ等を管理するクラスです。IListViewModel は、ListView の
    /// VirtualizingStackPanel による仮想化を考慮したインターフェースと
    /// なっていますが、ListViewModel クラスでは仮想化は考慮していません。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ListViewModel : IListViewModel
    {
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
        /// FilePath
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// パスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// ファイルサイズを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public long FileSize
        {
            get { return _size; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreationTime
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// 作成日時を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime CreationTime
        {
            get { return _create; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateTime
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// 更新日時を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime UpdateTime
        {
            get { return _update; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// AccessTime
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// アクセス日時を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime AccessTime
        {
            get { return _access; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルの文書プロパティを取得、または設定します。
        /// 
        /// NOTE: Metadata のメンバのみを変更する場合、その変更は履歴には
        /// 残りません。履歴に残す場合、Metadata オブジェクト自体を変更
        /// して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Metadata Metadata
        {
            get { return _meta; }
            set
            {
                UpdateHistory(ListViewCommands.Metadata, _meta);
                _meta = value;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// PDF ファイルのセキュリティに関する情報を取得します。
        /// 
        /// NOTE: Encryption のメンバのみを変更する場合、その変更は履歴には
        /// 残りません。履歴に残す場合、Encryption オブジェクト自体を変更
        /// して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Encryption Encryption
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
        /// EncryptionStatus
        /// 
        /// <summary>
        /// 暗号化されている PDF ファイルへのアクセス（許可）状態を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.EncryptionStatus EncryptionStatus
        {
            get { return _encrypt_status; }
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
        /// ItemCount
        /// 
        /// <summary>
        /// 現在、開いている（または各種操作を行った結果の）PDF ファイルに
        /// 含まれるページ数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ItemCount
        {
            get { return _pages.Count; }
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
        public ObservableCollection<CubePdf.Drawing.ImageContainer> Items
        {
            get { return _images; }
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
        /// Open
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルを開き、画面に表示可能な状態にする
        /// ための準備を行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(string path, string password = "")
        {
            if (_pages.Count > 0) Close();
            using (var reader = new CubePdf.Editing.DocumentReader(path, password))
            {
                var engine = CreateEngine(reader);
                foreach (var page in reader.Pages) Add(new CubePdf.Data.Page(page));
                _modified = false;
                _path   = path;
                _size   = reader.FileSize;
                _create = reader.CreationTime;
                _update = reader.UpdateTime;
                _access = reader.AccessTime;
                _meta   = new Data.Metadata(reader.Metadata);
                _encrypt  = new Data.Encryption();
                _encrypt.Method = reader.EncryptionMethod;
                _encrypt.Permission = new Data.Permission(reader.Permission);
                _encrypt_status = reader.EncryptionStatus;
                _undo.Clear();
                _redo.Clear();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在開いている PDF ファイルを閉じます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Close()
        {
            _modified = false;
            _path  = string.Empty;
            _size  = 0;
            _meta  = null;
            _encrypt = null;
            _encrypt_status = Data.EncryptionStatus.NotEncrypted;
            _undo.Clear();
            _redo.Clear();

            lock (_pages) _pages.Clear();
            lock (_engines)
            {
                foreach (var engine in _engines.Values) engine.Dispose();
                _engines.Clear();
            }
            lock (_requests) _requests.Clear();
            lock (_images)
            {
                foreach (var image in _images) image.Dispose();
                _images.Clear();
            }
        }

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
        public void Save(string path = null)
        {
            lock (_engines)
            lock (_pages)
            lock (_requests)
            {
                var binder = new CubePdf.Editing.PageBinder();
                foreach (var page in _pages) binder.Pages.Add(page);
                binder.Metadata = Metadata;
                binder.Encryption = Encryption;

                var dest = String.IsNullOrEmpty(path) ? _path : path;
                var tmp = System.IO.Path.GetTempFileName();
                binder.Save(tmp);
                CubePdf.Data.FileIOWrapper.Move(tmp, dest);

                _modified = false;
                _path = dest;

                _requests.Clear();
                _pages.Clear();
                foreach (var engine in _engines.Values) engine.Dispose();
                _engines.Clear();

                using (var reader = new CubePdf.Editing.DocumentReader(dest, ""))
                {
                    var newengine = CreateEngine(reader);
                    foreach (var page in reader.Pages) _pages.Add(new CubePdf.Data.Page(page));
                }
            }
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
        public void Add(CubePdf.Data.Page item) { Insert(_pages.Count, item); }
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
        public void Insert(int index, CubePdf.Data.Page item)
        {
            lock (_pages)
            {
                _pages.Insert(index, item);
                UpdateHistory(ListViewCommands.Insert, new KeyValuePair<int, CubePdf.Data.Page>(index, item));
            }

            lock (_images)
            {
                _images.Insert(index, GetDummyItem(item));
                UpdateImageText(index);
            }

            lock (_requests)
            {
                UpdateRequest(index, item);
                FetchRequest();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページを index の位置に挿入
        /// します。
        /// 
        /// TODO: リクエストの調整。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert(int index, string path, string password = "")
        {
            try
            {
                BeginCommand();
                using (var reader = new CubePdf.Editing.DocumentReader(path, password))
                {
                    CreateEngine(reader);
                    foreach (var page in reader.Pages)
                    {
                        var item = new CubePdf.Data.Page(page);
                        Insert(index, item);
                        ++index;
                    }
                }
            }
            finally { EndCommand(); }
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
        public void Extract(IList<CubePdf.Data.Page> pages, string path)
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
            IList<CubePdf.Data.Page> list = new List<CubePdf.Data.Page>();
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
        public  void Split(IList<CubePdf.Data.Page> pages, string directory) { foreach (var page in pages) Split(_pages.IndexOf(page), directory); }
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
        public void Remove(CubePdf.Data.Page item) { RemoveAt(_pages.IndexOf(item)); }

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
            {
                var page = _pages[index];
                _pages.RemoveAt(index);
                var image = _images[index];
                _images.RemoveAt(index);
                if (image != null) image.Dispose();
                UpdateImageText(index);
                UpdateHistory(ListViewCommands.Remove, new KeyValuePair<int, CubePdf.Data.Page>(index, page));
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
            {
                var item = _pages[oldindex];
                _pages.RemoveAt(oldindex);
                _pages.Insert(newindex, item);
                UpdateHistory(ListViewCommands.Move, new KeyValuePair<int, int>(oldindex, newindex));
            }

            lock (_images)
            {
                _images.Move(oldindex, newindex);
                if (oldindex <= newindex) UpdateImageText(oldindex, newindex);
                else UpdateImageText(newindex, oldindex);
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
        public void Rotate(CubePdf.Data.Page item, int degree) { RotateAt(_pages.IndexOf(item), degree); }

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

            var item = _pages[index];
            item.Rotation += degree;
            if (item.Rotation < 0) item.Rotation += 360;
            if (item.Rotation >= 360) item.Rotation -= 360;
            var image = _engines[item.FilePath].CreateImage(item.PageNumber, GetPower(item));
            if (image == null) return;

            var delta = item.Rotation - _engines[item.FilePath].GetPage(item.PageNumber).Rotation;
            if (delta < 0) delta += 360;
            if (delta >= 360) delta -= 360;

            RotateImage(image, delta);
            _images[index].UpdateImage(image, Drawing.ImageStatus.Created);
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
        public int IndexOf(CubePdf.Data.Page page) { return _pages.IndexOf(page); }

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
        public CubePdf.Data.Page ToPage(object item) { return ToPage(item as CubePdf.Drawing.ImageContainer); }
        public CubePdf.Data.Page ToPage(CubePdf.Drawing.ImageContainer item) { return ToPage(_images.IndexOf(item)); }
        public CubePdf.Data.Page ToPage(int index) { return (index >= 0 && index < _pages.Count) ? _pages[index] : null; }

        #endregion

        #region Undo methods

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
                    var param = (KeyValuePair<int, CubePdf.Data.Page>)parameters[i];
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
                    var param = (KeyValuePair<int, CubePdf.Data.Page>)parameters[i];
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
            var metadata = parameters[0] as CubePdf.Data.Metadata;
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
            var encrypt = parameters[0] as CubePdf.Data.Encryption;
            if (encrypt == null) return;

            Encryption = encrypt;
        }

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

        public int ProvideItemsCount() { throw new NotImplementedException(); }
        public CubePdf.Drawing.ImageContainer ProvideItem(int index) { throw new NotImplementedException(); }

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
            var index = _pages.IndexOf((CubePdf.Data.Page)e.Page);
            if (e.Image != null && index >= 0)
            {
                lock (_images)
                {
                    _images[index].UpdateImage(e.Image, Drawing.ImageStatus.Created);
                    Debug.WriteLine(String.Format("Created[{0}] => {1}", index, e.Page.ToString()));
                }
            }
            FetchRequest();
        }

        #endregion

        #region Methods for data access and converting

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
        /// GetDummyItem
        /// 
        /// <summary>
        /// ListView に本来表示される画像とサイズのみ等しいダミー画像を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Drawing.ImageContainer GetDummyItem(CubePdf.Data.IPage page)
        {
            var power = GetPower(page);
            var width = (int)(page.ViewSize.Width * power);
            var height = (int)(page.ViewSize.Height * power);
            return new Drawing.ImageContainer(new Bitmap(width, height), Drawing.ImageStatus.Dummy);
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

        #region Methods for changing condition

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
        /// CreateEngine
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
            if (last == -1) last = _images.Count - 1;
            for (int i = first; i <= last; ++i) _images[i].Text = (i + 1).ToString();
        }

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
        /// UpdateRequest
        /// 
        /// <summary>
        /// 引数に指定されたインデックスをリクエストキューに追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateRequest(int index, CubePdf.Data.Page page)
        {
            lock (_requests)
            {
                if (!_requests.ContainsKey(index)) _requests.Add(index, page);
                else _requests[index] = page;
                Debug.WriteLine(String.Format("Register[{0}] => {1}", index, page.ToString()));
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

            lock (_pages)
            lock (_requests)
            {
                while (_requests.Count > 0)
                {
                    var key = _requests.Keys[0];
                    var value = _requests[key];
                    _requests.Remove(key);
                    if (key < 0 || key >= _pages.Count ||
                        value.FilePath != _pages[key].FilePath ||
                        value.PageNumber != _pages[key].PageNumber)
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
        private int _width = 0;
        private int _maxundo = 30;
        private bool _modified = false;
        private string _path = string.Empty;
        private long _size = 0;
        private DateTime _create = new DateTime();
        private DateTime _update = new DateTime();
        private DateTime _access = new DateTime();
        private CubePdf.Data.Metadata _meta = null;
        private CubePdf.Data.Encryption _encrypt = null;
        private CubePdf.Data.EncryptionStatus _encrypt_status = Data.EncryptionStatus.NotEncrypted;
        private List<CubePdf.Data.Page> _pages = new List<CubePdf.Data.Page>();
        private ObservableCollection<CubePdf.Drawing.ImageContainer> _images = new ObservableCollection<CubePdf.Drawing.ImageContainer>();
        private SortedList<string, CubePdf.Drawing.BitmapEngine> _engines = new SortedList<string, CubePdf.Drawing.BitmapEngine>();
        private SortedList<int, CubePdf.Data.Page> _requests = new SortedList<int, CubePdf.Data.Page>();
        private CommandStatus _status = CommandStatus.End;
        private UndoStatus _undostatus = UndoStatus.Normal;
        private ObservableCollection<CommandElement> _undo = new ObservableCollection<CommandElement>();
        private ObservableCollection<CommandElement> _redo = new ObservableCollection<CommandElement>();
        #endregion
    }
}
