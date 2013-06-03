/* ------------------------------------------------------------------------- */
///
/// Settings/UpdateCheckerTester.cs
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
    /// UpdateCheckerTester
    /// 
    /// <summary>
    /// UpdateChecker クラスのテストをするためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class UpdateCheckerTester
    {
        /* ----------------------------------------------------------------- */
        ///
        /// TestNotify
        /// 
        /// <summary>
        /// サーバへインストールが完了した事を通知するためのテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestNotify()
        {
            try
            {                
                var checker = new CubePdf.Settings.UpdateChecker();
                checker.ProductName = "cubepdf";
                checker.Version = "1.0.0RC4";
                checker.Notify();
            }
            catch (Exception err)
            {
                Assert.Fail(err.ToString());
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestGetResponse
        /// 
        /// <summary>
        /// サーバからバージョンアップに関するレスポンスを取得するための
        /// テストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestGetResponse()
        {
            var date = DateTime.Now;

            var checker = new CubePdf.Settings.UpdateChecker();
            checker.ProductName = "cubepdf";
            checker.Version = "0.9.1β";
            checker.CheckInterval = 0;
            var response = checker.GetResponse();
            Assert.NotNull(response);
            Assert.IsTrue(date <= checker.LastCheckUpdate);

            checker.CheckInterval = 1;
            response = checker.GetResponse();
            Assert.IsNull(response);
        }
    }
}
