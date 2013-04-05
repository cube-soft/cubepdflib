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
    /// PageBinder
    /// 
    /// <summary>
    /// 複数の PDF ファイルのページの一部、または全部をまとめて一つの
    /// PDF ファイルにするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class PageBinder
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

            var doc = new iTextSharp.text.Document();
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new System.IO.FileStream(path, System.IO.FileMode.Create));

            writer.Open();
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
            foreach (var page in _pages)
            {
                var reader = new iTextSharp.text.pdf.PdfReader(page.FilePath);
                switch (page.Rotation)
                {
                    case 0:
                        doc.SetPageSize(reader.GetPageSize(page.PageNumber));//WithRotation
                        break;
                    case 90:
                        doc.SetPageSize(reader.GetPageSize(page.PageNumber).Rotate());//WithRotation
                        break;
                    case 180:
                        doc.SetPageSize(reader.GetPageSize(page.PageNumber).Rotate().Rotate());//WithRotation
                        break;
                    case 270:
                        doc.SetPageSize(reader.GetPageSize(page.PageNumber).Rotate().Rotate().Rotate());//WithRotation
                        break;
                }
                doc.NewPage();

                switch (page.Rotation)
                {
                    case 0:
                        wdc.AddTemplate(writer.GetImportedPage(reader, page.PageNumber), 1f, 0f, 0f, 1f, 0f, 0f);
                        break;
                    case 90:
                        wdc.AddTemplate(writer.GetImportedPage(reader, page.PageNumber), 0f, -1f, 1f, 0f, 0f, reader.GetPageSizeWithRotation(page.PageNumber).Width);
                        break;
                    case 180:
                        wdc.AddTemplate(writer.GetImportedPage(reader, page.PageNumber), -1f, 0f, 0f, -1f, reader.GetPageSizeWithRotation(page.PageNumber).Width, reader.GetPageSizeWithRotation(page.PageNumber).Height);
                        break;
                    case 270:
                        wdc.AddTemplate(writer.GetImportedPage(reader, page.PageNumber), 0f, 1f, -1f, 0f, reader.GetPageSizeWithRotation(page.PageNumber).Width, 0f);
                        break;
                }
                //writer.AddPage(writer.GetImportedPage(reader, page.PageNumber));
                reader.Close();
            }

            doc.AddAuthor(_metadata.Author);
            doc.AddTitle(_metadata.Title);
            doc.AddSubject(_metadata.Subtitle);
            doc.AddKeywords(_metadata.Keywords);
            doc.AddCreator(_metadata.Creator);
            doc.AddProducer();

            doc.Close();
            writer.Close();
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        /// Metadata
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Metadata Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Encryption
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Encryption Encryption
        {
            get { return _encrypt; }
            set { _encrypt = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Pages
        /* ----------------------------------------------------------------- */
        public ICollection<CubePdf.Data.Page> Pages
        {
            get { return _pages; }
        }

        #endregion

        #region Variables
        private CubePdf.Data.Metadata _metadata = new CubePdf.Data.Metadata();
        private CubePdf.Data.Encryption _encrypt = new CubePdf.Data.Encryption();
        private List<CubePdf.Data.Page> _pages = new List<CubePdf.Data.Page>();
        #endregion
    }
}
