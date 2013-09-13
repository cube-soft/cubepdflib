/* ------------------------------------------------------------------------- */
///
/// Settings/NodeTester.cs
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

namespace CubePdfTests.Settings
{    /* --------------------------------------------------------------------- */
    ///
    /// NodeTester
    /// 
    /// <summary>
    /// Node クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class NodeTester
    {
        #region Test methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestSetValue
        /// 
        /// <summary>
        /// 各種の型の値を設定するテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestSetValue()
        {
            var node = new CubePdf.Settings.Node("Dummy");

            // string
            string str = "Hello, world!";
            node.SetValue(str);
            Assert.AreEqual(CubePdf.Settings.ValueKind.String, node.ValueKind);
            Assert.AreEqual(str, (string)node.Value);

            // int
            int number = 10;
            node.SetValue(number);
            Assert.AreEqual(CubePdf.Settings.ValueKind.Number, node.ValueKind);
            Assert.AreEqual(number, (int)node.Value);

            // bool
            bool enable = true;
            node.SetValue(enable);
            Assert.AreEqual(CubePdf.Settings.ValueKind.Bool, node.ValueKind);
            Assert.AreEqual(enable, (bool)node.Value);

            // List<Node>
            var list = new List<CubePdf.Settings.Node>();
            list.Add(new CubePdf.Settings.Node("Child0"));
            list.Add(new CubePdf.Settings.Node("Child1"));
            list.Add(new CubePdf.Settings.Node("Child2"));
            node.SetValue(list);
            Assert.AreEqual(CubePdf.Settings.ValueKind.NodeSet, node.ValueKind);

            // NodeSet
            var nodeset = new CubePdf.Settings.NodeSet();
            list.Add(new CubePdf.Settings.Node("Child0"));
            list.Add(new CubePdf.Settings.Node("Child1"));
            list.Add(new CubePdf.Settings.Node("Child2"));
            node.SetValue(nodeset);
            Assert.AreEqual(CubePdf.Settings.ValueKind.NodeSet, node.ValueKind);
        }

        #endregion
    }
}
