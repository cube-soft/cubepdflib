/* ------------------------------------------------------------------------- */
///
/// Data/StringConverterTester.cs
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

namespace CubePdfTests.Data
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
            var dest = CubePdf.Data.StringConverter.ParseRange(src);
            Assert.AreEqual(1, dest.Count);
            Assert.AreEqual(dest[0], 3);

            src  = "1,2,3";
            dest = CubePdf.Data.StringConverter.ParseRange(src);
            Assert.AreEqual(dest.Count, 3);
            Assert.AreEqual(dest[0], 1);
            Assert.AreEqual(dest[1], 2);
            Assert.AreEqual(dest[2], 3);

            src  = "1,3,5";
            dest = CubePdf.Data.StringConverter.ParseRange(src);
            Assert.AreEqual(dest.Count, 3);
            Assert.AreEqual(dest[0], 1);
            Assert.AreEqual(dest[1], 3);
            Assert.AreEqual(dest[2], 5);

            src  = "1,2-4,6";
            dest = CubePdf.Data.StringConverter.ParseRange(src);
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
                dest = CubePdf.Data.StringConverter.ParseRange(src);
                Assert.Fail("never reached");
            }
            catch (ArgumentException /* err */) { Assert.Pass(); }

            try
            {
                src  = "1,2-4-5,6";
                dest = CubePdf.Data.StringConverter.ParseRange(src);
                Assert.Fail("never reached");
            }
            catch (ArgumentException /* err */) { Assert.Pass(); }
        }

        #endregion
    }
}
