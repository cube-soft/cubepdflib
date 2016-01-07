/* ------------------------------------------------------------------------- */
///
/// ImageCreator.cs
///
/// Copyright (c) 2013 CubeSoft, Inc.
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Cube;
using CubePdf.Data;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageCreator
    /// 
    /// <summary>
    /// PDF の各ページのサムネイルを作成するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ImageCreator
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// ImageCreator
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageCreator()
        {
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// IsBusy
        /// 
        /// <summary>
        /// サムネイルを作成中かどうかを示す値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool IsBusy
        {
            get { return _worker.IsBusy; }
        }

        #endregion

        #region Events

        /* ----------------------------------------------------------------- */
        ///
        /// Creating
        /// 
        /// <summary>
        /// サムネイルが作成される直前に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EventHandler<DataCancelEventArgs<ImageEntry>> Creating;

        /* ----------------------------------------------------------------- */
        ///
        /// Created
        /// 
        /// <summary>
        /// サムネイルが作成された時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EventHandler<DataEventArgs<ImageEntry>> Created;

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Create
        /// 
        /// <summary>
        /// 非同期でイメージを作成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Image Create(PageBase page)
        {
            switch (page.Type)
            {
                case PageType.Pdf:   return CreateUsingPage(page as Page);
                case PageType.Image: return CreateUsingImagePage(page as ImagePage);
                default:
                    break;
            }
            return null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateAsync
        /// 
        /// <summary>
        /// 非同期でイメージを作成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void CreateAsync(PageBase page, int index)
        {
            lock (_queue)
            {
                if (_queue.ContainsKey(index)) _queue[index] = page;
                else _queue.Add(index, page);
            }
            Fetch();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        /// 
        /// <summary>
        /// サムネイルの作成用キューに登録されている項目を削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Remove(int first, int last)
        {
            lock (_queue)
            {
                for (int i = first; i <= last; ++i)
                {
                    if (_queue.ContainsKey(i)) _queue.Remove(i);
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        /// 
        /// <summary>
        /// サムネイルの作成用キューに登録されている項目を全て削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Clear()
        {
            if (_worker.IsBusy) _worker.CancelAsync();
            lock (_queue) _queue.Clear();
        }

        #endregion

        #region Virtual methods

        /* ----------------------------------------------------------------- */
        ///
        /// OnCreating
        /// 
        /// <summary>
        /// サムネイルが作成される直前に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnCreating(DataCancelEventArgs<ImageEntry> e)
        {
            if (Creating != null) Creating(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnCreated
        /// 
        /// <summary>
        /// サムネイルが作成された後に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnCreated(DataEventArgs<ImageEntry> e)
        {
            if (Created != null) Created(this, e);
        }

        #endregion

        #region Other private methods

        /* ----------------------------------------------------------------- */
        ///
        /// Fetch
        /// 
        /// <summary>
        /// サムネイルの作成待ちキューの先頭を実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Fetch()
        {
            if (_worker.IsBusy) return;

            lock (_queue)
            {
                while (_queue.Count > 0)
                {
                    var item = _queue.First();
                    _queue.Remove(item.Key);

                    var value = new ImageEntry
                    {
                        Index = item.Key,
                        Page  = item.Value,
                        Image = null
                    };
                    var args  = new DataCancelEventArgs<ImageEntry>(value);
                    OnCreating(args);
                    if (args.Cancel) continue;

                    _worker.RunWorkerAsync(value);
                    break;
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateUsingPage
        /// 
        /// <summary>
        /// Page オブジェクトの情報を用いてイメージを生成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Image CreateUsingPage(Page page)
        {
            if (page == null) return null;

            var engine = CubePdf.Drawing.BitmapEnginePool.Get(page);
            if (engine == null) return null;

            Image image = null;
            PageBase original = null;
            lock (engine)
            {
                image = engine.CreateImage(page.PageNumber, page.Power);
                if (image == null) return null;
                original = engine.GetPage(page.PageNumber);
            }
            RotateImage(image, page, original);
            return image;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateUsingImagePage
        /// 
        /// <summary>
        /// ImagePage オブジェクトの情報を用いてイメージを生成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Image CreateUsingImagePage(ImagePage page)
        {
            if (page == null) return null;

            throw new NotImplementedException();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RotateImage
        /// 
        /// <summary>
        /// 現在のページ情報とオリジナルを比較して、必要であればイメージを
        /// 回転させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RotateImage(Image image, PageBase current, PageBase original)
        {
            var delta = current.Rotation - original.Rotation;
            if (delta < 0) delta += 360;
            if (delta >= 360) delta -= 360;
            if (delta == 0) return;

            RotateImage(image, delta);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RotateImage
        /// 
        /// <summary>
        /// 引数に指定された image を degree 度だけ回転させます。
        /// </summary>
        /// 
        /// <remarks>
        /// System.Drawing.Image.RotateFlip メソッドは 90 度単位でしか
        /// 回転させる事ができないので、引数に指定された回転度数を 90 度単位
        /// で丸めています。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private void RotateImage(Image image, int degree)
        {
            var value = RotateFlipType.RotateNoneFlipNone;
            if (degree >= 90 && degree < 180) value = RotateFlipType.Rotate90FlipNone;
            else if (degree >= 180 && degree < 270) value = RotateFlipType.Rotate180FlipNone;
            else if (degree >= 270 && degree < 360) value = RotateFlipType.Rotate270FlipNone;
            image.RotateFlip(value);
        }

        #endregion

        #region Background worker's handlers

        /* ----------------------------------------------------------------- */
        ///
        /// Worker_DoWork
        /// 
        /// <summary>
        /// 非同期で処理が実行される時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var item = e.Argument as ImageEntry;
            if (item == null || _worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            var image = Create(item.Page);
            if (image == null || _worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            item.Image = image;
            e.Result = item;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Worker_RunWorkerCompleted
        /// 
        /// <summary>
        /// 非同期による処理が完了した時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var result = e.Result as ImageEntry;
                if (e.Cancelled || e.Error != null || result == null) return;
                OnCreated(new DataEventArgs<ImageEntry>(result));
            }
            catch (Exception /* err */) { return; }
            finally { Fetch(); }
        }

        #endregion

        #region Fields
        private SortedList<int, PageBase> _queue = new SortedList<int, PageBase>();
        private BackgroundWorker _worker = new BackgroundWorker();
        #endregion
    }
}
