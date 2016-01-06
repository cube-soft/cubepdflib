/* ------------------------------------------------------------------------- */
///
/// Wpf/ListViewModelTester.cs
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
        [TestCase("rotated.pdf",         "")]
        [TestCase("password-aes256.pdf", "password")]
        public void TestOpen(string filename, string password)
        {
            try
            {
                var viewmodel = CreateViewModel(System.IO.Path.Combine(_src, filename), password);
                Assert.IsFalse(viewmodel.IsModified);
                Assert.AreEqual(CubePdf.Wpf.ListViewItemVisibility.Normal, viewmodel.ItemVisibility);
                Assert.IsNotNullOrEmpty(viewmodel.FilePath);
                Assert.NotNull(viewmodel.Pages);
                Assert.IsTrue(viewmodel.Pages.Count > 0);
                Assert.NotNull(viewmodel.Metadata);
                Assert.NotNull(viewmodel.Metadata.Version);
                Assert.NotNull(viewmodel.Encryption);
                Assert.IsTrue(viewmodel.Encryption.IsEnabled || string.IsNullOrEmpty(password));
                Assert.AreEqual(password, viewmodel.Encryption.OwnerPassword);
                Assert.NotNull(viewmodel.Encryption.Permission);
                Assert.AreEqual(30, viewmodel.MaxHistoryCount);
                Assert.NotNull(viewmodel.History);
                Assert.AreEqual(0, viewmodel.History.Count);
                Assert.NotNull(viewmodel.UndoHistory);
                Assert.AreEqual(0, viewmodel.UndoHistory.Count);
                Assert.IsEmpty(viewmodel.BackupFolder);
                Assert.AreEqual(0, viewmodel.BackupDays);
                viewmodel.Close();
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
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
        /// TestOverwrite
        /// 
        /// <summary>
        /// 上書き保存のテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestOverwrite()
        {
            var dest = System.IO.Path.Combine(_dest, "TestListViewModelSave.pdf");
            try
            {
                var viewmodel = CreateViewModel();
                System.IO.File.Delete(dest);
                viewmodel.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));
                viewmodel.Close();
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }

            try
            {
                var viewmodel = CreateViewModel(dest);
                viewmodel.RemoveAt(0);
                viewmodel.Save();
                Assert.IsTrue(System.IO.File.Exists(dest));
                viewmodel.Close();
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(8, doc.Pages.Count);
            }

        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestSaveOnClose
        /// 
        /// <summary>
        /// PDF ファイルを閉じる際に、保存処理を行うテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestSaveOnClose()
        {
            var viewmodel = CreateViewModel();
            var dest = System.IO.Path.Combine(_dest, "TestListViewModelSaveOnClose.pdf");
            System.IO.File.Delete(dest);
            viewmodel.SaveOnClose(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));
            Assert.IsTrue(String.IsNullOrEmpty(viewmodel.FilePath));
            Assert.AreEqual(0, viewmodel.Pages.Count);
            Assert.AreEqual(0, viewmodel.History.Count);
            Assert.AreEqual(0, viewmodel.UndoHistory.Count);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestBackup
        /// 
        /// <summary>
        /// バックアップファイルの作成を行うテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestBackup()
        {
            var filename = "TestListViewModelBackup.pdf";
            var original = System.IO.Path.Combine(_src, "rotated.pdf");
            var src = System.IO.Path.Combine(_dest, filename);
            System.IO.File.Copy(original, src, true);
            Assert.IsTrue(System.IO.File.Exists(src));

            var folder = System.IO.Path.Combine(_dest, "Backup");
            var backup = System.IO.Path.Combine(folder, String.Format("{0}\\{1}", DateTime.Today.ToString("yyyyMMdd"), filename));
            if (System.IO.Directory.Exists(folder)) System.IO.Directory.Delete(folder, true);

            var viewmodel = CreateViewModel(src);
            viewmodel.Save();
            Assert.IsFalse(System.IO.File.Exists(backup));

            viewmodel.BackupFolder = folder;
            viewmodel.BackupDays = 30;
            viewmodel.Save();
            Assert.IsTrue(System.IO.File.Exists(backup));

            System.IO.File.Delete(backup);
            Assert.IsFalse(System.IO.File.Exists(backup));
            viewmodel.SaveOnClose();
            Assert.IsTrue(System.IO.File.Exists(backup));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestDeleteBackup
        /// 
        /// <summary>
        /// バックアップ保存期間の過ぎたファイルを消去するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestDeleteBackup()
        {
            var filename = "TestListViewModelDeleteBackup.pdf";
            var original = System.IO.Path.Combine(_src, "rotated.pdf");
            var src = System.IO.Path.Combine(_dest, filename);
            System.IO.File.Copy(original, src, true);
            Assert.IsTrue(System.IO.File.Exists(src));

            var folder = System.IO.Path.Combine(_dest, "Backup");
            var today = System.IO.Path.Combine(folder, DateTime.Today.ToString("yyyyMMdd"));
            var old = System.IO.Path.Combine(folder, "19830214");
            System.IO.Directory.CreateDirectory(old);
            if (System.IO.Directory.Exists(today)) System.IO.Directory.Delete(today, true);

            using (var viewmodel = new CubePdf.Wpf.ListViewModel())
            {
                viewmodel.ViewSize = 64;
                viewmodel.BackupFolder = folder;
                viewmodel.BackupDays = 30;
                viewmodel.Open(src);
                viewmodel.SaveOnClose();
                Assert.IsTrue(System.IO.Directory.Exists(today));
                Assert.IsTrue(System.IO.Directory.Exists(old));
            }

            Assert.IsTrue(System.IO.Directory.Exists(today));
            Assert.IsFalse(System.IO.Directory.Exists(old));

            try
            {
                System.IO.Directory.Delete(folder, true);
                using (var viewmodel = new CubePdf.Wpf.ListViewModel())
                {
                    viewmodel.ViewSize = 64;
                    viewmodel.BackupFolder = folder;
                    viewmodel.BackupDays = 30;
                }
            }
            catch (Exception err)
            {
                Assert.Fail(err.ToString());
            }
        }

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
            Assert.IsTrue(viewmodel.IsModified);
            Assert.AreEqual(11, viewmodel.Pages.Count);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelInsert.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(11,  doc.Pages.Count);
                Assert.AreEqual(0,   doc.GetPage(1).Rotation);
                Assert.AreEqual(90,  doc.GetPage(2).Rotation);
                Assert.AreEqual(0,   doc.GetPage(3).Rotation);
                Assert.AreEqual(0,   doc.GetPage(4).Rotation);
                Assert.AreEqual(180, doc.GetPage(5).Rotation);
                Assert.AreEqual(270, doc.GetPage(6).Rotation);
                Assert.AreEqual(0,   doc.GetPage(7).Rotation);
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
            Assert.IsTrue(viewmodel.IsModified);
            Assert.AreEqual(11, viewmodel.Pages.Count);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelAdd.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Dispose();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(11, doc.Pages.Count);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestAddDuplicatedFiles
        /// 
        /// <summary>
        /// 同じ PDF ファイルを重複して追加するテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestAddDuplicatedFiles()
        {
            try
            {
                var viewmodel = CreateViewModel();
                var src = System.IO.Path.Combine(_src, "readme.pdf");
                Assert.IsTrue(System.IO.File.Exists(src));
                viewmodel.Add(src);
                viewmodel.Add(src);
                Assert.IsTrue(viewmodel.IsModified);
                Assert.AreEqual(13, viewmodel.Pages.Count);

                var dest = System.IO.Path.Combine(_dest, "TestListViewModelAdd2.pdf");
                System.IO.File.Delete(dest);
                viewmodel.Save(dest);
                Assert.IsTrue(System.IO.File.Exists(dest));

                viewmodel.Dispose();

                using (var doc = new CubePdf.Editing.DocumentReader(dest))
                {
                    Assert.AreEqual(13, doc.Pages.Count);
                }
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
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
            viewmodel.RemoveAt(viewmodel.Pages.Count - 1);
            Assert.IsTrue(viewmodel.IsModified);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelRemoveAt.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(6,   doc.Pages.Count);
                Assert.AreEqual(90,  doc.GetPage(1).Rotation);
                Assert.AreEqual(180, doc.GetPage(2).Rotation);
                Assert.AreEqual(0,   doc.GetPage(3).Rotation);
                Assert.AreEqual(0,   doc.GetPage(4).Rotation);
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
            viewmodel.Remove(viewmodel.Items[viewmodel.Pages.Count - 1]);
            Assert.IsTrue(viewmodel.IsModified);

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
                Assert.AreEqual(0,   doc.GetPage(1).Rotation);
                Assert.AreEqual(180, doc.GetPage(2).Rotation);
                Assert.AreEqual(90,  doc.GetPage(3).Rotation);
                Assert.AreEqual(0,   doc.GetPage(4).Rotation);
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
            Assert.AreEqual(11, viewmodel.Pages.Count);

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
            viewmodel.Move(2, viewmodel.Pages.Count - 1);
            Assert.IsTrue(viewmodel.IsModified);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelMove.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(9,   doc.Pages.Count);
                Assert.AreEqual(90,  doc.GetPage(1).Rotation);
                Assert.AreEqual(180, doc.GetPage(2).Rotation);
                Assert.AreEqual(0,   doc.GetPage(3).Rotation);
                Assert.AreEqual(0,   doc.GetPage(4).Rotation);
                Assert.AreEqual(270, doc.GetPage(9).Rotation);
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
            Assert.IsTrue(viewmodel.IsModified);

            var dest = System.IO.Path.Combine(_dest, "TestListViewModelRotate.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.IsTrue(System.IO.File.Exists(dest));

            viewmodel.Close();

            using (var doc = new CubePdf.Editing.DocumentReader(dest))
            {
                Assert.AreEqual(9,   doc.Pages.Count);
                Assert.AreEqual(90,  doc.GetPage(1).Rotation);
                Assert.AreEqual(270, doc.GetPage(2).Rotation);
                Assert.AreEqual(90,  doc.GetPage(3).Rotation);
                Assert.AreEqual(180, doc.GetPage(4).Rotation);
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
            Assert.AreEqual(11, viewmodel.Pages.Count);

            // 削除
            viewmodel.BeginCommand();
            viewmodel.RemoveAt(0);
            viewmodel.RemoveAt(9);
            viewmodel.RemoveAt(3);
            viewmodel.EndCommand();
            Assert.AreEqual(5, viewmodel.History.Count);
            Assert.AreEqual(8, viewmodel.Pages.Count);

            // 移動
            viewmodel.BeginCommand();
            viewmodel.Move(0, 2);
            viewmodel.Move(1, 7);
            viewmodel.Move(4, 2);
            viewmodel.EndCommand();
            Assert.AreEqual(6, viewmodel.History.Count);

            // Undo 開始
            viewmodel.Undo();
            Assert.AreEqual(5, viewmodel.History.Count);

            viewmodel.Undo();
            Assert.AreEqual(4, viewmodel.History.Count);
            Assert.AreEqual(11, viewmodel.Pages.Count);

            viewmodel.Undo();
            Assert.AreEqual(3, viewmodel.History.Count);
            Assert.AreEqual(9, viewmodel.Pages.Count);

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

        /* ----------------------------------------------------------------- */
        ///
        /// TestIsModified
        /// 
        /// <summary>
        /// 編集されたかどうかの判定条件のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestIsModified()
        {
            var viewmodel = CreateViewModel();
            viewmodel.MaxHistoryCount = 10;

            for (int i = 0; i < 10; ++i) viewmodel.Move(0, 1);
            Assert.IsTrue(viewmodel.IsModified);

            for (int i = 0; i < 10; ++i) viewmodel.Undo();
            Assert.IsFalse(viewmodel.IsModified);

            for (int i = 0; i < 20; ++i) viewmodel.Move(0, 1);
            Assert.IsTrue(viewmodel.IsModified);

            for (int i = 0; i < 20; ++i) viewmodel.Undo();
            Assert.IsTrue(viewmodel.IsModified);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestRunCompleted
        /// 
        /// <summary>
        /// RunCompleted イベントのテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestRunCompleted()
        {
            var count = 0;
            var viewmodel = new CubePdf.Wpf.ListViewModel();
            viewmodel.ViewSize = 64;
            viewmodel.RunCompleted += (sender, e) => {
                ++count;
            };

            // Open
            var src = System.IO.Path.Combine(_src, "rotated.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            viewmodel.Open(src);
            Assert.AreEqual(1, count);

            // Add
            src = System.IO.Path.Combine(_src, "readme.pdf");
            Assert.IsTrue(System.IO.File.Exists(src));
            viewmodel.Add(src);
            Assert.AreEqual(2, count);

            // Remove
            Assert.AreEqual(11, viewmodel.Pages.Count);
            viewmodel.RemoveAt(0);
            Assert.AreEqual(3, count);
            viewmodel.BeginCommand();
            viewmodel.RemoveAt(0);
            viewmodel.RemoveAt(0);
            viewmodel.RemoveAt(0);
            viewmodel.EndCommand();
            Assert.AreEqual(4, count);
            Assert.AreEqual(7, viewmodel.Pages.Count);

            // Move
            viewmodel.Move(0, 1);
            Assert.AreEqual(5, count);
            viewmodel.BeginCommand();
            viewmodel.Move(1, 3);
            viewmodel.Move(3, 4);
            viewmodel.Move(4, 5);
            viewmodel.EndCommand();
            Assert.AreEqual(6, count);

            // Rotate
            Assert.AreEqual(0, viewmodel.GetPage(1).Rotation);
            viewmodel.RotateAt(0, 180);
            Assert.AreEqual(7, count);
            viewmodel.BeginCommand();
            viewmodel.RotateAt(0, 90);
            viewmodel.RotateAt(0, 270);
            viewmodel.RotateAt(0, -90);
            viewmodel.EndCommand();
            Assert.AreEqual(8, count);
            Assert.AreEqual(90, viewmodel.GetPage(1).Rotation);

            // Extract
            IList<CubePdf.Data.PageBase> pages = new List<CubePdf.Data.PageBase>();
            pages.Add(viewmodel.GetPage(1));
            pages.Add(viewmodel.GetPage(2));
            pages.Add(viewmodel.GetPage(3));
            var dest = System.IO.Path.Combine(_dest, "TestListViewModelRunCompletedExtract.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Extract(pages, dest);
            Assert.AreEqual(9, count);
            Assert.IsTrue(System.IO.File.Exists(dest)); 

            // Split
            viewmodel.Split(pages, _dest);
            Assert.AreEqual(10, count);

            // Reset
            viewmodel.Reset();
            Assert.AreEqual(11, count);

            // Undo
            viewmodel.Undo();
            Assert.AreEqual(12, count);

            // Redo
            viewmodel.Redo();
            Assert.AreEqual(13, count);

            // Metadata
            var metadata = new CubePdf.Data.Metadata(viewmodel.Metadata);
            viewmodel.Metadata = metadata;
            Assert.AreEqual(14, count);

            // Encryption
            var encrypt = new CubePdf.Data.Encryption(viewmodel.Encryption);
            viewmodel.Encryption = encrypt;
            Assert.AreEqual(15, count);

            // Save
            dest = System.IO.Path.Combine(_dest, "TestListViewModelRunCompletedSave.pdf");
            System.IO.File.Delete(dest);
            viewmodel.Save(dest);
            Assert.AreEqual(16, count);
            Assert.IsTrue(System.IO.File.Exists(dest));

            // Close
            viewmodel.Close();
            Assert.AreEqual(17, count);
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
        private CubePdf.Wpf.ListViewModel CreateViewModel(string alternate = null, string password = "")
        {
            var src = String.IsNullOrEmpty(alternate) ? System.IO.Path.Combine(_src, "rotated.pdf") : alternate;
            Assert.IsTrue(System.IO.File.Exists(src));

            var dest = new CubePdf.Wpf.ListViewModel();
            dest.ViewSize = 64;
            dest.Open(src, password);
            return dest;
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
