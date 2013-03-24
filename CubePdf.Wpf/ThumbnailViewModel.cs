/* ------------------------------------------------------------------------- */
///
/// ThumbnailViewModel.cs
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ThumbnailViewModel
    /// 
    /// <summary>
    /// ListView に表示する PDF ファイルの各ページの情報、およびイメージ
    /// データ等を管理するクラスです。
    /// 
    /// TODO: _pages, _requests, _cache 辺りのロックを見直す。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ThumbnailViewModel : IItemsProvider<ImageSource>
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        /// constructor
        /* ----------------------------------------------------------------- */
        public ThumbnailViewModel()
        {
            _dummy = ToImageSource(new System.Drawing.Bitmap(1, 1));
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        /// ItemWidth
        /* ----------------------------------------------------------------- */
        public int ItemWidth
        {
            get { return _width; }
            set { _width = value; }
        }

        /* ----------------------------------------------------------------- */
        /// FilePath
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
        }

        /* ----------------------------------------------------------------- */
        /// ItemCount
        /* ----------------------------------------------------------------- */
        public int ItemCount
        {
            get { return _pages.Count; }
        }

        #endregion

        #region Public Methods

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
            if (_pages.Count > 0) this.Close();
            this.Add(path, password);
            _path = path;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        ///
        /// <summary>
        /// 現在、開いている PDF ファイルを閉じ、関連する表示されている
        /// 画像を全て消去します。
        /// 
        /// TODO: 表示されている画像は Freeze() メソッドを実行しないと
        /// メモリリークする恐れがあるのだが、Freeze() していると例外が
        /// 発生する事がある。要調査。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Close()
        {
            _pages.Clear();
            _requests.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            foreach (var engine in _engines.Values) engine.Dispose();
            _engines.Clear();
            //foreach (var image in _images.Values)
            //{
            //    if (image.Image != null && image.Image.CanFreeze) image.Image.Freeze();
            //}
            _images.Clear();
            _path = string.Empty;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルに保存します。引数 path を省略した
        /// 場合は、Open メソッドにより開いたファイルに対して上書き保存を
        /// 行います。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        public void Save(string path = null)
        {
            var dest = (path != null) ? path : _path;
            var binder = new CubePdf.Editing.PageBinder();
            foreach (var page in _pages) binder.Pages.Add(page);
            binder.Save(dest);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        ///
        /// <summary>
        /// ページ末尾に引数に指定された PDF ファイルの各ページを追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Add(string path, string password = "")
        {
            var engine = new CubePdf.Drawing.BitmapEngine();
            engine.Open(path, password);
            engine.ImageCreated -= new Drawing.ImageEventHandler(BitmapEngine_ImageCreated);
            engine.ImageCreated += new Drawing.ImageEventHandler(BitmapEngine_ImageCreated);
            _engines[engine.FilePath] = engine;
            foreach (var page in engine.Pages.Values)
            {
                _pages.Add(new CubePdf.Data.Page(page));
                _images.Add(this.GetIdentifier(page), new ImageContainer());
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        ///
        /// <summary>
        /// TODO: 不安定なので要調整。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Remove(object item)
        {
            //var src = item as ImageSource;
            //if (src == null) return;

            //int key = -1;
            //lock (_cache)
            //{
            //    foreach (var obj in _cache)
            //    {
            //        if (src != obj.Value) continue;
            //        key = obj.Key;
            //        break;
            //    }
            //}
            //if (key == -1) return;

            //lock (_requests)
            //{
            //    _requests.Clear();
            //    _pages.RemoveAt(key);
            //}

            //var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _cache[key], key);
            //OnCollectionChanged(e);
            //lock (_cache)
            //{
            //    if (_cache[key].CanFreeze) _cache[key].Freeze();
            //    _cache.Remove(key);
            //}
        }

        #endregion

        #region Implementations for IItemsProvider<ImageSource>

        /* ----------------------------------------------------------------- */
        /// ProvideItemsCount
        /* ----------------------------------------------------------------- */
        public int ProvideItemsCount()
        {
            return ItemCount;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItem
        ///
        /// <summary>
        /// IList の [] 演算子 (this[int index]) がコールされた時に実行
        /// されるメソッドです。表示する画像が用意されていない場合は、
        /// いったんサイズのみ同じなダミー画像を返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageSource ProvideItem(int index)
        {
            var page = _pages[index];
            var id = GetIdentifier(page);

            lock (_images)
            {
                var image = _images[id];
                if (image.Status == ImageStatus.Cached) return image.Image;
            }

            var power = GetPower(page);

            lock (_requests)
            {
                UpdateRequest(index, id);
                if (_requests.Count > 0 && UnderImageCreation() == null)
                {
                    _engines[page.FilePath].CreateImageAsync(page.PageNumber, power);
                }
            }

            var width = (int)(page.ViewSize.Width * power);
            var height = (int)(page.ViewSize.Height * power);
            var dummy = ToImageSource(new System.Drawing.Bitmap(width, height));

            lock (_images)
            {
                _images[id].Status = ImageStatus.Dummy;
                _images[id].Image = dummy;
            }

            return dummy;
        }

        #endregion

        #region Utility Methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetIdentifier
        ///
        /// <summary>
        /// PDF の各ページに対応する画像を管理するための識別を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetIdentifier(CubePdf.Data.IReadOnlyPage page)
        {
            return String.Format("{0}:{1}", page.FilePath, page.PageNumber);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetIndex
        ///
        /// <summary>
        /// 引数に指定された Page オブジェクトに対応するインデックスを
        /// 取得します。
        /// 
        /// TODO: ページ数が多くなってくるとパフォーマンスに影響するので、
        /// 何らかの方法を検討する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private int GetIndex(CubePdf.Data.IReadOnlyPage page)
        {
            for (int i = 0; i < _pages.Count; ++i)
            {
                if (page.FilePath == _pages[i].FilePath && page.PageNumber == _pages[i].PageNumber) return i;
            }
            return -1;
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
        /// UnderImageCreation
        /// 
        /// <summary>
        /// 非同期でPDF のページ画像を生成している BitmapEngine が存在する
        /// かどうかを判断します。存在する場合は、最初に見つかった
        /// BitmapEngine オブジェクトを見つからなかった場合は null を
        /// 返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Drawing.BitmapEngine UnderImageCreation()
        {
            foreach (var engine in _engines.Values)
            {
                if (engine.UnderImageCreation) return engine;
            }
            return null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToImageSource
        /// 
        /// <summary>
        /// System.Drawing.Image オブジェクトから
        /// System.Windows.Media.Imaging.ImageSource オブジェクトへの変換を
        /// 行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private ImageSource ToImageSource(System.Drawing.Image src)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                src.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var dest = new BitmapImage();
                dest.BeginInit();
                dest.StreamSource = new System.IO.MemoryStream(stream.ToArray());
                dest.EndInit();
                return dest;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateRequest
        /// 
        /// <summary>
        /// 引数に指定されたインデックスをリクエストキューに追加し、キューの
        /// 上限数チェック等の必要な更新処理を行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateRequest(int index, string id)
        {
            lock (_requests)
            {
                if (!_requests.ContainsKey(index)) _requests.Add(index, id);

                // TODO: リクエスト数の上限値に関する処理
            }
        }

        #endregion

        #region Event Handler for BitmapEngine

        /* ----------------------------------------------------------------- */
        ///
        /// BitmapEngine_ImageCreated
        ///
        /// <summary>
        /// TODO: NotifyCollectionChangedEventArgs に指定する olditem
        /// オブジェクトは、本当の olditem でなくても良いのかどうか要調査。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void BitmapEngine_ImageCreated(object sender, CubePdf.Drawing.ImageEventArgs e)
        {
            if (e.Image == null) return;

            var id = this.GetIdentifier(e.Page);
            var index = this.GetIndex(e.Page);
            
            lock (_images)
            {
                var olditem = _images[id].Image;
                var newitem = ToImageSource(e.Image);
                _images[id].Status = ImageStatus.Cached;
                _images[id].Image = newitem;
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, _dummy, index);
                OnCollectionChanged(args);
                //if (olditem.CanFreeze) olditem.Freeze();
            }

            lock (_requests)
            {
                _requests.Remove(index);

                if (_requests.Count > 0　&& UnderImageCreation() == null)
                {
                    var page = _pages[_requests.Keys[0]];
                    var power  = GetPower(page);
                    var engine = _engines[page.FilePath];
                    engine.CreateImageAsync(page.PageNumber, power);
                }
            }
        }

        #endregion

        #region INotifyCollectionChanged

        /* ----------------------------------------------------------------- */
        ///
        /// CollectionChanged
        /// 
        /// <summary>
        /// リスト自体、またはリストの各要素に何らかの変更が生じた場合に
        /// 発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Variables
        private int _width = 0;
        private string _path = string.Empty;
        private SortedList<string, CubePdf.Drawing.BitmapEngine> _engines = new SortedList<string, CubePdf.Drawing.BitmapEngine>();
        private List<CubePdf.Data.Page> _pages = new List<CubePdf.Data.Page>();
        private SortedList<string, ImageContainer> _images = new SortedList<string, ImageContainer>();
        private SortedList<int, string> _requests = new SortedList<int, string>();
        private ImageSource _dummy = null;
        #endregion
    }
}
