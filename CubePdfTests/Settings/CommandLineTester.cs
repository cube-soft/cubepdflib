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
using NUnit.Framework;

namespace CubePdfTests.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// CommandLineTester
    /// 
    /// <summary>
    /// CommandLine クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class CommandLineTester
    {
        /* ----------------------------------------------------------------- */
        ///
        /// TestParse
        /// 
        /// <summary>
        /// コマンドラインを解析するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestParse()
        {
            var cmdline = new CubePdf.Settings.CommandLine();
            var args = new string[] { "foo", "bar", "bas" };
            cmdline.Add(args);
            Assert.AreEqual(3, cmdline.Values.Count);
            Assert.AreEqual("foo", cmdline.Values[0]);
            Assert.AreEqual("bar", cmdline.Values[1]);
            Assert.AreEqual("bas", cmdline.Values[2]);
            Assert.AreEqual(0, cmdline.Options.Count);

            args = new string[] {
                "日本語引数",
                "空白付 arguments",
                "/bar",
                "bas",
                "この引数はValues行き",
                "/apple",
                "orange",
            };

            cmdline.Clear();
            Assert.AreEqual(0, cmdline.Values.Count);
            Assert.AreEqual(0, cmdline.Options.Count);
            cmdline.Add(args);
            Assert.AreEqual(3, cmdline.Values.Count);
            Assert.AreEqual("日本語引数", cmdline.Values[0]);
            Assert.AreEqual("空白付 arguments", cmdline.Values[1]);
            Assert.AreEqual("この引数はValues行き", cmdline.Values[2]);
            Assert.AreEqual(2, cmdline.Options.Count);
            Assert.AreEqual("bas", cmdline.Options["bar"]);
            Assert.AreEqual("orange", cmdline.Options["apple"]);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestParse
        /// 
        /// <summary>
        /// オプション付き引数用の接頭辞を変更して、コマンドラインを解析
        /// するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestOptionPrefix()
        {
            var args = new string[] {
                "foo",
                "日本語引数",
                "空白付 arguments",
                "-bar",
                "bas",
                "この引数はValues行き",
                "/apple",
                "orange",
            };

            var cmdline = new CubePdf.Settings.CommandLine(args, '-');
            Assert.AreEqual(6, cmdline.Values.Count);
            Assert.AreEqual("foo", cmdline.Values[0]);
            Assert.AreEqual("日本語引数", cmdline.Values[1]);
            Assert.AreEqual("空白付 arguments", cmdline.Values[2]);
            Assert.AreEqual("この引数はValues行き", cmdline.Values[3]);
            Assert.AreEqual("/apple", cmdline.Values[4]);
            Assert.AreEqual("orange", cmdline.Values[5]);
            Assert.AreEqual(1, cmdline.Options.Count);
            Assert.AreEqual("bas", cmdline.Options["bar"]);
        }
    }
}
