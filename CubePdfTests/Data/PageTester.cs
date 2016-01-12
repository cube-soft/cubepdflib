/* ------------------------------------------------------------------------- */
///
/// Data/PageTester.cs
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
using System.Collections.Generic;
using NUnit.Framework;
using CubePdf.Data.Extensions;

namespace CubePdfTests.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// PageTester
    /// 
    /// <summary>
    /// CubePdf.Data.Page クラスのテストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class PageTester
    {
        /* ----------------------------------------------------------------- */
        ///
        /// TestConstruct
        /// 
        /// <summary>
        /// Page クラスの初期化のテストを行います。
        /// 
        /// TODO: ページ番号が非常に大きい場合や負数の場合に何らかの処理を
        /// 行うべきかどうか検討する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("example.pdf", 10)]
        [TestCase("C:\\file\\to\\path.pdf", 256)]
        [TestCase("negative-pages.pdf", -1)]
        [TestCase("fuge-pages.pdf", 1234567890)]
        public void TestConstruct(string path, int pagenum)
        {
            var page = new CubePdf.Data.Page(path, pagenum);
            Assert.AreEqual(path, page.FilePath);
            Assert.AreEqual(pagenum, page.PageNumber);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestCopy
        /// 
        /// <summary>
        /// コピーコンストラクタのテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestCopy()
        {
            var page = new CubePdf.Data.Page("example.pdf", 10);
            page.Rotation = 90;
            page.Power = 0.5;
            page.Password = "Test";

            var copied = page.Copy() as CubePdf.Data.Page;
            copied.FilePath = "copied.pdf";
            copied.PageNumber = 20;
            copied.Rotation = 45;
            copied.Power = 2.5;

            Assert.AreEqual("example.pdf", page.FilePath);
            Assert.AreEqual(10, page.PageNumber);
            Assert.AreEqual(90, page.Rotation);
            Assert.AreEqual(0.5, page.Power);

            Assert.AreEqual("copied.pdf", copied.FilePath);
            Assert.AreEqual("Test", copied.Password);
            Assert.AreEqual(20, copied.PageNumber);
            Assert.AreEqual(45, copied.Rotation);
            Assert.AreEqual(2.5, copied.Power);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestEquals
        /// 
        /// <summary>
        /// Equals メソッドのテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestEquals()
        {
            var page1 = new CubePdf.Data.Page("foo.pdf", 1);
            var page2 = new CubePdf.Data.Page("foo.pdf", 1);
            var page3 = new CubePdf.Data.Page("foo.pdf", 2);
            var page4 = new CubePdf.Data.Page("bas.pdf", 3);

            Assert.IsTrue(page1.Equals(page1));
            Assert.IsFalse(page1 == page2);
            Assert.IsTrue(page1.Equals(page2));
            Assert.IsFalse(page1.Equals(page3));
            Assert.IsFalse(page1.Equals(page4));

            var page5 = page2 as CubePdf.Data.PageBase;
            Assert.IsTrue(page1.Equals(page5));
            Assert.IsTrue(page5.Equals(page1));

            var page6 = page3 as CubePdf.Data.PageBase;
            Assert.IsFalse(page1.Equals(page6));
            Assert.IsFalse(page6.Equals(page1));

            var list = new List<CubePdf.Data.PageBase>();
            list.Add(page1);
            list.Add(page3);
            list.Add(page4);
            Assert.IsTrue(list.Contains(new CubePdf.Data.Page("foo.pdf", 1)));
            Assert.AreEqual(0, list.IndexOf(new CubePdf.Data.Page("foo.pdf", 1)));
            Assert.IsFalse(list.Contains(new CubePdf.Data.Page("notfound.pdf", 1)));
            Assert.AreEqual(-1, list.IndexOf(new CubePdf.Data.Page("notfound.pdf", 1)));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestViewSize
        /// 
        /// <summary>
        /// 指定されたサイズ、角度、倍率によって ViewSize プロパティが
        /// 適切な値を返すかどうかをテストします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase(0, 1.0)]
        [TestCase(90, 1.0)]
        [TestCase(180, 1.0)]
        [TestCase(270, 1.0)]
        [TestCase(450, 1.0)]
        [TestCase(-90, 1.0)]
        [TestCase(30, 1.0)]
        [TestCase(45, 1.0)]
        [TestCase(218, 1.0)]
        [TestCase(90, 5.0)]
        [TestCase(180, 0.5)]
        public void TestViewSize(int rotation, double power)
        {
            var page = new CubePdf.Data.Page("esample.pdf", 10);
            Assert.AreEqual(0, page.ViewSize().Width);
            Assert.AreEqual(0, page.ViewSize().Height);

            page.OriginalSize = new System.Drawing.Size(600, 400);
            Assert.AreEqual(page.OriginalSize.Width, page.ViewSize().Width);
            Assert.AreEqual(page.OriginalSize.Height, page.ViewSize().Height);

            page.Rotation = rotation;
            page.Power = power;

            if (rotation < 0) rotation += 360;
            else if (rotation >= 360) rotation -= 360;

            var radian = Math.PI * rotation / 180.0;
            var sin = Math.Abs(Math.Sin(radian));
            var cos = Math.Abs(Math.Cos(radian));
            var width = (int)((page.OriginalSize.Width * cos + page.OriginalSize.Height * sin) * power);
            var height = (int)((page.OriginalSize.Width * sin + page.OriginalSize.Height * cos) * power);
            Assert.AreEqual(width, page.ViewSize().Width);
            Assert.AreEqual(height, page.ViewSize().Height);
        }
    }
}
