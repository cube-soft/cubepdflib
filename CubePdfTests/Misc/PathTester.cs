/* ------------------------------------------------------------------------- */
///
/// Misc/PathTester.cs
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

namespace CubePdfTests.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// PathTester
    /// 
    /// <summary>
    /// CubePdf.Misc.Path クラスのテストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class PathTester
    {
        /* ----------------------------------------------------------------- */
        ///
        /// TestNormalizePath
        /// 
        /// <summary>
        /// パスの正規化のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase(@"C:\foo\bar\bas.txt",            @"C:\foo\bar\bas.txt",            true)]
        [TestCase(@"\\server\foo\bar\bas.txt",      @"\\server\foo\bar\bas.txt",      true)]
        [TestCase(@"C:\foo\bar\bas______.txt",      @"C:\foo\bar\bas/*?<>|.txt",     false)]
        [TestCase(@"C:\foo\bar\bas_.txt",           @"C:\foo\bar\bas:.txt",          false)]
        [TestCase(@"C:\foo\bar\bas_\hoge.txt",      @"C:\foo\bar\bas:\hoge.txt",     false)]
        [TestCase(@"c:\foo\bar\bas.txt",            @"c:\foo\\bar\bas.txt",           true)]
        [TestCase(@"\\?\c:\foo\bar\bas.txt",        @"\\?\c:\foo\bar\bas.txt",        true)]
        [TestCase(@"C:\foo\bar\bas.txt",            @"C:\foo\bar\bas.txt . ... ",    false)]
        [TestCase(@"\\?\C:\foo\bar\bas.txt . ... ", @"\\?\C:\foo\bar\bas.txt . ... ", true)]
        public void TestNormalizePath(string expected, string src, bool valid)
        {
            Assert.AreEqual(expected, CubePdf.Misc.Path.Normalize(src, '_'));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestNormalizeFilename
        /// 
        /// <summary>
        /// ファイル名の正規化のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase(@"foo.txt",                      @"foo.txt",                      true)]
        [TestCase(@"symbols!#$%&'()=~`{}@;,..txt", @"symbols!#$%&'()=~`{}@;,..txt", true)]
        [TestCase(@"C__foo_bar_bas.txt",           @"C:\foo\bar\bas.txt",          false)]
        [TestCase(@"__server_foo_bar_bas.txt",     @"\\server\foo\bar\bas.txt",    false)]
        [TestCase(@"C__foo_bar_bas______.txt",     @"C:\foo\bar\bas/*?<>|.txt",    false)]
        [TestCase(@"____c__foo_bar_bas.txt",       @"\\?\c:\foo\bar\bas.txt",      false)]
        [TestCase(@"C__foo_bar_bas.txt",           @"C:\foo\bar\bas.txt . ... ",   false)]
        public void TestNormalizeFilename(string expected, string src, bool valid)
        {
            Assert.AreEqual(expected, CubePdf.Misc.Path.NormalizeFilename(src, '_'));
        }
    }
}
