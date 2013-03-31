/* ------------------------------------------------------------------------- */
///
/// BitmapEngine.cs
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
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;

namespace CubePdf.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// BitmapEngine
    /// 
    /// <summary>
    /// PDF 中の各ページ（、および PDF に関連する情報）を描画するために
    /// 必要なデータを生成し、提供するためのエンジンです。
    /// 
    /// NOTE: 現在は、実際の描画データ（ビットマップ画像）を生成する為に
    /// PDFLibNet と言うライブラリを使用しているが、将来的にはライブラリを
    /// 変更する予定である。そのため、PDFLibNet に関わるもの（クラス、
    /// メソッド、etc）はこのクラス内で隠蔽し、このクラスを利用する層
    /// では使用しないようにする。
    /// 
    /// http://www.codeproject.com/KB/files/xpdf_csharp.aspx
    /// https://github.com/cube-soft/PDFLibNet
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class BitmapEngine : IDisposable
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// BitmapEngine (constructor)
        /// 
        /// <summary>
        /// 既定の値で BitmapEngine クラスを初期化します。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        public BitmapEngine()
        {
            _pages.Clear();
            _creating.Clear();

            // for CreateImageAsync() method
            _creator.WorkerSupportsCancellation = true;
            _creator.DoWork -= new DoWorkEventHandler(CreateImageAsync_DoWork);
            _creator.DoWork += new DoWorkEventHandler(CreateImageAsync_DoWork);
            _creator.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(CreateImageAsync_RunWorkerCompleted);
            _creator.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CreateImageAsync_RunWorkerCompleted);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// BitmapEngine (constructor)
        ///
        /// <summary>
        /// 対象となる PDF ファイルへのパス、およびパスワードを指定して
        /// BitmapEngine クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BitmapEngine(string path, string password = "")
            : this()
        {
            this.Open(path, password);
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
        ~BitmapEngine()
        {
            this.Dispose(false);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        /// 
        /// <summary>
        /// IDisposable で定義されているメソッドの実装部分です。実際に必要な
        /// 処理は Dispose(bool) メソッドに記述して下さい。
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
            lock (_lock)
            {
                if (_disposed) return;
                _disposed = true;
                if (disposing) this.Close();
            }
        }

        #endregion

        #region Public Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// PDF ファイルを開きます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(string path, string password = "")
        {
            lock (_lock)
            {
                if (_core != null) this.Close();
                _core = new PDFLibNet.PDFWrapper();
                _core.UseMuPDF = true;

                if (password.Length > 0)
                {
                    _core.UserPassword = password;
                    _core.OwnerPassword = password;
                }

                if (!_core.LoadPDF(path)) throw new System.IO.FileLoadException(Properties.Resources.FileLoadException, path);

                _core.CurrentPage = 1;
                _path = path;
            }

            this.ExtractPages();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在、開いている PDF ファイルを閉じます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Close()
        {
            lock (_creating) _creating.Clear();
            if (_creator.IsBusy) _creator.CancelAsync();

            lock (_lock)
            {
                if (_core != null)
                {
                    _core.Dispose();
                    _core = null;
                }
                _pages.Clear();
                _path = string.Empty;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        /// 
        /// <summary>
        /// BitmapEngine を Open() メソッド実行直後の状態にリセットします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Reset()
        {
            lock (_creating) _creating.Clear();
            if (_creator.IsBusy) _creator.CancelAsync();
            lock (_lock)
            {
                _core.CurrentPage = 1;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateImage
        /// 
        /// <summary>
        /// 指定されたページ番号に対応するイメージを生成します。
        /// 引数 power には、取得したいイメージのサイズを元データのサイズ
        /// に対する倍率で指定します。省略時は、等倍率の画像データが
        /// 生成されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Image CreateImage(int pagenum, double power = 1.0)
        {
            lock (_lock)
            {
                PDFLibNet.PDFPage obj;
                if (!_core.Pages.TryGetValue(pagenum, out obj)) return null;

                var page = _pages[pagenum];
                int width = (int)(page.ViewSize.Width * power);
                int height = (int)(page.ViewSize.Height * power);
                return obj.GetBitmap(width, height, true);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateImageAsync
        /// 
        /// <summary>
        /// 指定されたページ番号に対応するイメージを非同期で生成します。
        /// CreateImageAsync() メソッドで指定したイメージの生成が終了
        /// すると ImageGenerated イベントが発生するので、ユーザはこの
        /// イベントを監視する事で生成されたイメージを取得する事ができます。
        /// 
        /// TODO: CubePDF Viewer ではキューに上限値を設けていた。同様の
        /// 処理が必要かどうかを検討する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void CreateImageAsync(int pagenum, double power = 1.0)
        {
            lock (_creating)
            {
                if (!_pages.ContainsKey(pagenum)) return;
                var entry = new ImageEventArgs(new CubePdf.Data.Page(_pages[pagenum]));
                entry.Page.Power = power;
                _creating.Enqueue(entry);
            }
            if (!_creator.IsBusy) _creator.RunWorkerAsync();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CancelImageCreation
        /// 
        /// <summary>
        /// 非同期で実行中のイメージ生成処理をキャンセルします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void CancelImageCreation()
        {
            lock (_creating) _creating.Clear();
            if (_creator.IsBusy) _creator.CancelAsync();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToString
        ///
        /// <summary>
        /// 現在のオブジェクトの状態を表す文字列を返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public override string ToString()
        {
            return (_core != null) ? String.Format("{0}({1})", _path, _pages.Count) : "(empty)";
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// 現在、開いている PDF ファイルのパスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページのページ情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IDictionary<int, CubePdf.Data.IReadOnlyPage> Pages
        {
            get { return _pages; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UnderImageCreation
        /// 
        /// <summary>
        /// 非同期で実行中のイメージ作成処理が存在するかどうかを判定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool UnderImageCreation
        {
            get { lock (_creating) return _creating.Count > 0 || _creator.IsBusy; }
        }

        #endregion

        #region Event Handlers

        /* ----------------------------------------------------------------- */
        ///
        /// ImageGenerated
        ///
        /// <summary>
        /// あるページのイメージ生成が終了した際に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event ImageEventHandler ImageCreated;
        protected virtual void OnImageCreated(ImageEventArgs e)
        {
            if (ImageCreated != null) ImageCreated(this, e);
        }

        #endregion

        #region Methods for BackgroundWorker

        /* ----------------------------------------------------------------- */
        ///
        /// CreateImageAsync_DoWork
        ///
        /// <summary>
        /// 非同期で要求されたイメージを作成していくためのメソッドです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CreateImageAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            ImageEventArgs task = null;
            lock (_creating)
            {
                if (_creating.Count == 0) return;
                task = _creating.Dequeue();
            }

            if (_creator.CancellationPending) return;
            lock (_lock)
            {
                if (_creator.CancellationPending) return;
                task.Image = this.CreateImage(task.Page.PageNumber, task.Page.Power);
            }
            e.Result = task;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateImageAsync_RunWorkerCompleted
        ///
        /// <summary>
        /// 要求されたイメージの作成が完了した際に ImageCreated イベントを
        /// 発生させるためのメソッドです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CreateImageAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_creator.CancellationPending) return;
            lock (_creating)
            {
                if (_creating.Count > 0) _creator.RunWorkerAsync();
            }

            var args = e.Result as ImageEventArgs;
            if (args != null) this.OnImageCreated(args);
        }

        #endregion

        #region Other Methods

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractPageProperties
        /// 
        /// <summary>
        /// PDFLibNet から PDF の各ページの情報を抽出します。
        /// 
        /// NOTE: 各ページに関する情報を取得する際、PDFWrapper オブジェクト
        /// をロックしなければならず、非同期でイメージを作成している時
        /// などはページ情報の取得に予想外の時間を要する事もあるため、
        /// ファイルから情報をロードした段階で予め全てのページの情報を
        /// 取得しておきます。
        /// 
        /// TODO: PDF ファイルのページ数が増えると、この処理にかなりの
        /// 時間を要するようになる事が考えられる。テスト結果次第では、
        /// この処理を BackgroundWorker で行う等も検討する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ExtractPages()
        {
            _pages.Clear();
            for (int i = 0; i < _core.PageCount; ++i)
            {
                lock (_lock)
                {
                    PDFLibNet.PDFPage obj;
                    if (!_core.Pages.TryGetValue(i + 1, out obj)) return;

                    var page = new CubePdf.Data.Page(_path, i + 1);
                    page.OriginalSize = new Size((int)obj.Width, (int)obj.Height);
                    page.Rotation = obj.Rotation;
                    if (!_pages.ContainsKey(i + 1)) _pages.Add(i + 1, page);
                }
            }
        }

        #endregion

        #region Variables
        private bool _disposed = false;
        private object _lock = new object();
        private string _path = string.Empty;
        private PDFLibNet.PDFWrapper _core = null;
        private SortedDictionary<int, CubePdf.Data.IReadOnlyPage> _pages = new SortedDictionary<int, CubePdf.Data.IReadOnlyPage>();
        private BackgroundWorker _creator = new BackgroundWorker();
        private Queue<ImageEventArgs> _creating = new Queue<ImageEventArgs>();
        #endregion

    }
}
