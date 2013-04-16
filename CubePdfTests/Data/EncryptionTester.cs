/* ------------------------------------------------------------------------- */
///
/// Data/EncryptionTester.cs
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

        /* ----------------------------------------------------------------- */
        ///
        /// TestReadOnlyCast
        /// 
        /// <summary>
        /// Encryption クラスの情報を読み取り専用にするテストを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestReadOnlyCast()
        {
            var crypt = new CubePdf.Data.Encryption();
            crypt.IsEnabled = true;
            crypt.IsUserPasswordEnabled = true;
            crypt.OwnerPassword = "owner";
            crypt.UserPassword = "user";
            crypt.Method = CubePdf.Data.EncryptionMethod.Aes256;
            crypt.Permission.Printing = true;
            crypt.Permission.Accessibility = true;
            Assert.IsTrue(crypt.IsEnabled);
            Assert.IsTrue(crypt.IsUserPasswordEnabled);
            Assert.AreEqual("owner", crypt.OwnerPassword);
            Assert.AreEqual("user", crypt.UserPassword);
            Assert.AreEqual(CubePdf.Data.EncryptionMethod.Aes256, crypt.Method);
            Assert.IsTrue(crypt.Permission.Printing);
            Assert.IsFalse(crypt.Permission.Assembly);
            Assert.IsFalse(crypt.Permission.ModifyContents);
            Assert.IsFalse(crypt.Permission.CopyContents);
            Assert.IsTrue(crypt.Permission.Accessibility);
            Assert.IsFalse(crypt.Permission.ExtractPage);
            Assert.IsFalse(crypt.Permission.ModifyAnnotations);
            Assert.IsFalse(crypt.Permission.InputFormFields);
            Assert.IsFalse(crypt.Permission.Signature);
            Assert.IsFalse(crypt.Permission.TemplatePage);

            var readable = new CubePdf.Data.Encryption(crypt) as CubePdf.Data.IEncryption;
            Assert.IsTrue(readable.IsEnabled);
            Assert.IsTrue(readable.IsUserPasswordEnabled);
            Assert.AreEqual("owner", readable.OwnerPassword);
            Assert.AreEqual("user", readable.UserPassword);
            Assert.AreEqual(CubePdf.Data.EncryptionMethod.Aes256, readable.Method);
            Assert.IsTrue(readable.Permission.Printing);
            Assert.IsFalse(readable.Permission.Assembly);
            Assert.IsFalse(readable.Permission.ModifyContents);
            Assert.IsFalse(readable.Permission.CopyContents);
            Assert.IsTrue(readable.Permission.Accessibility);
            Assert.IsFalse(readable.Permission.ExtractPage);
            Assert.IsFalse(readable.Permission.ModifyAnnotations);
            Assert.IsFalse(readable.Permission.InputFormFields);
            Assert.IsFalse(readable.Permission.Signature);
            Assert.IsFalse(readable.Permission.TemplatePage);
        }
    }
}
