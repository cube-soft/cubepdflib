/* ------------------------------------------------------------------------- */
///
/// BackgroundImageExtractor.cs
///
/// Copyright (c) 2010 CubeSoft, Inc.
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
using System.Drawing;
using Cube;
using CubePdf.Data;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// BackgroundImageExtractor
    /// 
    /// <summary>
    /// バックグラウンドで PDF 内のイメージを抽出するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class BackgroundImageExtractor
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// BackgroundImageExtractor
        ///
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BackgroundImageExtractor()
        {
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        ///
        /// <summary>
        /// イメージを抽出するページ一覧を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IList<PageBase> Pages { get; } = new List<PageBase>();

        /* ----------------------------------------------------------------- */
        ///
        /// IsBusy
        ///
        /// <summary>
        /// 処理中かどうかを示す値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool IsBusy { get { return _worker.IsBusy; } }

        #endregion

        #region Events

        /* ----------------------------------------------------------------- */
        ///
        /// ProgressChanged
        ///
        /// <summary>
        /// 進捗状況が変化した時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EventHandler<ProgressEventArgs<ImageList>> ProgressChanged;

        /* ----------------------------------------------------------------- */
        ///
        /// Completed
        ///
        /// <summary>
        /// 非同期処理が完了した時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EventHandler Completed;

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// RunAsync
        ///
        /// <summary>
        /// 非同期で処理を実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RunAsync()
        {
            _worker.RunWorkerAsync();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CancelAsync
        ///
        /// <summary>
        /// 処理をキャンセルします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void CancelAsync()
        {
            _worker.CancelAsync();
        }

        #endregion

        #region Virtual methods

        /* ----------------------------------------------------------------- */
        ///
        /// OnProgressChanged
        ///
        /// <summary>
        /// 進捗状況が変化した時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnProgressChanged(ProgressEventArgs<ImageList> e)
        {
            if (ProgressChanged != null) ProgressChanged(this, e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnCompleted
        ///
        /// <summary>
        /// 非同期処理が完了した時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnCompleteed(EventArgs e)
        {
            if (Completed != null) Completed(this, e);
        }

        #endregion

        #region Event handlers

        /* ----------------------------------------------------------------- */
        ///
        /// Worker_DoWork
        ///
        /// <summary>
        /// 非同期で処理を実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (var i = 0; i < Pages.Count; ++i)
            {
                var percent = (int)((i + 1) / (double)Pages.Count * 100.0);
                var dest = new ImageList();
                dest.Page = Pages[i];

                switch (dest.Page.Type)
                {
                    case PageType.Image:
                        ExtractFromImagePage(dest.Page as ImagePage, dest.Images);
                        break;
                    case PageType.Pdf:
                        ExtractFromPage(dest.Page as Page, dest.Images);
                        break;
                    default:
                        break;
                }
                _worker.ReportProgress(percent, dest);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Worker_ProgressChanged
        ///
        /// <summary>
        /// 進捗状況が変化した時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var value = e.UserState as ImageList;
            if (value == null) return;
            OnProgressChanged(new ProgressEventArgs<ImageList>(e.ProgressPercentage, value));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Worker_RunWorkerCompleted
        ///
        /// <summary>
        /// 処理が完了した時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled || e.Error != null) return;
                OnCompleteed(new EventArgs());
            }
            finally
            {
                foreach (var item in _readers)
                {
                    if (item.Value != null) item.Value.Dispose();
                }
                _readers.Clear();
            }
        }

        #endregion

        #region Other private methods

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractFromPage
        ///
        /// <summary>
        /// PDF ページからイメージを抽出します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ExtractFromPage(Page src, IList<Image> dest)
        {
            if (src == null) return;
            if (!_readers.ContainsKey(src.FilePath))
            {
                var pre = new Editing.DocumentReader();
                pre.Open(src.FilePath, src.Password);
                _readers.Add(src.FilePath, pre);
            }

            var reader = _readers[src.FilePath];
            if (reader.EncryptionStatus == EncryptionStatus.RestrictedAccess) return;
            foreach (var image in reader.GetImages(src.PageNumber)) dest.Add(image);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractFromImagePage
        ///
        /// <summary>
        /// イメージページからイメージを抽出します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ExtractFromImagePage(ImagePage src, IList<Image> dest)
        {
            if (src == null) return;

            var image = Image.FromFile(src.FilePath);
            var guid  = image.FrameDimensionsList[0];
            var dim   = new System.Drawing.Imaging.FrameDimension(guid);
            var index = src.PageNumber - 1;
            if (index > 0 && index < image.GetFrameCount(dim)) image.SelectActiveFrame(dim, index);

            dest.Add(image);
        }

        #endregion

        #region Fields
        private BackgroundWorker _worker = new BackgroundWorker();
        private Dictionary<string, Editing.DocumentReader> _readers = new Dictionary<string, Editing.DocumentReader>();
        #endregion
    }
}
