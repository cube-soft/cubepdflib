/* ------------------------------------------------------------------------- */
///
/// Drawing/BitmapEngineTester.cs
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
using CubePdf.Data.Extensions;
using NUnit.Framework;

namespace CubePdfTests.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// BitmapEngineTester
    ///
    /// <summary>
    /// BitmapEngine クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class BitmapEngineTester
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
            _power = 0.1; // NOTE: イメージ作成のテスト時間を短縮するために生成倍率を下げる。
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
        /// TestOpen
        /// 
        /// <summary>
        /// PDF ファイルを開くテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestOpen()
        {
            // 1. ノーマルケース
            var filename = System.IO.Path.Combine(_src, "rotated.pdf");
            using (var engine = new CubePdf.Drawing.BitmapEngine(filename))
            {
                Assert.Pass();
                Assert.AreEqual(9, engine.Pages.Count);
            }

            // 2. オーナパスワードを指定して開く
            filename = System.IO.Path.Combine(_src, "password.pdf");
            var password = "password"; // OwnerPassword
            using (var engine = new CubePdf.Drawing.BitmapEngine(filename, password))
            {
                Assert.Pass();
                Assert.AreEqual(2, engine.Pages.Count);
            }

            // 3. ユーザパスワードを指定して開く
            password = "view"; // UserPassword
            using (var engine = new CubePdf.Drawing.BitmapEngine(filename, password))
            {
                Assert.Pass();
                Assert.AreEqual(2, engine.Pages.Count);
            }

            // 4. 間違ったパスワードを指定する
            password = "invalid";
            try
            {
                using (var engine = new CubePdf.Drawing.BitmapEngine(filename, password))
                {
                    Assert.Fail();
                }
            }
            catch (System.IO.FileLoadException /* e */)
            {
                Assert.Pass();
            }

            // 5. 存在しないファイルを指定する
            filename = "not-found.pdf";
            try
            {
                using (var engine = new CubePdf.Drawing.BitmapEngine(filename))
                {
                    Assert.Fail();
                }
            }
            catch (System.IO.FileLoadException /* e */)
            {
                Assert.Pass();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestProperties
        /// 
        /// <summary>
        /// 各種プロパティをテストします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestProperties()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            using (var engine = new CubePdf.Drawing.BitmapEngine(src))
            {
                Assert.AreEqual(src, engine.FilePath);
                Assert.AreEqual(9, engine.Pages.Count);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestPages
        /// 
        /// <summary>
        /// 各種ページの情報が取得できているかどうかテストします。
        /// テストに使用しているサンプルファイルは、全 9 ページの PDF で、
        /// 2 ページ目を 90 度、3ページ目を 180 度、4 ページ目を 270 度
        /// 回転させています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestPages()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            using (var engine = new CubePdf.Drawing.BitmapEngine(src))
            {
                var page = engine.GetPage(1);
                Assert.NotNull(page);
                Assert.AreEqual(engine.FilePath, page.FilePath);
                Assert.AreEqual(1, page.PageNumber);
                Assert.IsTrue(page.OriginalSize.Width > 0);
                Assert.IsTrue(page.OriginalSize.Height > 0);
                Assert.IsTrue(page.ViewSize().Width > 0);
                Assert.IsTrue(page.ViewSize().Height > 0);
                Assert.AreEqual(0, page.Rotation);
                Assert.AreEqual(1.0, page.Power);

                page = null;
                page = engine.GetPage(2);
                Assert.NotNull(page);
                Assert.AreEqual(engine.FilePath, page.FilePath);
                Assert.AreEqual(2, page.PageNumber);
                Assert.IsTrue(page.OriginalSize.Width > 0);
                Assert.IsTrue(page.OriginalSize.Height > 0);
                Assert.IsTrue(page.ViewSize().Width > 0);
                Assert.IsTrue(page.ViewSize().Height > 0);
                Assert.AreEqual(90, page.Rotation);
                Assert.AreEqual(1.0, page.Power);

                page = null;
                page = engine.GetPage(3);
                Assert.NotNull(page);
                Assert.AreEqual(engine.FilePath, page.FilePath);
                Assert.AreEqual(3, page.PageNumber);
                Assert.IsTrue(page.OriginalSize.Width > 0);
                Assert.IsTrue(page.OriginalSize.Height > 0);
                Assert.IsTrue(page.ViewSize().Width > 0);
                Assert.IsTrue(page.ViewSize().Height > 0);
                Assert.AreEqual(180, page.Rotation);
                Assert.AreEqual(1.0, page.Power);

                page = null;
                page = engine.GetPage(4);
                Assert.NotNull(page);
                Assert.AreEqual(engine.FilePath, page.FilePath);
                Assert.AreEqual(4, page.PageNumber);
                Assert.IsTrue(page.OriginalSize.Width > 0);
                Assert.IsTrue(page.OriginalSize.Height > 0);
                Assert.IsTrue(page.ViewSize().Width > 0);
                Assert.IsTrue(page.ViewSize().Height > 0);
                Assert.AreEqual(270, page.Rotation);
                Assert.AreEqual(1.0, page.Power);

                page = null;
                page = engine.GetPage(5);
                Assert.NotNull(page);
                Assert.AreEqual(engine.FilePath, page.FilePath);
                Assert.AreEqual(5, page.PageNumber);
                Assert.IsTrue(page.OriginalSize.Width > 0);
                Assert.IsTrue(page.OriginalSize.Height > 0);
                Assert.IsTrue(page.ViewSize().Width > 0);
                Assert.IsTrue(page.ViewSize().Height > 0);
                Assert.AreEqual(0, page.Rotation);
                Assert.AreEqual(1.0, page.Power);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestCreateImage
        /// 
        /// <summary>
        /// PDF ファイルの各ページのイメージを作成するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestCreateImage()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            using (var engine = new CubePdf.Drawing.BitmapEngine(src))
            {
                foreach (var page in engine.Pages)
                {
                    using (var image = engine.CreateImage(page.PageNumber, _power))
                    {
                        Assert.NotNull(image);
                        Assert.AreEqual((int)(page.ViewSize().Width * _power), image.Width);
                        Assert.AreEqual((int)(page.ViewSize().Height * _power), image.Height);
                        var filename = String.Format("TestCreateImage-{0}.png", page.PageNumber);
                        var dest = System.IO.Path.Combine(_dest, filename);
                        System.IO.File.Delete(dest);
                        image.Save(dest);
                        Assert.IsTrue(System.IO.File.Exists(dest));
                    }
                }
            }
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        private double _power = 1.0;
        #endregion
    }
}
