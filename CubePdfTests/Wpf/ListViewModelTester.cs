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
            var viewmodel = CreateViewModel();
            Assert.AreEqual(System.IO.Path.Combine(_src, "rotated.pdf"), viewmodel.FilePath);
            Assert.AreEqual(64877, viewmodel.FileSize);
            Assert.AreEqual(2013, viewmodel.CreationTime.Year);
            Assert.AreEqual(3, viewmodel.CreationTime.Month);
            Assert.AreEqual(6, viewmodel.CreationTime.Day);
            Assert.AreEqual(2013, viewmodel.UpdateTime.Year);
            Assert.AreEqual(3, viewmodel.UpdateTime.Month);
            Assert.AreEqual(8, viewmodel.UpdateTime.Day);
            Assert.NotNull(viewmodel.Metadata);
            Assert.NotNull(viewmodel.Metadata.Version);
            Assert.AreEqual(1, viewmodel.Metadata.Version.Major);
            Assert.AreEqual(7, viewmodel.Metadata.Version.Minor);
            Assert.AreEqual(0, viewmodel.Metadata.Version.Build);
            Assert.AreEqual(0, viewmodel.Metadata.Version.Revision);
            Assert.AreEqual("CubeSoft", viewmodel.Metadata.Author);
            Assert.AreEqual("CubePdfTests", viewmodel.Metadata.Title);
            Assert.AreEqual("rotated example", viewmodel.Metadata.Subtitle);
            Assert.AreEqual("CubeSoft,PDF,Test", viewmodel.Metadata.Keywords);
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
            var viewmodel = CreateViewModel();
            var dest = System.IO.Path.Combine(_dest, "TestListViewModelSaveAs.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
            viewmodel.Close();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestSaveAs
        /// 
        /// <summary>
        /// 上書き保存のテストです。
        /// 
        /// NOTE: 保留
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        //[Test]
        //public void TestOverwrite()
        //{
        //    var dest = System.IO.Path.Combine(_dest, "TestListViewModelSave.pdf");
        //    {
        //        var viewmodel = CreateViewModel();
        //        System.IO.File.Delete(dest);
        //        viewmodel.Save(dest);
        //        Assert.IsTrue(System.IO.File.Exists(dest));
        //        viewmodel.Close();
        //    }

        //    {
        //        var viewmodel = CreateViewModel(dest);
        //        viewmodel.RemoveAt(0);
        //        viewmodel.Save();
        //        Assert.IsTrue(System.IO.File.Exists(dest));
        //        viewmodel.Close();
        //    }

        //    using (var doc = new CubePdf.Editing.DocumentReader(dest))
        //    {
        //        Assert.AreEqual(8, doc.Pages.Count);
        //    }

        //}

        /* ----------------------------------------------------------------- */
        ///
        /// TestInsert
        /// 
        /// <summary>
        /// PDF ファイルを挿入するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestInsert()
        {
            var viewmodel = CreateViewModel();
            var src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            viewmodel.Insert(2, src);
            Assert.AreEqual(11, viewmodel.ItemCount);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelInsert.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(11,  doc.Pages.Count);
                Assert.AreEqual(0,   doc.Pages[1].Rotation);
                Assert.AreEqual(90,  doc.Pages[2].Rotation);
                Assert.AreEqual(0,   doc.Pages[3].Rotation);
                Assert.AreEqual(0,   doc.Pages[4].Rotation);
                Assert.AreEqual(180, doc.Pages[5].Rotation);
                Assert.AreEqual(270, doc.Pages[6].Rotation);
                Assert.AreEqual(0,   doc.Pages[7].Rotation);
            }

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
            var viewmodel = CreateViewModel();
            var src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            viewmodel.Add(src);
            Assert.AreEqual(11, viewmodel.ItemCount);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelAdd.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(11, doc.Pages.Count);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestRemoveAt
        /// 
        /// <summary>
        /// 対象となる PDF ページをインデックスで指定して削除するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestRemoveAt()
        {
            var viewmodel = CreateViewModel();
            
            viewmodel.RemoveAt(0);
            viewmodel.RemoveAt(2);
            viewmodel.RemoveAt(viewmodel.ItemCount - 1);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelRemoveAt.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(6,   doc.Pages.Count);
                Assert.AreEqual(90,  doc.Pages[1].Rotation);
                Assert.AreEqual(180, doc.Pages[2].Rotation);
                Assert.AreEqual(0,   doc.Pages[3].Rotation);
                Assert.AreEqual(0,   doc.Pages[4].Rotation);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestRemove
        /// 
        /// <summary>
        /// 対象となる PDF ページを対応する画像オブジェクトで指定して
        /// 削除するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestRemove()
        {
            var viewmodel = CreateViewModel();

            viewmodel.Remove(viewmodel.Items[0]);
            viewmodel.Remove(viewmodel.Items[viewmodel.ItemCount - 1]);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelRemove.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(7, doc.Pages.Count);
            }
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
            var viewmodel = CreateViewModel();
            
            var images = new ArrayList();
            images.Add(viewmodel.Items[0]);
            images.Add(viewmodel.Items[2]);
            images.Add(viewmodel.Items[1]);
            images.Add(viewmodel.Items[4]);
            var dest = System.IO.Path.Combine(_dest, "TestListViewModelExtract.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Extract(images, dest);
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
        /// TestExtract
        /// 
        /// <summary>
        /// PDF ファイルの一部を抽出して、別々のファイルに保存するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestSplit()
        {
            var viewmodel = CreateViewModel();
            var src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            viewmodel.Add(src);
            Assert.AreEqual(11, viewmodel.ItemCount);

            var images = new ArrayList();
            images.Add(viewmodel.Items[0]);
            images.Add(viewmodel.Items[2]);
            images.Add(viewmodel.Items[1]);
            images.Add(viewmodel.Items[10]);
            images.Add(viewmodel.Items[9]);

            viewmodel.Split(images, _dest);
            viewmodel.Close();

            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(_dest, "rotated-01.pdf")));
            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(_dest, "rotated-03.pdf")));
            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(_dest, "rotated-02.pdf")));
            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(_dest, "rotated-11.pdf")));
            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.Combine(_dest, "rotated-10.pdf")));
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
            var viewmodel = CreateViewModel();

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
            var viewmodel = CreateViewModel();

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
                Assert.AreEqual(9,   doc.Pages.Count);
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
            var viewmodel = CreateViewModel();

            // メタ情報の変更
            var metadata = new CubePdf.Data.Metadata(viewmodel.Metadata);
            metadata.Title = "TestHistory";
            viewmodel.Metadata = metadata;
            Assert.AreEqual(1, viewmodel.History.Count);
            Assert.AreEqual("TestHistory", viewmodel.Metadata.Title);

            // 暗号化に関する情報の変更
            var encrypt = new CubePdf.Data.Encryption(viewmodel.Encryption);
            encrypt.IsEnabled = true;
            encrypt.OwnerPassword = "owner";
            viewmodel.Encryption = encrypt;
            Assert.AreEqual(2, viewmodel.History.Count);
            Assert.IsTrue(viewmodel.Encryption.IsEnabled);
            Assert.AreEqual("owner", encrypt.OwnerPassword);

            // 回転
            viewmodel.BeginCommand();
            viewmodel.RotateAt(0, 90);
            viewmodel.RotateAt(1, 90);
            viewmodel.RotateAt(2, 90);
            viewmodel.RotateAt(3, 90);
            viewmodel.EndCommand();
            Assert.AreEqual(3, viewmodel.History.Count);
            Assert.AreEqual(90,  viewmodel.ToPage(viewmodel.Items[0]).Rotation);
            Assert.AreEqual(180, viewmodel.ToPage(viewmodel.Items[1]).Rotation);
            Assert.AreEqual(270, viewmodel.ToPage(viewmodel.Items[2]).Rotation);
            Assert.AreEqual(0,   viewmodel.ToPage(viewmodel.Items[3]).Rotation);

            // 挿入
            var added = System.IO.Path.Combine(_src, "readme.pdf");
            viewmodel.Insert(0, added);
            Assert.AreEqual(4, viewmodel.History.Count);
            Assert.AreEqual(11, viewmodel.ItemCount);

            // 削除
            viewmodel.BeginCommand();
            viewmodel.RemoveAt(0);
            viewmodel.RemoveAt(9);
            viewmodel.RemoveAt(3);
            viewmodel.EndCommand();
            Assert.AreEqual(5, viewmodel.History.Count);
            Assert.AreEqual(8, viewmodel.ItemCount);

            // Undo 開始
            viewmodel.Undo();
            Assert.AreEqual(4, viewmodel.History.Count);
            Assert.AreEqual(11, viewmodel.ItemCount);

            viewmodel.Undo();
            Assert.AreEqual(3, viewmodel.History.Count);
            Assert.AreEqual(9, viewmodel.ItemCount);

            viewmodel.Undo();
            Assert.AreEqual(2, viewmodel.History.Count);
            Assert.AreEqual(0,   viewmodel.ToPage(viewmodel.Items[0]).Rotation);
            Assert.AreEqual(90,  viewmodel.ToPage(viewmodel.Items[1]).Rotation);
            Assert.AreEqual(180, viewmodel.ToPage(viewmodel.Items[2]).Rotation);
            Assert.AreEqual(270, viewmodel.ToPage(viewmodel.Items[3]).Rotation);

            viewmodel.Undo();
            Assert.AreEqual(1, viewmodel.History.Count);
            Assert.IsFalse(viewmodel.Encryption.IsEnabled);
            Assert.IsNullOrEmpty(viewmodel.Encryption.OwnerPassword);

            viewmodel.Undo();
            Assert.AreEqual(0, viewmodel.History.Count);
            Assert.AreEqual("CubePdfTests", viewmodel.Metadata.Title);

            viewmodel.Close();
        }

        #endregion

        #region Utility methods

        /* ----------------------------------------------------------------- */
        ///
        /// CreateViewModel
        /// 
        /// <summary>
        /// サンプルとなる PDF ファイルを開いた ListViewModel オブジェクトを
        /// 生成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private CubePdf.Wpf.ListViewModel CreateViewModel(string alternate = null)
        {
            var src = String.IsNullOrEmpty(alternate) ? System.IO.Path.Combine(_src, "rotated.pdf") : alternate;
            Assert.IsTrue(System.IO.File.Exists(src));

            var dest = new CubePdf.Wpf.ListViewModel();
            dest.ItemWidth = 64;
            dest.Open(src);
            return dest;
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
