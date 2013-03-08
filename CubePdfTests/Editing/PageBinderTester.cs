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
                binder.Encryption = new CubePdf.Data.Encryption(reader.Encryption);
                foreach (var page in reader.Pages)
                {
                    binder.Pages.Add(new CubePdf.Data.Page(page.Value));
                }
                
                var dest = System.IO.Path.Combine(_dest, "TestCopy.pdf");
                binder.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));
            }
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

            var dest = System.IO.Path.Combine(_dest, "TestFullMerge.pdf");
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
            
            var dest = System.IO.Path.Combine(_dest, "TestPartMerge.pdf");
            binder.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
