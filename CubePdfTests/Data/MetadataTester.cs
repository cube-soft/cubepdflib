/* ------------------------------------------------------------------------- */
///
/// Data/EncryptionTester.cs
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
using NUnit.Framework;

namespace CubePdfTests.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// MetadataTester
    /// 
    /// <summary>
    /// CubePdf.Data.Metadata クラスのテストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class MetadataTester
    {
        /* ----------------------------------------------------------------- */
        ///
        /// TestConstruct
        /// 
        /// <summary>
        /// Metadata クラスの初期化のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestConstruct()
        {
            var meta = new CubePdf.Data.Metadata();
            Assert.AreEqual(0, meta.Author.Length);
            Assert.AreEqual(0, meta.Title.Length);
            Assert.AreEqual(0, meta.Subtitle.Length);
            Assert.AreEqual(0, meta.Keywords.Length);
            Assert.AreEqual("CubePDF", meta.Creator);
            Assert.AreEqual(0, meta.Producer.Length);
        }
    }
}
