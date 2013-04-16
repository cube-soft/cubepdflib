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
                    binder.Pages.Add(new CubePdf.Data.Page(page.Value));
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
                    binder.Pages.Add(new CubePdf.Data.Page(page.Value));
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
                    binder.Pages.Add(new CubePdf.Data.Page(page.Value));
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
        /// TestFullMerge
        /// 
        /// <summary>
        /// PageBinder クラスを用いて、2 つの PDF ファイルの全ページを結合
        /// するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestFullMerge()
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page.Value));
            }

            src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages) binder.Pages.Add(new CubePdf.Data.Page(page.Value));
            }

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderFullMerge.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
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
                Assert.AreEqual(9, reader.Pages.Count);
                binder.Pages.Add(new CubePdf.Data.Page(reader.Pages[1]));
                binder.Pages.Add(new CubePdf.Data.Page(reader.Pages[3]));
                binder.Pages.Add(new CubePdf.Data.Page(reader.Pages[5]));
                binder.Pages.Add(new CubePdf.Data.Page(reader.Pages[7]));
            }

            src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                Assert.AreEqual(2, reader.Pages.Count);
                binder.Pages.Add(new CubePdf.Data.Page(reader.Pages[1]));
                binder.Pages.Add(new CubePdf.Data.Page(reader.Pages[2]));
            }

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderPartMerge.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(6, reader.Pages.Count);
            }
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
        [Test]
        public void TestRotate()
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                Assert.AreEqual(9, reader.Pages.Count);

                var page = new CubePdf.Data.Page(reader.Pages[1]);
                Assert.AreEqual(0, page.Rotation);
                page.Rotation += 90;
                binder.Pages.Add(page);

                page = new CubePdf.Data.Page(reader.Pages[2]);
                Assert.AreEqual(90, page.Rotation);
                page.Rotation += 180;
                binder.Pages.Add(page);

                page = new CubePdf.Data.Page(reader.Pages[3]);
                Assert.AreEqual(180, page.Rotation);
                page.Rotation = 0;
                binder.Pages.Add(page);

                page = new CubePdf.Data.Page(reader.Pages[5]);
                Assert.AreEqual(0, page.Rotation);
                page.Rotation += 180;
                binder.Pages.Add(page);

                page = new CubePdf.Data.Page(reader.Pages[6]);
                Assert.AreEqual(0, page.Rotation);
                page.Rotation += 270;
                binder.Pages.Add(page);
            }

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderRotate.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(5,   reader.Pages.Count);
                Assert.AreEqual(90,  reader.Pages[1].Rotation);
                Assert.AreEqual(270, reader.Pages[2].Rotation);
                Assert.AreEqual(0,   reader.Pages[3].Rotation);
                Assert.AreEqual(180, reader.Pages[4].Rotation);
                Assert.AreEqual(270, reader.Pages[5].Rotation);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestRotateDetail
        /// 
        /// <summary>
        /// PageBinder クラスを用いてページ回転のテストを行います。
        /// (0, 90, 180, 270) x (0, 90, 180, 270) の 16 パターンの回転を
        /// 行い生成後の PDF を確認します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase(0)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void TestRotateDetail(int degree)
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                Assert.AreEqual(0,   reader.Pages[1].Rotation);
                Assert.AreEqual(90,  reader.Pages[2].Rotation);
                Assert.AreEqual(180, reader.Pages[3].Rotation);
                Assert.AreEqual(270, reader.Pages[4].Rotation);

                for (int i = 1; i <= 4; ++i)
                {
                    var page = new CubePdf.Data.Page(reader.Pages[i]);
                    page.Rotation = degree;
                    binder.Pages.Add(page);
                }
            }

            var filename = String.Format("TestPageBinderRotate{0}.pdf", degree);
            var dest = System.IO.Path.Combine(_dest, filename);
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(degree, reader.Pages[1].Rotation);
                Assert.AreEqual(degree, reader.Pages[2].Rotation);
                Assert.AreEqual(degree, reader.Pages[3].Rotation);
                Assert.AreEqual(degree, reader.Pages[4].Rotation);
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
                foreach (var page in reader.Pages.Values) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            // 新しい情報の設定テスト
            binder.Metadata.Version = new Version("1.7");
            binder.Metadata.Title = "TestPageBinderMetadata";
            binder.Metadata.Author = "キューブ・ソフト";
            binder.Metadata.Subtitle = "文書プロパティ編集テスト";
            binder.Metadata.Keywords = "Cube,PDF,編集";

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
            binder.Metadata.Version = new Version("1.5");
            binder.Metadata.Title = "";
            binder.Metadata.Author = "";
            binder.Metadata.Subtitle = "";
            binder.Metadata.Keywords = "";

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
        /// 
        /// TODO: EncryptionMethod のテストについては DocumentReader の実装が
        /// まだのため、コメントアウト。修正終了後、コメントを外してテスト
        /// 行う。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestEncryption()
        {
            var binder = new CubePdf.Editing.PageBinder();

            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            using (var reader = new CubePdf.Editing.DocumentReader(src))
            {
                foreach (var page in reader.Pages.Values) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            binder.Encryption.IsEnabled = true;
            binder.Encryption.OwnerPassword = "password";
            binder.Encryption.IsUserPasswordEnabled = true;
            binder.Encryption.UserPassword = "view";
            binder.Encryption.Method = CubePdf.Data.EncryptionMethod.Aes128;
            binder.Encryption.Permission.Printing = true;
            binder.Encryption.Permission.DegradedPrinting = true;
            binder.Encryption.Permission.ModifyContents = true;
            binder.Encryption.Permission.Assembly = true;
            binder.Encryption.Permission.CopyContents = true;
            binder.Encryption.Permission.ExtractPage = true;
            binder.Encryption.Permission.Accessibility = true;
            binder.Encryption.Permission.ModifyAnnotations = true;
            binder.Encryption.Permission.InputFormFields = true;
            binder.Encryption.Permission.Signature = true;
            binder.Encryption.Permission.TemplatePage = true;

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderEncryption.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest, "password"))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.FullAccess, reader.EncryptionStatus);
                //Assert.AreEqual(CubePdf.Data.EncryptionMethod.Aes128, reader.EncryptionMethod);
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

            binder.Encryption.Permission.Printing = false;
            binder.Encryption.Permission.DegradedPrinting = false;
            binder.Encryption.Permission.ModifyContents = false;
            binder.Encryption.Permission.Assembly = false;
            binder.Encryption.Permission.CopyContents = false;
            binder.Encryption.Permission.ExtractPage = false;
            binder.Encryption.Permission.Accessibility = false;
            binder.Encryption.Permission.ModifyAnnotations = false;
            binder.Encryption.Permission.InputFormFields = false;
            binder.Encryption.Permission.Signature = false;
            binder.Encryption.Permission.TemplatePage = false;

            dest = System.IO.Path.Combine(_dest, "TestPageBinderNoPermission.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest, "password"))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.FullAccess, reader.EncryptionStatus);
                //Assert.AreEqual(CubePdf.Data.EncryptionMethod.Aes128, reader.EncryptionMethod);
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
                foreach (var page in reader.Pages.Values) binder.Pages.Add(new CubePdf.Data.Page(page));
            }

            binder.Encryption.IsEnabled = false;
            binder.Encryption.OwnerPassword = "password";
            binder.Encryption.IsUserPasswordEnabled = true;
            binder.Encryption.UserPassword = "view";
            binder.Encryption.Method = CubePdf.Data.EncryptionMethod.Aes128;

            var dest = System.IO.Path.Combine(_dest, "TestPageBinderNoEncryption.pdf");
            System.IO.File.Delete(dest);
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var reader = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(CubePdf.Data.EncryptionStatus.NotEncrypted, reader.EncryptionStatus);
            }

            binder.Encryption.IsEnabled = true;
            binder.Encryption.IsUserPasswordEnabled = false;

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
                foreach (var page in reader.Pages.Values) binder.Pages.Add(new CubePdf.Data.Page(page));
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
