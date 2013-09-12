/* ------------------------------------------------------------------------- */
///
/// Editing/DocumentReaderTester.cs
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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace CubePdfTests.Editing
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentReaderTester
    /// 
    /// <summary>
    /// DocumentReader クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class DocumentReaderTester
    {
        #region Setup and TearDown

        /* ----------------------------------------------------------------- */
        ///
        /// Setup
        /// 
        /// <summary>
        /// NOTE: テストに使用するサンプルファイル群は、テスト用プロジェクト
        /// フォルダ直下にある Examples と言うフォルダに存在します。
        /// テストを実行する際には、実行ファイルをテスト用プロジェクトに
        /// コピーしてから行う必要があります（ビルド後イベントで、自動的に
        /// コピーされるように設定されてある）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [SetUp]
        public void Setup()
        {
            var current = System.Environment.CurrentDirectory;
            _src = System.IO.Path.Combine(current, "Examples");
            _dest = System.IO.Path.Combine(current, "Results");
            if (!System.IO.Directory.Exists(_dest)) System.IO.Directory.CreateDirectory(_dest);
        }

        /* ----------------------------------------------------------------- */
        /// TearDown
        /* ----------------------------------------------------------------- */
        [TearDown]
        public void TearDown() { }

        #endregion

        #region Test Methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestConstruct
        /// 
        /// <summary>
        /// DocumentReader クラスの初期化のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestConstruct()
        {
            var doc = new CubePdf.Editing.DocumentReader();
            Assert.AreEqual(0, doc.FilePath.Length);
            Assert.IsNull(doc.Metadata);
            Assert.IsNull(doc.Encryption);
            Assert.AreEqual(CubePdf.Data.EncryptionStatus.NotEncrypted, doc.EncryptionStatus);
            Assert.NotNull(doc.Pages);
            Assert.AreEqual(0, doc.PageCount);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestOpen
        /// 
        /// <summary>
        /// ファイルを開くテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestOpen()
        {
            try
            {
                var filename = System.IO.Path.Combine(_src, "rotated.pdf");
                Assert.IsTrue(System.IO.File.Exists(filename));

                using (var doc = new CubePdf.Editing.DocumentReader(filename))
                {
                    Assert.AreEqual(filename, doc.FilePath);
                    Assert.NotNull(doc.Metadata);
                    Assert.NotNull(doc.Metadata.Version);
                    Assert.AreEqual(1, doc.Metadata.Version.Major);
                    Assert.AreEqual(7, doc.Metadata.Version.Minor);
                    Assert.AreEqual(0, doc.Metadata.Version.Build);
                    Assert.AreEqual(0, doc.Metadata.Version.Revision);
                    Assert.AreEqual("CubeSoft", doc.Metadata.Author);
                    Assert.AreEqual("CubePdfTests", doc.Metadata.Title);
                    Assert.AreEqual("rotated example", doc.Metadata.Subtitle);
                    Assert.AreEqual("CubeSoft,PDF,Test", doc.Metadata.Keywords);

                    Assert.IsFalse(doc.Encryption.IsEnabled);
                    Assert.IsNullOrEmpty(doc.Encryption.OwnerPassword);
                    Assert.IsFalse(doc.Encryption.IsUserPasswordEnabled);
                    Assert.IsNullOrEmpty(doc.Encryption.UserPassword);
                    Assert.AreEqual(CubePdf.Data.EncryptionMethod.Unknown, doc.Encryption.Method);
                    Assert.NotNull(doc.Encryption.Permission);
                    Assert.AreEqual(CubePdf.Data.EncryptionStatus.NotEncrypted, doc.EncryptionStatus);

                    Assert.AreEqual(9, doc.PageCount);
                    var page = doc.GetPage(1);
                    Assert.NotNull(page);
                    Assert.AreEqual(doc.FilePath, page.FilePath);
                    Assert.AreEqual(1, page.PageNumber);
                    Assert.IsTrue(page.OriginalSize.Width > 0);
                    Assert.IsTrue(page.OriginalSize.Height > 0);
                    Assert.IsTrue(page.ViewSize.Width > 0);
                    Assert.IsTrue(page.ViewSize.Height > 0);
                    Assert.AreEqual(0, page.Rotation);
                    Assert.AreEqual(1.0, page.Power);

                    page = null;
                    page = doc.GetPage(2);
                    Assert.NotNull(page);
                    Assert.AreEqual(doc.FilePath, page.FilePath);
                    Assert.AreEqual(2, page.PageNumber);
                    Assert.IsTrue(page.OriginalSize.Width > 0);
                    Assert.IsTrue(page.OriginalSize.Height > 0);
                    Assert.IsTrue(page.ViewSize.Width > 0);
                    Assert.IsTrue(page.ViewSize.Height > 0);
                    Assert.AreEqual(90, page.Rotation);
                    Assert.AreEqual(1.0, page.Power);

                    page = null;
                    page = doc.GetPage(3);
                    Assert.NotNull(page);
                    Assert.AreEqual(doc.FilePath, page.FilePath);
                    Assert.AreEqual(3, page.PageNumber);
                    Assert.IsTrue(page.OriginalSize.Width > 0);
                    Assert.IsTrue(page.OriginalSize.Height > 0);
                    Assert.IsTrue(page.ViewSize.Width > 0);
                    Assert.IsTrue(page.ViewSize.Height > 0);
                    Assert.AreEqual(180, page.Rotation);
                    Assert.AreEqual(1.0, page.Power);

                    page = null;
                    page = doc.GetPage(4);
                    Assert.NotNull(page);
                    Assert.AreEqual(doc.FilePath, page.FilePath);
                    Assert.AreEqual(4, page.PageNumber);
                    Assert.IsTrue(page.OriginalSize.Width > 0);
                    Assert.IsTrue(page.OriginalSize.Height > 0);
                    Assert.IsTrue(page.ViewSize.Width > 0);
                    Assert.IsTrue(page.ViewSize.Height > 0);
                    Assert.AreEqual(270, page.Rotation);
                    Assert.AreEqual(1.0, page.Power);

                    page = null;
                    page = doc.GetPage(5);
                    Assert.NotNull(page);
                    Assert.AreEqual(doc.FilePath, page.FilePath);
                    Assert.AreEqual(5, page.PageNumber);
                    Assert.IsTrue(page.OriginalSize.Width > 0);
                    Assert.IsTrue(page.OriginalSize.Height > 0);
                    Assert.IsTrue(page.ViewSize.Width > 0);
                    Assert.IsTrue(page.ViewSize.Height > 0);
                    Assert.AreEqual(0, page.Rotation);
                    Assert.AreEqual(1.0, page.Power);
                }
            }
            catch (Exception err)
            {
                Assert.Fail(err.ToString());
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestPassword
        /// 
        /// <summary>
        /// パスワードの設定されているファイルを開くテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestPassword()
        {
            try
            {
                var filename = System.IO.Path.Combine(_src, "password.pdf");
                var password = "password"; // OwnerPassword

                using (var doc = new CubePdf.Editing.DocumentReader(filename, password))
                {
                    Assert.AreEqual(CubePdf.Data.EncryptionStatus.FullAccess, doc.EncryptionStatus);
                    Assert.IsTrue(doc.Encryption.IsEnabled);
                    Assert.AreEqual(password, doc.Encryption.OwnerPassword);
                    Assert.IsTrue(doc.Encryption.IsUserPasswordEnabled);
                    Assert.AreEqual("view", doc.Encryption.UserPassword);
                    Assert.AreEqual(CubePdf.Data.EncryptionMethod.Standard128, doc.Encryption.Method);

                    Assert.IsFalse(doc.Encryption.Permission.Accessibility);
                    Assert.IsTrue(doc.Encryption.Permission.Assembly);
                    Assert.IsFalse(doc.Encryption.Permission.CopyContents);
                    Assert.IsTrue(doc.Encryption.Permission.DegradedPrinting);
                    Assert.IsFalse(doc.Encryption.Permission.InputFormFields);
                    Assert.IsFalse(doc.Encryption.Permission.ModifyAnnotations);
                    Assert.IsFalse(doc.Encryption.Permission.ModifyContents);
                    Assert.IsTrue(doc.Encryption.Permission.Printing);

                    // NOTE: 以下の 3 項目は、iTextSharp に該当項目がないため未設定
                    // Assert.IsTrue(doc.Encryption.Permission.ExtractPage);
                    // Assert.IsTrue(doc.Encryption.Permission.Signature);
                    // Assert.IsTrue(doc.Encryption.Permission.TemplatePage);
                }

                password = "view"; // UserPassword
                using (var doc = new CubePdf.Editing.DocumentReader(filename, password))
                {
                    Assert.AreEqual(CubePdf.Data.EncryptionStatus.RestrictedAccess, doc.EncryptionStatus);
                    Assert.IsTrue(doc.Encryption.IsEnabled);
                    Assert.IsNullOrEmpty(doc.Encryption.OwnerPassword);
                    Assert.IsTrue(doc.Encryption.IsUserPasswordEnabled);
                    Assert.AreEqual(password, doc.Encryption.UserPassword);
                    Assert.AreEqual(CubePdf.Data.EncryptionMethod.Standard128, doc.Encryption.Method);
                }

                password = "bad-password";
                try
                {
                    using (var doc = new CubePdf.Editing.DocumentReader(filename, password))
                    {
                        Assert.Fail("never reached");
                    }
                }
                catch (CubePdf.Data.EncryptionException /* err */)
                {
                    //Assert.Pass(err.ToString());
                }
            }
            catch (Exception err)
            {
                Assert.Fail(err.ToString());
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestIsTaggedDocument
        /// 
        /// <summary>
        /// タグ付き PDF（構造化された PDF）を開くテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotated.pdf", false)]
        [TestCase("tagged.pdf",   true)]
        public void TestIsTaggedDocument(string filename, bool is_tagged)
        {
            try
            {
                var path = System.IO.Path.Combine(_src, filename);
                Assert.IsTrue(System.IO.File.Exists(path));

                using (var doc = new CubePdf.Editing.DocumentReader(path))
                {
                    Assert.AreEqual(is_tagged, doc.IsTaggedDocument);
                }
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
