/* ------------------------------------------------------------------------- */
///
/// DocumentReader.cs
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

namespace CubePdf.Editing
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentReader
    /// 
    /// <summary>
    /// PDF ファイルの各種情報を保持するためのクラスです。iTextSharp を用いて
    /// 解析を行います。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class DocumentReader : CubePdf.Data.IDocumentReader
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// DocumentReader (constructor)
        /// 
        /// <summary>
        /// 既定の値で DocumentReader クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DocumentReader() { }

        /* ----------------------------------------------------------------- */
        ///
        /// DocumentReader (constructor)
        /// 
        /// <summary>
        /// 対象となる PDF ファイルへのパス、およびパスワードを指定して
        /// DocumentReader クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DocumentReader(string path, string password = "")
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
        ~DocumentReader()
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
            if (_disposed) return;
            _disposed = true;
            if (disposing) this.Close();
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
            if (_core != null) this.Close();

            try
            {
                _core = password.Length > 0 ?
                    new iTextSharp.text.pdf.PdfReader(path, System.Text.Encoding.UTF8.GetBytes(password)) :
                    new iTextSharp.text.pdf.PdfReader(path);
                _path = path;
                _password = password;

                ExtractPages(_core, _path);
                ExtractMetadata(_core, _path);
                ExtractEncryption(_core, _password);
            }
            catch (iTextSharp.text.pdf.BadPasswordException err)
            {
                throw new CubePdf.Data.EncryptionException(err.Message, err);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在、開いている PDF ファイルを閉じます。
        /// 
        /// TODO: DateTime オブジェクトは null 非許容なので、どういった値で
        /// リセットするか検討する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Close()
        {
            if (_core == null) return;

            _core.Close();
            _core = null;
            _metadata = null;
            _permission = null;
            _path = string.Empty;
            _pages.Clear();
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

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// PDF ファイルのパスを取得します。
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
        /// なのかの判断については、EncryptionStatus の情報から判断します。
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
        /// PDF ファイルのページ数を取得します。
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
        /// PDF ファイルのメタデータを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IMetadata Metadata
        {
            get { return _metadata; }
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
            get { return _status; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionMethod
        /// 
        /// <summary>
        /// 暗号化方式を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.EncryptionMethod EncryptionMethod
        {
            get { return _method; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Permission
        /// 
        /// <summary>
        /// PDF ファイルに設定されている各種操作の権限に関する情報を取得
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IPermission Permission
        {
            get { return _permission; }
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

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractPages
        /// 
        /// <summary>
        /// PDF ファイルのページ情報を抽出します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ExtractPages(iTextSharp.text.pdf.PdfReader reader, string path)
        {
            _pages.Capacity = reader.NumberOfPages + 1;
            for (int i = 0; i < reader.NumberOfPages; ++i)
            {
                var page = new CubePdf.Data.Page();
                page.FilePath = path;
                page.PageNumber = i + 1;
                page.OriginalSize = Translator.ToSize(reader.GetPageSize(i + 1));
                page.Rotation = reader.GetPageRotation(i + 1);
                page.Power = 1.0;
                _pages.Add(page);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractMetadata
        /// 
        /// <summary>
        /// PDF ファイルのメタデータを抽出します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ExtractMetadata(iTextSharp.text.pdf.PdfReader reader, string path)
        {
            var metadata = new CubePdf.Data.Metadata();
            metadata.Version  = new Version(1, Int32.Parse(reader.PdfVersion.ToString()), 0, 0);
            metadata.Author   = reader.Info.ContainsKey("Author")   ? reader.Info["Author"] : "";
            metadata.Title    = reader.Info.ContainsKey("Title")    ? reader.Info["Title"] : "";
            metadata.Subtitle = reader.Info.ContainsKey("Subject")  ? reader.Info["Subject"] : "";
            metadata.Keywords = reader.Info.ContainsKey("Keywords") ? reader.Info["Keywords"] : "";
            metadata.Creator  = reader.Info.ContainsKey("Creator")  ? reader.Info["Creator"] : "";
            metadata.Producer = reader.Info.ContainsKey("Producer") ? reader.Info["Producer"] : "";
            _metadata = metadata;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractEncryption
        /// 
        /// <summary>
        /// PDF ファイルの暗号化に関わる情報を抽出します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ExtractEncryption(iTextSharp.text.pdf.PdfReader reader, string password)
        {
            if (!reader.IsOpenedWithFullPermissions) _status = Data.EncryptionStatus.RestrictedAccess;
            else if (password.Length == 0) _status = Data.EncryptionStatus.NotEncrypted;
            else _status = Data.EncryptionStatus.FullAccess;
            _permission = Translator.ToPermission(reader.Permissions);
        }

        #endregion

        #region Variables
        private bool _disposed = false;
        private iTextSharp.text.pdf.PdfReader _core = null;
        private string _path = string.Empty;
        private string _password = string.Empty;
        private CubePdf.Data.IMetadata _metadata = null;
        private CubePdf.Data.EncryptionStatus _status = Data.EncryptionStatus.NotEncrypted;
        private CubePdf.Data.EncryptionMethod _method = Data.EncryptionMethod.Unknown;
        private CubePdf.Data.IPermission _permission = null;
        private List<CubePdf.Data.IPage> _pages = new List<CubePdf.Data.IPage>();
        #endregion
    }
}
