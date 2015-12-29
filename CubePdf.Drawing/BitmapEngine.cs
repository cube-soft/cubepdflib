/* ------------------------------------------------------------------------- */
///
/// BitmapEngine.cs
///
/// Copyright (c) 2013 CubeSoft, Inc. All rights reserved.
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
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using CubePdf.Data.Extensions;

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
    public class BitmapEngine : CubePdf.Data.IDocumentReader
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
            Open(path, password);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// IDocumentReader の情報を利用して、BitmapEngine クラスを初期化
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BitmapEngine(CubePdf.Data.IDocumentReader reader)
            : this()
        {
            Open(reader);
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
            Dispose(false);
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
            Dispose(true);
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
        /// <remarks>
        /// 現在、引数にパスワードが設定された場合は全てオーナパスワードと
        /// 判断している。オーナパスワードかユーザパスワードかを判別する
        /// 方法を検討する。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(string path, string password = "")
        {
            _metadata = new CubePdf.Data.Metadata();

            var encrypt = new CubePdf.Data.Encryption();
            if (!string.IsNullOrEmpty(password))
            {
                encrypt.IsEnabled = true;
                encrypt.OwnerPassword = password;
            }
            _encrypt = encrypt;

            OpenFile(path, password);
            ExtractPages();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// 他の IDocumentReader オブジェクトの情報を利用して、PDF ファイル
        /// の各ページに対応するイメージを生成可能な状態にします。
        /// </summary>
        ///
        /// <remarks>
        /// イメージ生成に際したパフォーマンスの関係で、まず始めに
        /// パスワード無で開けるかどうかを試行します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(CubePdf.Data.IDocumentReader other)
        {
            try
            {
                _metadata = new CubePdf.Data.Metadata(other.Metadata);
                _encrypt  = new CubePdf.Data.Encryption(other.Encryption);
                _status   = other.EncryptionStatus;
                OpenFile(other.FilePath, "");
            }
            catch (Exception /* err */)
            {
                var password = (_encrypt.IsEnabled && !string.IsNullOrEmpty(_encrypt.OwnerPassword)) ?
                    other.Encryption.OwnerPassword : other.Encryption.UserPassword;
                OpenFile(other.FilePath, password);
            }

            // TODO: 要素をコピーして代入
            foreach (var page in other.Pages) _pages.Add(page);
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
                _pages.Clear();
                _path = string.Empty;
                _metadata = null;
                _encrypt = null;
                _status = Data.EncryptionStatus.NotEncrypted;

                if (_core != null)
                {
                    _core.Dispose();
                    _core = null;
                }
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
        /// GetPage
        /// 
        /// <summary>
        /// 指定されたページ番号に対応するページ情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IPage GetPage(int pagenum)
        {
            return _pages[pagenum - 1];
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
            var index = pagenum - 1;
            lock (_lock)
            {
                PDFLibNet.PDFPage obj;
                if (!_core.Pages.TryGetValue(pagenum, out obj)) return null;
                
                var page = _pages[index];
                int width = (int)(page.ViewSize().Width * power);
                int height = (int)(page.ViewSize().Height * power);
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
        /// </summary>
        /// 
        /// <remarks>
        /// TODO: CubePDF Viewer ではキューに上限値を設けていた。同様の
        /// 処理が必要かどうかを検討する。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void CreateImageAsync(int pagenum, double power = 1.0)
        {
            var index = pagenum - 1;
            lock (_creating)
            {
                if (index >= _pages.Count) return;
                var page = new CubePdf.Data.Page(_pages[index] as CubePdf.Data.Page);
                page.Power = power;
                var entry = new ImageEventArgs(page);
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
        /// PageCount
        /// 
        /// <summary>
        /// 現在、開いている PDF ファイルのページ数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int PageCount
        {
            get { return _pages.Count; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページ情報へアクセスするための反復子を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<CubePdf.Data.IPage> Pages
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

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルのメタデータを取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// PDFLibNet からは現在メタデータの情報が取得できないため、
        /// 情報が不完全な場合があります。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Metadata Metadata
        {
            get { return _metadata; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// PDF ファイルの暗号化に関する状態を取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// PDFLibNet からは現在、暗号化に関する情報が取得できないため、
        /// 情報が不完全な場合があります。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Encryption Encryption
        {
            get { return _encrypt; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionStatus
        /// 
        /// <summary>
        /// 暗号化されている PDF ファイルへのアクセス（許可）状態を取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// PDFLibNet からは現在、暗号化に関する情報が取得できないため、
        /// 情報が不完全な場合があります。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.EncryptionStatus EncryptionStatus
        {
            get { return _status; }
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
            else if (e.Image != null) e.Image.Dispose();
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
            var args = e.Result as ImageEventArgs;
            if (_creator.CancellationPending)
            {
                if (args != null && args.Image != null) args.Image.Dispose();
                return;
            }

            lock (_creating)
            {
                if (_creating.Count > 0 && !_creator.IsBusy) _creator.RunWorkerAsync();
            }
            if (args != null) this.OnImageCreated(args);
        }

        #endregion

        #region Other Methods

        /* ----------------------------------------------------------------- */
        ///
        /// OpenFile
        /// 
        /// <summary>
        /// PDF ファイルを開いて、各ページに対するイメージを生成可能な状態に
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OpenFile(string path, string password)
        {
            lock (_lock)
            {
                if (_core != null) Close();
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
        }

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
                    page.Password = _encrypt.OwnerPassword;
                    page.OriginalSize = new Size((int)obj.Width, (int)obj.Height);
                    page.Rotation = obj.Rotation;
                    if (i >= _pages.Count) _pages.Add(page);
                    else _pages[i] = page;
                }
            }
        }

        #endregion

        #region Variables
        private bool _disposed = false;
        private object _lock = new object();
        private string _path = string.Empty;
        private PDFLibNet.PDFWrapper _core = null;
        private IList<CubePdf.Data.IPage> _pages = new List<CubePdf.Data.IPage>();
        private CubePdf.Data.Metadata _metadata = null;
        private CubePdf.Data.Encryption _encrypt = null;
        private CubePdf.Data.EncryptionStatus _status = Data.EncryptionStatus.NotEncrypted;
        private BackgroundWorker _creator = new BackgroundWorker();
        private Queue<ImageEventArgs> _creating = new Queue<ImageEventArgs>();
        #endregion

    }
}
