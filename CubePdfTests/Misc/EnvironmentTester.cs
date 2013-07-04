/* ------------------------------------------------------------------------- */
///
/// Misc/EnvironmentTester.cs
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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NUnit.Framework;

namespace CubePdfTests.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// SystemEnvironmentTester
    /// 
    /// <summary>
    /// SystemEnvironment クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class EnvironmentTester
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

        /* ----------------------------------------------------------------- */
        ///
        /// TestGetRecentFiles
        /// 
        /// <summary>
        /// 「最近使ったファイル一覧」を取得するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestGetRecentFiles()
        {
            var path = System.IO.Path.Combine(_dest, "TestGetRecentFiles.txt");
            using (var writer = new System.IO.StreamWriter(path)) writer.WriteLine("test");
            Win32Api.SHAddToRecentDocs(ShellAddToRecentDocsFlags.Path, path);
            using (var reader = new System.IO.StreamReader(path)) { }
            var results = CubePdf.Misc.Environment.GetRecentFiles("*.txt");
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Contains(path));
        }

        #region Win32 APIs

        internal enum ShellAddToRecentDocsFlags { Pidl = 0x001, Path = 0x002, }
        internal class Win32Api
        {
            [DllImport("shell32.dll")]
            public static extern void SHAddToRecentDocs(ShellAddToRecentDocsFlags flag, string path);
        }

        #endregion

        #region Variables
        private string _src = string.Empty;
        private string _dest = string.Empty;
        #endregion
    }
}
