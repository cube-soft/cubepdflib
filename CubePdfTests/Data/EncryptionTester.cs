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
using NUnit.Framework;

namespace CubePdfTests.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// EncryptionTester
    /// 
    /// <summary>
    /// CubePdf.Data.Encryption クラスのテストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class EncryptionTester
    {
        /* ----------------------------------------------------------------- */
        ///
        /// TestConstruct
        /// 
        /// <summary>
        /// Encryption クラスの初期化のテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestConstruct()
        {
            var encrypt = new CubePdf.Data.Encryption();
            Assert.IsFalse(encrypt.IsEnabled);
            Assert.IsFalse(encrypt.IsUserPasswordEnabled);
            Assert.AreEqual(0, encrypt.OwnerPassword.Length);
            Assert.AreEqual(0, encrypt.UserPassword.Length);
            Assert.AreEqual(CubePdf.Data.EncryptionMethod.Unknown, encrypt.Method);
            Assert.IsFalse(encrypt.Permission.Printing);
            Assert.IsFalse(encrypt.Permission.Assembly);
            Assert.IsFalse(encrypt.Permission.ModifyContents);
            Assert.IsFalse(encrypt.Permission.CopyContents);
            Assert.IsFalse(encrypt.Permission.Accessibility);
            Assert.IsFalse(encrypt.Permission.ExtractPage);
            Assert.IsFalse(encrypt.Permission.ModifyAnnotations);
            Assert.IsFalse(encrypt.Permission.InputFormFields);
            Assert.IsFalse(encrypt.Permission.Signature);
            Assert.IsFalse(encrypt.Permission.TemplatePage);
        }
    }
}
