/* ------------------------------------------------------------------------- */
///
/// Editing/DocumentReaderTester.cs
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
using System.Text;
using NUnit.Framework;

namespace CubePdfTests.Editing
{
    /* --------------------------------------------------------------------- */
    ///
    /// PageBinderTester
    /// 
    /// <summary>
    /// PageBinder クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class PageBinderTester
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

        #region TestMethods

        /* ----------------------------------------------------------------- */
        ///
        /// TestCopy
        /// 
        /// <summary>
        /// PageBinder クラスを用いて、元の PDF ファイルと同じものをコピー
        /// するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestCopy()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                var binder = new CubePdf.Editing.PageBinder();
                binder.Metadata = new CubePdf.Data.Metadata(reader.Metadata);
                foreach (var page in reader.Pages)
                {
                    binder.Pages.Add(new CubePdf.Data.Page(page));
                }

                var dest = System.IO.Path.Combine(_dest, "TestPageBinderCopy.pdf");
                System.IO.File.Delete(dest);
                binder.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestOverwrite
        /// 
        /// <summary>
        /// PageBinder クラスを用いて、ソースファイルへの上書きをテスト
        /// します。処理としては、いったん一時ファイルへ保存した後に
        /// 移動しています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestOverwrite()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderOverwrite.pdf");
            System.IO.File.Delete(dest);

            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                var binder = new CubePdf.Editing.PageBinder();
                binder.Metadata = new CubePdf.Data.Metadata(reader.Metadata);
                foreach (var page in reader.Pages)
                {
                    binder.Pages.Add(new CubePdf.Data.Page(page));
                }
                binder.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));
            }

            var tmp = System.IO.Path.GetTempFileName();
            System.IO.File.Delete(tmp);

            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                var binder = new CubePdf.Editing.PageBinder();
                binder.Metadata = new CubePdf.Data.Metadata(reader.Metadata);
                foreach (var page in reader.Pages)
                {
                    binder.Pages.Add(new CubePdf.Data.Page(page));
                }
                binder.Save(tmp);
                Assert.IsTrue(System.IO.File.Exists(tmp));
            }

            System.IO.File.Delete(dest);
            System.IO.File.Move(tmp, dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestMerge
        /// 
        /// <summary>
        /// PageBinder クラスを用いてページ結合のテストを行います。
        /// </summary>
        /// 
        /// <param name="head">先頭に結合するファイル名</param>
        /// <param name="tail">末尾に結合するファイル名</param>
        /// <param name="filename">保存するファイル名</param>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("readme.pdf",     "rotated.pdf",     "TestMerge.pdf")]
        [TestCase("readme.pdf",     "annotation.pdf",  "TestMergeAnnotation1.pdf")]
        [TestCase("annotation.pdf", "readme.pdf",      "TestMergeAnnotation2.pdf")]
        [TestCase("annotation2.pdf", "annotation.pdf", "TestMergeAnnotation3.pdf")]
        [TestCase("annotation.pdf", "annotation2.pdf", "TestMergeAnnotation4.pdf")]
        [TestCase("readme.pdf",     "bookmark.pdf",    "TestMergeBookmark1.pdf")]
        [TestCase("readme.pdf",     "bookmark2.pdf",   "TestMergeBookmark2.pdf")]
        [TestCase("bookmark.pdf",   "bookmark2.pdf",   "TestMergeBookmark3.pdf")]
        public void TestMerge(string head, string tail, string filename)
        {
            var binder = new CubePdf.Editing.PageBinder();

            var pagenum = 0;
            var src = System.IO.Path.Combine(_src, head);
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page));
                pagenum += reader.PageCount;
            }

            src = System.IO.Path.Combine(_src, tail);
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page));
                pagenum += reader.PageCount;
            }

            var dest = System.IO.Path.Combine(_dest, filename);
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new iTextSharp.text.pdf.PdfReader(dest))
            {
                Assert.AreEqual(pagenum, reader.NumberOfPages);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestPartMerge
        /// 
        /// <summary>
        /// PageBinder クラスを用いて、2 つの PDF ファイルの一部ページを
        /// 結合するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestPartMerge()
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                Assert.AreEqual(9, reader.PageCount);
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(1)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(3)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(5)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(7)));
            }

            src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                Assert.AreEqual(2, reader.PageCount);
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(1)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(2)));
            }

            var dest = System.IO.Path.Combine(_dest, "TestPartMerge.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(6, reader.PageCount);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestMergeWithPassword
        /// 
        /// <summary>
        /// パスワードの設定されている PDF ファイルのページを結合対象と
        /// して指定された場合のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestMergeWithPassword()
        {
            var binder = new CubePdf.Editing.PageBinder();

            // OwnerPassword
            var src = System.IO.Path.Combine(_src, "password.pdf");
            var password = "password";
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src, password))
            {
                foreach (var page in reader.Pages)
                {
                    Assert.AreEqual(src, page.FilePath);
                    Assert.AreEqual(password, page.Password);
                    binder.Pages.Add(new CubePdf.Data.Page(page));
                }
            }

            try
            {
                var dest = System.IO.Path.Combine(_dest, "TestMergeWithPassword.pdf");
                System.IO.File.Delete(dest);
                binder.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
            binder.Pages.Clear();

            // UserPassword
            password = "view";
            using (var reader = new CubePdf.Editing.DocumentReader(src, password))
            {
                foreach (var page in reader.Pages)
                {
                    Assert.AreEqual(src, page.FilePath);
                    Assert.AreEqual(password, page.Password);
                    binder.Pages.Add(new CubePdf.Data.Page(page));
                }
            }

            try
            {
                var dest = System.IO.Path.Combine(_dest, "TestMergePassword.pdf");
                System.IO.File.Delete(dest);
                binder.Save(dest);
                Assert.Fail("never reached");
            }
            catch (Exception /* err */) { Assert.Pass(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestRotate
        /// 
        /// <summary>
        /// PageBinder クラスを用いてページ回転のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("rotated.pdf",    "TestRotate0.pdf",              0)]
        [TestCase("rotated.pdf",    "TestRotate90.pdf",            90)]
        [TestCase("rotated.pdf",    "TestRotate180.pdf",          180)]
        [TestCase("rotated.pdf",    "TestRotate270.pdf",          270)]
        [TestCase("annotation.pdf", "TestRotate90Annotation.pdf",  90)]
        [TestCase("bookmark.pdf",   "TestRotate90Bookmark.pdf",    90)]
        public void TestRotate(string src, string filename, int degree)
        {
            var binder = new CubePdf.Editing.PageBinder();

            src = System.IO.Path.Combine(_src, src);
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            foreach (var page in reader.Pages)
            {
                var duplicate = new CubePdf.Data.Page(page);
                duplicate.Rotation = degree;
                binder.Pages.Add(duplicate);
            }

            var dest = System.IO.Path.Combine(_dest, filename);
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            foreach (var page in reader.Pages)
            {
                Assert.AreEqual(degree, page.Rotation);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestPageOrderWithBookmark
        /// 
        /// <summary>
        /// ページ順番を元の PDF ファイルとはまったく異なる配置にしたとき、
        /// しおりが追随できるかどうかをテストします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestPageOrderWithBookmark()
        {
            var src = System.IO.Path.Combine(_src, "bookmark.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                var binder = new CubePdf.Editing.PageBinder();
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(3)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(1)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(4)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(5)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(9)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(2)));
                binder.Pages.Add(new CubePdf.Data.Page(reader.GetPage(6)));

                var dest = System.IO.Path.Combine(_dest, "TestPageOrderBookmark.pdf");
                System.IO.File.Delete(dest);
                binder.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestMetadata
        /// 
        /// <summary>
        /// PageBinder クラスを用いて文書プロパティの設定・消去テストを
        /// 行います。
        /// 
        /// TODO: PDF バージョンの設定にバグが存在する模様。
        /// 現在コメントアウトしている箇所に関して、修正とテストを行う。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestMetadata()
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            // 新しい情報の設定テスト
            var metadata = new CubePdf.Data.Metadata(binder.Metadata);
            metadata.Version = new Version("1.7");
            metadata.Title = "TestPageBinderMetadata";
            metadata.Author = "キューブ・ソフト";
            metadata.Subtitle = "文書プロパティ編集テスト";
            metadata.Keywords = "Cube,PDF,編集";
            binder.Metadata = metadata;

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderMetadata.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(1, reader.Metadata.Version.Major);
                Assert.AreEqual(7, reader.Metadata.Version.Minor);
                Assert.AreEqual("TestPageBinderMetadata", reader.Metadata.Title);
                Assert.AreEqual("キューブ・ソフト", reader.Metadata.Author);
                Assert.AreEqual("文書プロパティ編集テスト", reader.Metadata.Subtitle);
                Assert.AreEqual("Cube,PDF,編集", reader.Metadata.Keywords);
            }

            // 情報の消去テスト
            metadata = new CubePdf.Data.Metadata(binder.Metadata);
            metadata.Version = new Version("1.5");
            metadata.Title = "";
            metadata.Author = "";
            metadata.Subtitle = "";
            metadata.Keywords = "";
            binder.Metadata = metadata;

            dest = System.IO.Path.Combine(_dest, "TestPageBinderMetadataDeleted.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(1, reader.Metadata.Version.Major);
                Assert.AreEqual(5, reader.Metadata.Version.Minor);
                Assert.IsTrue(String.IsNullOrEmpty(binder.Metadata.Title));
                Assert.IsTrue(String.IsNullOrEmpty(binder.Metadata.Author));
                Assert.IsTrue(String.IsNullOrEmpty(binder.Metadata.Subtitle));
                Assert.IsTrue(String.IsNullOrEmpty(binder.Metadata.Keywords));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestEncryption
        /// 
        /// <summary>
        /// PageBinder クラスを用いて暗号化に関する情報を設定するテストを
        /// 行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("TestPageBinderAes128.pdf", CubePdf.Data.EncryptionMethod.Aes128)]
        [TestCase("TestPageBinderAes256.pdf", CubePdf.Data.EncryptionMethod.Aes256)]
        public void TestEncryption(string filename, CubePdf.Data.EncryptionMethod method)
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            var encrypt = new CubePdf.Data.Encryption(binder.Encryption);
            encrypt.IsEnabled = true;
            encrypt.OwnerPassword = "password";
            encrypt.IsUserPasswordEnabled = true;
            encrypt.UserPassword = "view";
            encrypt.Method = method;
            encrypt.Permission.Printing = true;
            encrypt.Permission.DegradedPrinting = true;
            encrypt.Permission.ModifyContents = true;
            encrypt.Permission.Assembly = true;
            encrypt.Permission.CopyContents = true;
            encrypt.Permission.ExtractPage = true;
            encrypt.Permission.Accessibility = true;
            encrypt.Permission.ModifyAnnotations = true;
            encrypt.Permission.InputFormFields = true;
            encrypt.Permission.Signature = true;
            encrypt.Permission.TemplatePage = true;
            binder.Encryption = encrypt;

            var dest = System.IO.Path.Combine(_dest, filename);
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest, "password"))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.FullAccess, reader.EncryptionStatus);
                Assert.AreEqual(method, reader.EncryptionMethod);
                Assert.IsTrue(reader.Permission.Printing);
                Assert.IsTrue(reader.Permission.DegradedPrinting);
                Assert.IsTrue(reader.Permission.ModifyAnnotations);
                Assert.IsTrue(reader.Permission.Assembly);
                Assert.IsTrue(reader.Permission.CopyContents);
                Assert.IsTrue(reader.Permission.Accessibility);
                Assert.IsTrue(reader.Permission.ModifyAnnotations);
                Assert.IsTrue(reader.Permission.InputFormFields);

                // NOTE: ExtractPage, Signature, TemplatePage は DocumentReader (iTextSharp) が未対応。
                // Assert.IsTrue(reader.Permission.ExtractPage);
                // Assert.IsTrue(reader.Permission.Signature);
                // Assert.IsTrue(reader.Permission.TemplatePage);
            }

            encrypt = new CubePdf.Data.Encryption(binder.Encryption);
            encrypt.Permission.Printing = false;
            encrypt.Permission.DegradedPrinting = false;
            encrypt.Permission.ModifyContents = false;
            encrypt.Permission.Assembly = false;
            encrypt.Permission.CopyContents = false;
            encrypt.Permission.ExtractPage = false;
            encrypt.Permission.Accessibility = false;
            encrypt.Permission.ModifyAnnotations = false;
            encrypt.Permission.InputFormFields = false;
            encrypt.Permission.Signature = false;
            encrypt.Permission.TemplatePage = false;
            binder.Encryption = encrypt;

            dest = System.IO.Path.Combine(_dest, "TestPageBinderNoPermission.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest, "password"))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.FullAccess, reader.EncryptionStatus);
                Assert.AreEqual(method, reader.EncryptionMethod);
                Assert.IsFalse(reader.Permission.Printing);
                Assert.IsFalse(reader.Permission.DegradedPrinting);
                Assert.IsFalse(reader.Permission.ModifyAnnotations);
                Assert.IsFalse(reader.Permission.Assembly);
                Assert.IsFalse(reader.Permission.CopyContents);
                Assert.IsFalse(reader.Permission.Accessibility);
                Assert.IsFalse(reader.Permission.ModifyAnnotations);
                Assert.IsFalse(reader.Permission.InputFormFields);

                // NOTE: ExtractPage, Signature, TemplatePage は DocumentReader (iTextSharp) が未対応。
                // Assert.IsFalse(reader.Permission.ExtractPage);
                // Assert.IsFalse(reader.Permission.Signature);
                // Assert.IsFalse(reader.Permission.TemplatePage);
            }

            using (var reader = new CubePdf.Editing.DocumentReader(dest, "view"))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.RestrictedAccess, reader.EncryptionStatus);
            }

            try
            {
                new CubePdf.Editing.DocumentReader(dest, "invalid");
                Assert.Fail();
            }
            catch (Exception /* err */) { Assert.Pass(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestNoEncryption
        /// 
        /// <summary>
        /// PageBinder クラスを用いて暗号化に関する情報を設定するテストを
        /// 行います。暗号化、および閲覧時のパスワードを設定するかどうかは
        /// それぞれ IsEnabled, IsUserPasswordEnabled プロパティで判断
        /// します。このテストでは、それぞの値を False に設定し、他の暗号化
        /// に関する設定が無効になっている事を確認します。
        /// 
        /// NOTE: EncryptionMethod, および Permission の各値のテストに
        /// ついては DocumentReader の実装がまだのため、コメントアウトして
        /// います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestNoEncryption()
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            var encrypt = new CubePdf.Data.Encryption(binder.Encryption);
            encrypt.IsEnabled = false;
            encrypt.OwnerPassword = "password";
            encrypt.IsUserPasswordEnabled = true;
            encrypt.UserPassword = "view";
            encrypt.Method = CubePdf.Data.EncryptionMethod.Aes128;
            binder.Encryption = encrypt;

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderNoEncryption.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.NotEncrypted, reader.EncryptionStatus);
            }

            encrypt = new CubePdf.Data.Encryption(binder.Encryption);
            encrypt.IsEnabled = true;
            encrypt.IsUserPasswordEnabled = false;
            binder.Encryption = encrypt;

            dest = System.IO.Path.Combine(_dest, "TestPageBinderNoEncryptionUserPassword.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest, "password"))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.FullAccess, reader.EncryptionStatus);
            }

            try
            {
                new CubePdf.Editing.DocumentReader(dest, "view");
                Assert.Fail();
            }
            catch (Exception /* err */) { Assert.Pass(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestPermission
        /// 
        /// <summary>
        /// 各パーミッションのみを許可した場合のテストを行います。通常テスト
        /// の他、生成されたファイルは、パーミッションが Adobe Reader でどの
        /// ように表示されるかを確認されるために使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestPermission()
        {
            var binder = new CubePdf.Editing.PageBinder();
            var encrypt = new CubePdf.Data.Encryption();
            encrypt.IsEnabled = true;
            encrypt.OwnerPassword = "owner";
            encrypt.Method = CubePdf.Data.EncryptionMethod.Aes256;

            var src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            // Printing
            var permission = new CubePdf.Data.Permission();
            permission.Printing = true;
            var dest = System.IO.Path.Combine(_dest, "TestPermissionPrinting.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // DegradedPrinting
            permission = new CubePdf.Data.Permission();
            permission.DegradedPrinting = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionDegradedPrinting.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // Assembly
            permission = new CubePdf.Data.Permission();
            permission.Assembly = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionAssembly.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // ModifyContents
            permission = new CubePdf.Data.Permission();
            permission.ModifyContents = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionModifyContents.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // CopyContents
            permission = new CubePdf.Data.Permission();
            permission.CopyContents = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionCopyContents.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // Accessibility
            permission = new CubePdf.Data.Permission();
            permission.Accessibility = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionAccessibility.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // ModifyAnnotations
            permission = new CubePdf.Data.Permission();
            permission.ModifyAnnotations = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionModifyAnnotations.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // InputFormFields
            permission = new CubePdf.Data.Permission();
            permission.InputFormFields = true;
            dest = System.IO.Path.Combine(_dest, "TestPermissionInputFormFields.pdf");
            System.IO.File.Delete(dest);
            encrypt.Permission = permission;
            binder.Encryption = encrypt;
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // ExtractPage
            //permission = new CubePdf.Data.Permission();
            //permission.ExtractPage = true;
            //dest = System.IO.Path.Combine(_dest, "TestPermissionExtractPage.pdf");
            //System.IO.File.Delete(dest);
            //encrypt.Permission = permission;
            //binder.Encryption = encrypt;
            //binder.Save(dest);
            //Assert.IsTrue(System.IO.File.Exists(dest));

            // Signature
            //permission = new CubePdf.Data.Permission();
            //permission.Signature = true;
            //dest = System.IO.Path.Combine(_dest, "TestPermissionSignature.pdf");
            //System.IO.File.Delete(dest);
            //encrypt.Permission = permission;
            //binder.Encryption = encrypt;
            //binder.Save(dest);
            //Assert.IsTrue(System.IO.File.Exists(dest));

            // TemplatePage
            //permission = new CubePdf.Data.Permission();
            //permission.TemplatePage = true;
            //dest = System.IO.Path.Combine(_dest, "TestPermissionTemplatePage.pdf");
            //System.IO.File.Delete(dest);
            //encrypt.Permission = permission;
            //binder.Encryption = encrypt;
            //binder.Save(dest);
            //Assert.IsTrue(System.IO.File.Exists(dest));
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
