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
        /// PDF ファイルの文書プロパティを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Metadata Metadata
        {
            get { return _meta; }
            set { _meta = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// PDF ファイルのセキュリティに関する情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Encryption Encryption
        {
            get { return _crypt; }
            set { _crypt = value; }
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
        public ObservableCollection<Image> Items
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
        public int MaxUndoCount
        {
            get { return _maxundo; }
            set { _maxundo = value; }
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
                foreach (var page in engine.Pages.Values) Add(new CubePdf.Data.Page(page));
                _path   = path;
                _size   = reader.FileSize;
                _create = reader.CreationTime;
                _update = reader.UpdateTime;
                _access = reader.AccessTime;
                _meta   = new Data.Metadata(reader.Metadata);
                _crypt  = new Data.Encryption();
                _crypt.Method = reader.EncryptionMethod;
                _crypt.Permission = new Data.Permission(reader.Permission);
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
            _path  = string.Empty;
            _size  = 0;
            _meta  = null;
            _crypt = null;

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

                _requests.Clear();
                _pages.Clear();
                foreach (var engine in _engines.Values) engine.Dispose();
                _engines.Clear();

                var dest = String.IsNullOrEmpty(path) ? _path : path;
                var tmp  = System.IO.Path.GetTempFileName();
                binder.Save(tmp);
                CubePdf.Data.FileIOWrapper.Move(tmp, dest);

                _path = dest;
                var newengine = CreateEngine(dest, "");
                foreach (var page in newengine.Pages.Values) _pages.Add(new CubePdf.Data.Page(page));
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
            lock (_pages) _pages.Insert(index, item);
            lock (_images) _images.Insert(index, GetDummyItem(item));
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
            if (_engines.ContainsKey(path)) throw new MultipleLoadException(Properties.Resources.MultipleLoadException, path);

            var engine = CreateEngine(path, password);
            foreach (var page in engine.Pages.Values)
            {
                var item = new CubePdf.Data.Page(page);
                Insert(index, item);
                ++index;
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
        public  void Split(IList items, string directory) { foreach (var obj in items) Split(_images.IndexOf(obj as Image), directory); }
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
        public void Remove(object item) { RemoveAt(_images.IndexOf(item as Image)); }
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
                _pages.RemoveAt(index);
                var image = _images[index];
                _images.RemoveAt(index);
                if (image != null) image.Dispose();
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
            }
            lock (_images) _images.Move(oldindex, newindex);
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
        public void Rotate(object item, int degree) { RotateAt(_images.IndexOf(item as Image), degree); }
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

            var delta = item.Rotation - _engines[item.FilePath].Pages[item.PageNumber].Rotation;
            if (delta < 0) delta += 360;
            if (delta >= 360) delta -= 360;

            RotateImage(image, delta);
            var prev = _images[index];
            _images[index] = image;
            if (prev != null) prev.Dispose();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        /// 
        /// <summary>
        /// 直前の操作を取り消します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Undo() { throw new NotImplementedException(); }

        /* ----------------------------------------------------------------- */
        ///
        /// Redo
        /// 
        /// <summary>
        /// 取り消した操作を再実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Redo() { throw new NotImplementedException(); }

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
        /// ToPage
        /// 
        /// <summary>
        /// ListView で表示されているサムネイルに対応する PDF ページの情報を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Page ToPage(object item) { return ToPage(item as Image); }
        public CubePdf.Data.Page ToPage(Image item)
        {
            var index = _images.IndexOf(item);
            return (index >= 0 && index < _pages.Count) ? _pages[index] : null;
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
        public Image ProvideItem(int index) { throw new NotImplementedException(); }

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
            var index = _pages.IndexOf(e.Page);
            if (e.Image != null && index >= 0)
            {
                lock (_images)
                {
                    var prev = _images[index];
                    _images[index] = e.Image;
                    if (prev != null) prev.Dispose();
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
        private double GetPower(CubePdf.Data.IReadOnlyPage page)
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
        private Image GetDummyItem(CubePdf.Data.IReadOnlyPage page)
        {
            var power = GetPower(page);
            var width = (int)(page.ViewSize.Width * power);
            var height = (int)(page.ViewSize.Height * power);
            return new Bitmap(width, height);
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

        #region Variables
        private int _width = 0;
        private int _maxundo = 0;
        private string _path = string.Empty;
        private long _size = 0;
        private DateTime _create = new DateTime();
        private DateTime _update = new DateTime();
        private DateTime _access = new DateTime();
        private CubePdf.Data.Metadata _meta = null;
        private CubePdf.Data.Encryption _crypt = null;
        private List<CubePdf.Data.Page> _pages = new List<CubePdf.Data.Page>();
        private ObservableCollection<Image> _images = new ObservableCollection<Image>();
        private SortedList<string, CubePdf.Drawing.BitmapEngine> _engines = new SortedList<string, CubePdf.Drawing.BitmapEngine>();
        private SortedList<int, CubePdf.Data.Page> _requests = new SortedList<int, CubePdf.Data.Page>();
        #endregion
    }
}
