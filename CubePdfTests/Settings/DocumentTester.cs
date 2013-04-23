/* ------------------------------------------------------------------------- */
///
/// Settings/DocumentTester.cs
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
using Microsoft.Win32;
using NUnit.Framework;

namespace CubePdfTests.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentTester
    /// 
    /// <summary>
    /// Document クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class DocumentTester
    {
        #region Setup and TearDown

        /* ----------------------------------------------------------------- */
        /// Setup
        /* ----------------------------------------------------------------- */
        [SetUp]
        public void Setup() {
            var subkey = Registry.CurrentUser.CreateSubKey(_root);
            if (subkey == null) throw new ArgumentException("setup");
        }

        /* ----------------------------------------------------------------- */
        /// TearDown
        /* ----------------------------------------------------------------- */
        [TearDown]
        public void TearDown()
        {
            Registry.CurrentUser.DeleteSubKeyTree(_root);
        }

        #endregion

        #region Test methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestReadRegistry
        /// 
        /// <summary>
        /// レジストリから設定を読み込むテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestReadRegistry()
        {
            var subkey = Registry.CurrentUser.CreateSubKey(_root);
            CleanRegistry(subkey);
            Assert.NotNull(subkey);
            WriteRegistryTestData(subkey);

            var doc = new CubePdf.Settings.Document();
            doc.Read(subkey);
            Assert.AreEqual(6, doc.Root.Count);
            Assert.IsTrue(doc.Root.Contains("sample"));
            Assert.IsTrue(doc.Root.Contains("日本語のサブキー"));

            var node = doc.Root.Find("SampleString");
            Assert.NotNull(node);
            Assert.AreEqual("Hello, world!", node.GetValue("error"));

            node = doc.Root.Find("文字列サンプル");
            Assert.NotNull(node);
            Assert.AreEqual("こんにちはこんにちは！", node.GetValue("error"));

            node = doc.Root.Find("数値サンプル");
            Assert.NotNull(node);
            Assert.AreEqual(1024, node.GetValue(-1));

            node = doc.Root.Find("論理値サンプル");
            Assert.NotNull(node);
            Assert.AreEqual(false, node.GetValue(true));

            node = doc.Root.Find("NotExist");
            Assert.IsNull(node);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestWriteRegistry
        /// 
        /// <summary>
        /// レジストリへ設定を書き込むテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestWriteRegistry()
        {
            var subkey = Registry.CurrentUser.CreateSubKey(_root);
            CleanRegistry(subkey);
            Assert.NotNull(subkey);
            WriteRegistryTestData(subkey);

            var element = new CubePdf.Settings.NodeSet();
            element.Add(new CubePdf.Settings.Node("string", "こんにちはこんにちは！"));
            element.Add(new CubePdf.Settings.Node("number", 324234));
            element.Add(new CubePdf.Settings.Node("boolean", true));
            var doc = new CubePdf.Settings.Document();
            doc.Root.Add(new CubePdf.Settings.Node("example", element));

            doc.Write(subkey);
            doc.Clear();
            Assert.AreEqual(0, doc.Root.Count);
            
            doc.Read(subkey);
            Assert.AreEqual(7, doc.Root.Count); // Write + 既に存在していた分
            var node = doc.Root.Find("example");
            Assert.NotNull(node);
            element = node.Value as  CubePdf.Settings.NodeSet;
            Assert.NotNull(element);

            node = element.Find("string");
            Assert.NotNull(node);
            Assert.AreEqual("こんにちはこんにちは！", node.GetValue("error"));

            node = element.Find("number");
            Assert.NotNull(node);
            Assert.AreEqual(324234, node.GetValue(-1));

            node = element.Find("boolean");
            Assert.NotNull(node);
            Assert.AreEqual(true, node.GetValue(false));

            node = element.Find("NotExist");
            Assert.IsNull(node);
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// WriteRegistryTestData
        /// 
        /// <summary>
        /// root 下にテスト用のレジストリを書き込みます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void WriteRegistryTestData(RegistryKey root)
        {
            var subkey = root.CreateSubKey("sample");
            subkey.SetValue("SubKeyValue", "test");

            subkey = root.CreateSubKey("日本語のサブキー");
            subkey.SetValue("日本語の値", 32);

            root.SetValue("SampleString", "Hello, world!");
            root.SetValue("文字列サンプル", "こんにちはこんにちは！");
            root.SetValue("数値サンプル", 1024);
            byte[] data = { 0 };
            root.SetValue("論理値サンプル", data, RegistryValueKind.Binary);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CleanRegistry
        /// 
        /// <summary>
        /// root 下の全てのレジストリを消去します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CleanRegistry(RegistryKey root)
        {
            foreach (var name in root.GetSubKeyNames()) root.DeleteSubKeyTree(name);
            foreach (var name in root.GetValueNames())  root.DeleteValue(name);
        }

        #endregion

        #region Variables
        private string _root = @"Software\CubeSoft\CubePdfLibTest";
        #endregion
    }
}
