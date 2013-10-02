/* ------------------------------------------------------------------------- */
///
/// Wpf/IndexTableTester.cs
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
    /// IndexTableTester
    /// 
    /// <summary>
    /// IndexTable クラスのテストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class IndexTableTester
    {
        #region Test methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestUpdate
        /// 
        /// <summary>
        /// インデックス管理テーブルの更新処理をテストします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestUpdate()
        {
            try
            {
                var table = new CubePdf.Wpf.IndexTable();
                Assert.IsNull(table.Items);
                table.Capacity = 5;
                Assert.AreEqual(5, table.Capacity);

                table.Update(0);
                table.Update(1);
                table.Update(2);
                table.Update(3);
                table.Update(4);
                Assert.AreEqual(5, table.Indices.Count);

                table.Update(6);
                Assert.AreEqual(5, table.Indices.Count);
                Assert.IsFalse(table.Indices.Contains(0));

                table.Update(10);
                Assert.AreEqual(5, table.Indices.Count);
                Assert.IsFalse(table.Indices.Contains(1));

                table.Update(11);
                Assert.AreEqual(5, table.Indices.Count);
                Assert.IsFalse(table.Indices.Contains(2));

                table.Update(0);
                Assert.AreEqual(5, table.Indices.Count);
                Assert.IsFalse(table.Indices.Contains(11));

                // 0, 3, 4, 6, 10
                // ちょうど真ん中の値 (5) を Update するテスト
                Assert.IsTrue(table.Indices.Contains(0));
                Assert.IsTrue(table.Indices.Contains(3));
                Assert.IsTrue(table.Indices.Contains(4));
                Assert.IsTrue(table.Indices.Contains(6));
                Assert.IsTrue(table.Indices.Contains(10));
                table.Update(5);
                Assert.AreEqual(5, table.Indices.Count);
                Assert.IsTrue(table.Indices.Contains(0));
                Assert.IsFalse(table.Indices.Contains(10));

                // 既に存在する値で Update
                table.Update(4);
                Assert.AreEqual(5, table.Indices.Count);
                Assert.IsTrue(table.Indices.Contains(0));
                Assert.IsTrue(table.Indices.Contains(3));
                Assert.IsTrue(table.Indices.Contains(4));
                Assert.IsTrue(table.Indices.Contains(5));
                Assert.IsTrue(table.Indices.Contains(6));
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestChangeCapacity
        /// 
        /// <summary>
        /// 途中で Capacity の値を変更した場合のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestChangeCapacity()
        {
            try
            {
                var table = new CubePdf.Wpf.IndexTable();
                Assert.IsNull(table.Items);
                Assert.AreEqual(0, table.Capacity);
                Assert.AreEqual(0, table.Indices.Count);

                table.Update(0);
                table.Update(3);
                table.Update(5);
                table.Update(10);
                table.Update(12345);
                table.Update(2);

                Assert.AreEqual(0, table.Indices.Count);

                table.Capacity = 3;
                Assert.AreEqual(3, table.Capacity);

                table.Update(0);
                table.Update(3);
                table.Update(5);
                table.Update(10);
                table.Update(12345);
                table.Update(2);
                Assert.AreEqual(3, table.Capacity);
                Assert.IsTrue(table.Indices.Contains(2));
                Assert.IsTrue(table.Indices.Contains(5));
                Assert.IsTrue(table.Indices.Contains(10));

                table.Capacity = 5;
                table.Update(0);
                table.Update(3);
                table.Update(5);
                table.Update(10);
                table.Update(12345);
                table.Update(2);
                Assert.AreEqual(5, table.Capacity);
                Assert.IsTrue(table.Indices.Contains(2));
                Assert.IsTrue(table.Indices.Contains(3));
                Assert.IsTrue(table.Indices.Contains(5));
                Assert.IsTrue(table.Indices.Contains(10));
                Assert.IsTrue(table.Indices.Contains(12345));

                // Capacity が縮小する場合は、最初の Update 時に調整
                table.Capacity = 3;
                Assert.AreEqual(5, table.Indices.Count);
                table.Update(5);
                Assert.AreEqual(3, table.Indices.Count);
                Assert.IsTrue(table.Indices.Contains(2));
                Assert.IsTrue(table.Indices.Contains(3));
                Assert.IsTrue(table.Indices.Contains(5));
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestItemInserted
        /// 
        /// <summary>
        /// 管理する対象となる IListProxy オブジェクトに新しい項目が挿入
        /// された時のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestItemInserted()
        {
            try
            {
                var table = new CubePdf.Wpf.IndexTable();
                Assert.IsNull(table.Items);
                table.Capacity = 5;
                Assert.AreEqual(5, table.Capacity);

                table.Update(1);
                table.Update(3);
                table.Update(5);
                table.Update(7);
                table.Update(9);
                Assert.AreEqual(5, table.Indices.Count);

                table.ItemInserted(5, 4);
                Assert.IsTrue(table.Indices.Contains(1));
                Assert.IsTrue(table.Indices.Contains(3));
                Assert.IsTrue(table.Indices.Contains(9));
                Assert.IsTrue(table.Indices.Contains(11));
                Assert.IsTrue(table.Indices.Contains(13));
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestItemRemoved
        /// 
        /// <summary>
        /// 管理する対象となる IListProxy オブジェクトから項目が削除された
        /// 時のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestItemRemoved()
        {
            try
            {
                var table = new CubePdf.Wpf.IndexTable();
                Assert.IsNull(table.Items);
                table.Capacity = 5;
                Assert.AreEqual(5, table.Capacity);

                table.Update(1);
                table.Update(3);
                table.Update(5);
                table.Update(7);
                table.Update(9);
                Assert.AreEqual(5, table.Indices.Count);

                table.ItemRemoved(3, 3);
                Assert.AreEqual(3, table.Indices.Count);
                Assert.IsTrue(table.Indices.Contains(1));
                Assert.IsTrue(table.Indices.Contains(4));
                Assert.IsTrue(table.Indices.Contains(6));
            }
            catch (Exception err) { Assert.Fail(err.ToString()); }
        }

        #endregion
    }
}
