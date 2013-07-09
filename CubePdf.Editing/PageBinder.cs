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
                var doc = new iTextSharp.text.Document();
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new System.IO.FileStream(path, System.IO.FileMode.Create));
                if (writer == null) return;

                writer.PdfVersion = _metadata.Version.Minor.ToString()[0];
                if (_encrypt.IsEnabled && _encrypt.OwnerPassword.Length > 0)
                {
                    var method = Translator.ToIText(_encrypt.Method);
                    var permission = Translator.ToIText(_encrypt.Permission);
                    var userpassword = _encrypt.IsUserPasswordEnabled ? _encrypt.UserPassword : "";
                    writer.SetEncryption(method, userpassword, _encrypt.OwnerPassword, permission);
                }

                doc.Open();
                var wdc = writer.DirectContent;
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
                    
                    doc.SetPageSize(new iTextSharp.text.Rectangle(page.ViewSize.Width, page.ViewSize.Height, page.Rotation));
                    doc.NewPage();

                    var radian = Math.PI * page.Rotation / 180.0;
                    var sin = (float)Math.Sin(radian);
                    var cos = (float)Math.Cos(radian);
                    var original = reader.GetPageSize(page.PageNumber);
                    var x = (original.Width * Math.Abs(cos) + original.Height * Math.Abs(sin)) * (-sin - cos + 1) / 2;
                    var y = (original.Width * Math.Abs(sin) + original.Height * Math.Abs(cos)) * (sin - cos + 1) / 2;

                    wdc.AddTemplate(writer.GetImportedPage(reader, page.PageNumber), cos, -sin, sin, cos, x, y);
                    CopyAnnotations(writer, reader, page.PageNumber);
                    AddBookmarks(reader, page.PageNumber, pagenum++);
                }
                writer.Outlines = _bookmarks;
                doc.AddAuthor(_metadata.Author);
                doc.AddTitle(_metadata.Title);
                doc.AddSubject(_metadata.Subtitle);
                doc.AddKeywords(_metadata.Keywords);
                doc.AddCreator(_metadata.Creator);
                doc.AddProducer();

                doc.Close();
                foreach (var reader in readers.Values) reader.Close();
                readers.Clear();
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

            for (int i = 0; i < annots.Size; i += 2)
            {
                var dic = PdfReader.GetPdfObject(annots[i]) as PdfDictionary;
                if (dic != null)
                {
                    var annotation = new PdfAnnotation(dest, null);
                    annotation.PutAll(dic);
                    annotation.MKRotation = rotate;
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
                if (bm["Page"].ToString().Contains(destpage.ToString()))
                {
                    _bookmarks.Add(bm);
                }
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
