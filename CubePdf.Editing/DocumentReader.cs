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
            _core = password.Length > 0 ?
                new iTextSharp.text.pdf.PdfReader(path, System.Text.Encoding.UTF8.GetBytes(password)) :
                new iTextSharp.text.pdf.PdfReader(path);
            _path = path;

            ExtractPages(_core, _path);
            ExtractMetadata(_core);
            ExtractEncryption(_core, password);
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
            if (_core == null) return;

            _core.Close();
            _core = null;
            _metadata = null;
            _permission = null;
            _path = string.Empty;
            _pages.Clear();
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
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルのメタデータを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IReadOnlyMetadata Metadata
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
        public CubePdf.Data.IReadOnlyPermission Permission
        {
            get { return _permission; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページの情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public SortedDictionary<int, CubePdf.Data.IReadOnlyPage> Pages
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
            for (int i = 0; i < reader.NumberOfPages; ++i)
            {
                var page = new CubePdf.Data.Page();
                page.FilePath = path;
                page.PageNumber = i + 1;
                page.OriginalSize = Translator.ToSize(reader.GetPageSize(i + 1));
                page.Rotation = reader.GetPageRotation(i + 1);
                page.Power = 1.0;
                _pages.Add(i + 1, page);
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
        private void ExtractMetadata(iTextSharp.text.pdf.PdfReader reader)
        {
            var metadata = new CubePdf.Data.Metadata();

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
        private CubePdf.Data.IReadOnlyMetadata _metadata = null;
        private CubePdf.Data.EncryptionStatus _status = Data.EncryptionStatus.NotEncrypted;
        private CubePdf.Data.EncryptionMethod _method = Data.EncryptionMethod.Unknown;
        private CubePdf.Data.IReadOnlyPermission _permission = null;
        private SortedDictionary<int, CubePdf.Data.IReadOnlyPage> _pages = new SortedDictionary<int, CubePdf.Data.IReadOnlyPage>();
        #endregion
    }
}
