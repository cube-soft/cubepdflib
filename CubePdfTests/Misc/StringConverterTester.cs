/* ------------------------------------------------------------------------- */
///
/// Misc/StringConverterTester.cs
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

namespace CubePdfTests.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// StringConverterTester
    /// 
    /// <summary>
    /// StringConverter クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class StringConverterTester
    {
        #region Test Methods

        /* ----------------------------------------------------------------- */
        ///
        /// FormatByteSize
        /// 
        /// <summary>
        /// バイト数を表示するための文字列に変換するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("1.20 KB", 1234)]
        [TestCase("12.0 KB", 12345)]
        [TestCase("120 KB",  123456)]
        [TestCase("1.17 MB", 1234567)]
        [TestCase("11.7 MB", 12345678)]
        [TestCase("117 MB",  123456789)]
        [TestCase("1.14 GB", 1234567890)]
        [TestCase("11.4 GB", 12345678901)]
        [TestCase("114 GB",  123456789012)]
        [TestCase("1.12 TB", 1234567890123)]
        [TestCase("964 KB",  987654)]
        public void FormatByteSize(string expected, long filesize)
        {
            var result = CubePdf.Misc.StringConverter.FormatByteSize(filesize);
            Assert.AreEqual(expected, result);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestParseRange
        /// 
        /// <summary>
        /// 範囲を表す文字列の解析テストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestParseRange()
        {
            var src  = "3";
            var dest = CubePdf.Misc.StringConverter.ParseRange(src);
            Assert.AreEqual(1, dest.Count);
            Assert.AreEqual(dest[0], 3);

            src  = "1,2,3";
            dest = CubePdf.Misc.StringConverter.ParseRange(src);
            Assert.AreEqual(dest.Count, 3);
            Assert.AreEqual(dest[0], 1);
            Assert.AreEqual(dest[1], 2);
            Assert.AreEqual(dest[2], 3);

            src  = "1,3,5";
            dest = CubePdf.Misc.StringConverter.ParseRange(src);
            Assert.AreEqual(dest.Count, 3);
            Assert.AreEqual(dest[0], 1);
            Assert.AreEqual(dest[1], 3);
            Assert.AreEqual(dest[2], 5);

            src  = "1,2-4,6";
            dest = CubePdf.Misc.StringConverter.ParseRange(src);
            Assert.AreEqual(dest.Count, 5);
            Assert.AreEqual(dest[0], 1);
            Assert.AreEqual(dest[1], 2);
            Assert.AreEqual(dest[2], 3);
            Assert.AreEqual(dest[3], 4);
            Assert.AreEqual(dest[4], 6);

            // 解析エラー
            try
            {
                src  = "1,a,b,c,5-8";
                dest = CubePdf.Misc.StringConverter.ParseRange(src);
                Assert.Fail("never reached");
            }
            catch (ArgumentException /* err */) { Assert.Pass(); }

            try
            {
                src  = "1,2-4-5,6";
                dest = CubePdf.Misc.StringConverter.ParseRange(src);
                Assert.Fail("never reached");
            }
            catch (ArgumentException /* err */) { Assert.Pass(); }
        }

        #endregion
    }
}
