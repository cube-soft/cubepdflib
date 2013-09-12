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
using System.Collections.Generic;
using NUnit.Framework;

namespace CubePdfTests.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListProxyTester
    /// 
    /// <summary>
    /// ListProxy クラスのテストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class ListProxyTester
    {
        #region Test methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestNoProxy
        /// 
        /// <summary>
        /// IItemsProvider オブジェクトを指定せずに初期化して、各種操作を
        /// 行うテストです。この場合、ListProxy は通常の IList(T), IList と
        /// 同様の挙動を示します（ただし、一部 NotSupportedException が
        /// 送出される）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestNoProxy()
        {
            var proxy = new CubePdf.Wpf.ListProxy<string>();
            for (int i = 0; i < 5; ++i) proxy.Add("TestNoProxy");
            proxy.RemoveAt(2);
            proxy.Insert(1, "TestNoProxyInsert");
            Assert.AreEqual(5, proxy.Count);
            Assert.AreEqual("TestNoProxy", proxy[0]);
            Assert.AreEqual("TestNoProxyInsert", proxy[1]);
            Assert.AreEqual("TestNoProxy", proxy[2]);
            Assert.AreEqual("TestNoProxy", proxy[3]);
            Assert.AreEqual("TestNoProxy", proxy[4]);
            proxy.Clear();
            Assert.AreEqual(0, proxy.Count);
            proxy.Add("Dummy");

            try
            {
                proxy[0] = "NotSupported";
                Assert.Fail("never reached");
            }
            catch (NotSupportedException /* err */) { Assert.Pass(); }

            try
            {
                string[] copyto = { "a", "b", "c" };
                proxy.CopyTo(copyto, 0);
            }
            catch (NotSupportedException /* err */) { Assert.Pass(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestProxy
        /// 
        /// <summary>
        /// DummyItemsProvider オブジェクトを中継オブジェクトとして指定した
        /// ListProxy オブジェクトのテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestProxy()
        {
            var proxy = new CubePdf.Wpf.ListProxy<string>(new DummyItemsProvider());
            for (int i = 0; i < 10; ++i) proxy.Add("TestProxy");
            Assert.AreEqual(5, proxy.Count);
            Assert.AreEqual(10, proxy.RawCount);
            for (int i = 0; i < proxy.Count; ++i) Assert.AreEqual("DummyItemsProvider", proxy[i]);
            for (int i = 0; i < proxy.RawCount; ++i) Assert.AreEqual("TestProxy", proxy.RawAt(i));
        }

        #region Internal classes

        /* ----------------------------------------------------------------- */
        ///
        /// DummyItemsProvider
        /// 
        /// <summary>
        /// 内部状態に関わらず一定のカウントと内容を返すダミー用の
        /// IItemsProvider 実装クラスです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        internal class DummyItemsProvider : CubePdf.Wpf.IItemsProvider<string>
        {
            public int ProvideItemsCount() { return 5; }
            public string ProvideItem(int index)
            {
                if (index < 0 || index >= 5) throw new IndexOutOfRangeException();
                return "DummyItemsProvider";
            }
        }

        #endregion

        #endregion
    }
}
