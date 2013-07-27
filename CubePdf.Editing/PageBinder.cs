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
using System.IO;
using iTextSharp.text.pdf;

namespace CubePdf.Editing
{
    /* --------------------------------------------------------------------- */
    ///
    /// PageBinder
    /// 
    /// <summary>
    /// 複数の PDF ファイルのページの一部、または全部をまとめて一つの
    /// PDF ファイルにするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class PageBinder : CubePdf.Data.IDocumentWriter
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// PageBinder (constructor)
        /// 
        /// <summary>
        /// 既定の値で DocumentReader クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PageBinder() { }

        #endregion

        #region Public Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        /// 
        /// <summary>
        /// PageBinder クラスを初期状態にリセットします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Reset()
        {
            _metadata = new CubePdf.Data.Metadata();
            _encrypt = new CubePdf.Data.Encryption();
            _pages.Clear();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在メンバ変数が保持している、メタデータ、暗号化に関する情報、
        /// 各ページ情報に基づいた PDF ファイルを指定されたパスに保存
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save(string path)
        {
            try
            {
                string tmp = Path.GetTempPath() + Path.GetFileName(path);
                var doc = new PdfCopyFields(new FileStream(tmp, FileMode.Create));
                var readers = new Dictionary<string, iTextSharp.text.pdf.PdfReader>();
                int pagenum = 1;
                foreach (var page in _pages)
                {
                    if (!readers.ContainsKey(page.FilePath))
                    {
                        var item = page.Password.Length > 0 ?
                            new iTextSharp.text.pdf.PdfReader(page.FilePath, System.Text.Encoding.UTF8.GetBytes(page.Password)) :
                            new iTextSharp.text.pdf.PdfReader(page.FilePath);
                        readers.Add(page.FilePath, item);
                    }
                    var reader = readers[page.FilePath];
                    doc.AddDocument(reader, page.PageNumber.ToString());
                    AddBookmarks(reader, page.PageNumber, pagenum++);
                }
                doc.Close();
                foreach (var reader in readers.Values) reader.Close();
                readers.Clear();

                EditMetadata(tmp, path);
                File.Delete(tmp);
            }
            catch (iTextSharp.text.pdf.BadPasswordException err)
            {
                throw new CubePdf.Data.EncryptionException(err.Message, err);
            }
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// EditMetadata
        /// 
        /// <summary>
        /// PdfStamperを用いて、Metadataなどを付与した新たなpdfを作ります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void EditMetadata(string src, string dest)
        {
            var reader = new PdfReader(src);
            var stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create));

            var info = reader.Info;
            info.Add("Title", _metadata.Title);
            info.Add("Subject", _metadata.Subtitle);
            info.Add("Keywords", _metadata.Keywords);
            info.Add("Creator", _metadata.Creator);
            info.Add("Author", _metadata.Author);
            stamper.MoreInfo = info;

            if (_encrypt.IsEnabled && _encrypt.OwnerPassword.Length > 0)
            {
                var method = Translator.ToIText(_encrypt.Method);
                var permission = Translator.ToIText(_encrypt.Permission);
                var userpassword = _encrypt.IsUserPasswordEnabled ? _encrypt.UserPassword : "";
                stamper.Writer.SetEncryption(method, userpassword, _encrypt.OwnerPassword, permission);
            }
            stamper.Writer.Outlines = _bookmarks;
            stamper.Writer.PdfVersion = _metadata.Version.Minor.ToString()[0];

            stamper.Close();
            reader.Close();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CopyAnnotations
        /// 
        /// <summary>
        /// 元の PDF ファイルにある注釈をコピーします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CopyAnnotations(PdfWriter dest, PdfReader src, int pagenum)
        {
            var page = src.GetPageN(pagenum);
            var rotate = src.GetPageRotation(pagenum);
            if (page == null) return;

            var annots = page.GetAsArray(PdfName.ANNOTS);
            if (annots == null) return;

            for (int i = 0; i < annots.Size; i++)
            {
                var dic = PdfReader.GetPdfObject(annots[i]) as PdfDictionary;
                if (dic != null)
                {
                    var annotation = new PdfAnnotation(dest, null);
                    foreach (var item in dic) annotation.Put(item.Key, PdfReader.GetPdfObject(item.Value));
                    dest.AddAnnotation(annotation);
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// AddBookmarks
        /// 
        /// <summary>
        /// 元の PDF ファイルにあるしおりを_bookmarksに追加します。
        /// 実際にしおりをPDFに追加するにはOutlinesプロパティに代入が必要
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void AddBookmarks(PdfReader src, int srcpage, int destpage)
        {
            var bookmarks = SimpleBookmark.GetBookmark(src);
            if (bookmarks == null) return;

            SimpleBookmark.ShiftPageNumbers(bookmarks, destpage - srcpage, null);
            foreach (var bm in bookmarks)
            {
                if (bm["Page"].ToString().Contains(destpage.ToString() + " FitH")) { _bookmarks.Add(bm); }
            }
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルのメタデータを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IMetadata Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// 暗号化に関する情報をを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IEncryption Encryption
        {
            get { return _encrypt; }
            set { _encrypt = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページ情報を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ICollection<CubePdf.Data.IPage> Pages
        {
            get { return _pages; }
        }

        #endregion

        #region Variables
        private CubePdf.Data.IMetadata _metadata = new CubePdf.Data.Metadata();
        private CubePdf.Data.IEncryption _encrypt = new CubePdf.Data.Encryption();
        private List<CubePdf.Data.IPage> _pages = new List<CubePdf.Data.IPage>();
        private IList<Dictionary<String, Object>> _bookmarks = new List<Dictionary<String, Object>>();
        #endregion
    }
}
