/* ------------------------------------------------------------------------- */
///
/// Wpf/ListViewModelTester.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using NUnit.Framework;

namespace CubePdfTests.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListViewModelTester
    /// 
    /// <summary>
    /// ListViewModel クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class ListViewModelTester
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
        /// TestOpen
        /// 
        /// <summary>
        /// ListViewModel オブジェクトを生成して、PDF ファイルを開くテスト
        /// です。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestOpen()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ItemWidth = 64;
            viewmodel.Open(src);
            Assert.AreEqual(9, viewmodel.ItemCount);
            viewmodel.Close();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestSaveAs
        /// 
        /// <summary>
        /// PDF ファイルを開いて、そのままの内容で別名で保存するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestSaveAs()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ItemWidth = 64;
            viewmodel.Open(src);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelSaveAs.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
            viewmodel.Close();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestAdd
        /// 
        /// <summary>
        /// PDF ファイルを追加するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestAdd()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ItemWidth = 64;
            viewmodel.Open(src);
            Assert.AreEqual(9, viewmodel.ItemCount);

            src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            viewmodel.Add(src);
            Assert.AreEqual(11, viewmodel.ItemCount);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelAdd.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            try
            {
                viewmodel.Add(src);
                Assert.Fail("never reached");
            }
            catch (CubePdf.Wpf.MultipleLoadException /* err */)
            {
                Assert.Pass();
            }
            finally
            {
                viewmodel.Close();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestAdd
        /// 
        /// <summary>
        /// PDF ファイルを挿入するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestInsert()
        {
            // TODO: テストを書く。
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestAdd
        /// 
        /// <summary>
        /// PDF ページを削除するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestRemoveAt()
        {
            // TODO: テストを書く。
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestExtract
        /// 
        /// <summary>
        /// PDF ファイルの一部を抽出するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestExtract()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ItemWidth = 64;
            viewmodel.Open(src);

            // TODO: Items に対して各種操作が効かないため、テスト方法を検討する。
            var images = new List<ImageSource>();
            images.Add(viewmodel.Items[0]);
            images.Add(viewmodel.Items[2]);
            images.Add(viewmodel.Items[1]);
            images.Add(viewmodel.Items[4]);
            var dest = System.IO.Path.Combine(_dest, "TestListViewModelExtract.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Extract(images as IList<ImageSource>, dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(4,   doc.Pages.Count);
                Assert.AreEqual(0,   doc.Pages[1].Rotation);
                Assert.AreEqual(180, doc.Pages[2].Rotation);
                Assert.AreEqual(90,  doc.Pages[3].Rotation);
                Assert.AreEqual(0,   doc.Pages[4].Rotation);
            }
        }
        /* ----------------------------------------------------------------- */
        ///
        /// TestMove
        /// 
        /// <summary>
        /// PDF ぺーじの移動をテストします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestMove()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ItemWidth = 64;
            viewmodel.Open(src);

            viewmodel.Move(1, 0);
            viewmodel.Move(1, 3);
            viewmodel.Move(2, viewmodel.ItemCount - 1);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelMove.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(9,   doc.Pages.Count);
                Assert.AreEqual(90,  doc.Pages[1].Rotation);
                Assert.AreEqual(180, doc.Pages[2].Rotation);
                Assert.AreEqual(0,   doc.Pages[3].Rotation);
                Assert.AreEqual(0,   doc.Pages[4].Rotation);
                Assert.AreEqual(270, doc.Pages[9].Rotation);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestRotateAt
        /// 
        /// <summary>
        /// PDF ページを回転するテストです。
        /// 
        /// NOTE: PageBinder クラスの回転処理が実装されていないので、
        /// テストに失敗します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestRotateAt()
        {
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));

            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ItemWidth = 64;
            viewmodel.Open(src);

            viewmodel.RotateAt(0, 90);  //   0 ->  90
            viewmodel.RotateAt(1, 180); //  90 -> 270
            viewmodel.RotateAt(2, 270); // 180 ->  90
            viewmodel.RotateAt(3, -90); // 270 -> 180

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelRotate.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(3,   doc.Pages.Count);
                Assert.AreEqual(90,  doc.Pages[1].Rotation);
                Assert.AreEqual(270, doc.Pages[2].Rotation);
                Assert.AreEqual(90,  doc.Pages[3].Rotation);
                Assert.AreEqual(180, doc.Pages[4].Rotation);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestUndo
        /// 
        /// <summary>
        /// やり直し（直前の操作を取り消す）のテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestUndo()
        {
            // TODO: テストを書く。
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
