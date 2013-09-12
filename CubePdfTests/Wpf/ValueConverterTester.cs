/* ------------------------------------------------------------------------- */
///
/// Wpf/ValueConverterTester.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using NUnit.Framework;

namespace CubePdfTests.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ValueConverterTester
    /// 
    /// <summary>
    /// IValueConverter インターフェースを実装した各種コンバータクラスの
    /// テストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class ValueConverterTester
    {
        #region Test methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestEnumToInt
        /// 
        /// <summary>
        /// EnumToIntConverter クラスのテストです。
        /// 
        /// TODO: Enum 型に定義されていない int 値が指定された場合の挙動が
        /// 不明なので要調査。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestEnumToInt()
        {
            var converter = new CubePdf.Wpf.EnumToIntConverter<CubePdf.Data.EncryptionMethod>();
            var result = ExecConvert(converter, CubePdf.Data.EncryptionMethod.Aes256, typeof(int));
            Assert.AreEqual(3, (int)result);

            result = ExecConvert(converter, CubePdf.Data.EncryptionMethod.Unknown, typeof(int));
            Assert.AreEqual(-1, (int)result);

            result = ExecConvert(converter, null, typeof(int));
            Assert.AreEqual(0, (int)result);

            result = ExecConvertBack(converter, 3, typeof(CubePdf.Data.EncryptionMethod));
            Assert.AreEqual(CubePdf.Data.EncryptionMethod.Aes256, (CubePdf.Data.EncryptionMethod)result);

            result = ExecConvertBack(converter, -1, typeof(CubePdf.Data.EncryptionMethod));
            Assert.AreEqual(CubePdf.Data.EncryptionMethod.Unknown, (CubePdf.Data.EncryptionMethod)result);

            //result = ExecConvertBack(converter, 65536, typeof(CubePdf.Data.EncryptionMethod));
            //Assert.AreEqual((CubePdf.Data.EncryptionMethod)0, (CubePdf.Data.EncryptionMethod)result);

            result = ExecConvertBack(converter, null, typeof(CubePdf.Data.EncryptionMethod));
            Assert.AreEqual((CubePdf.Data.EncryptionMethod)0, (CubePdf.Data.EncryptionMethod)result);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestStringToInt
        /// 
        /// <summary>
        /// StringToIntConverter クラスのテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestStringToInt()
        {
            var converter = new CubePdf.Wpf.StringToIntConverter();
            var result = ExecConvert(converter, "32538", typeof(int));
            Assert.AreEqual(32538, (int)result);

            result = ExecConvert(converter, "-2", typeof(int));
            Assert.AreEqual(-2, (int)result);

            result = ExecConvert(converter, "abcd", typeof(int));
            Assert.AreEqual(default(int), (int)result);

            result = ExecConvert(converter, null, typeof(int));
            Assert.AreEqual(default(int), (int)result);

            result = ExecConvertBack(converter, 65553, typeof(string));
            Assert.AreEqual("65553", result as string);

            result = ExecConvertBack(converter, -32, typeof(string));
            Assert.AreEqual("-32", result as string);

            result = ExecConvertBack(converter, null, typeof(string));
            Assert.IsTrue(string.IsNullOrEmpty(result as string));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestControlToTag
        /// 
        /// <summary>
        /// ControlToTagConverter クラスのテストです。
        /// 
        /// NOTE: STA で実行される事を要求されるため、テストを保留。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestControlToTag()
        {
            var converter = new CubePdf.Wpf.ControlToTagConverter();
            //var control = new Control();
            //control.Tag = "Hello, world!";
            //var result = ExecConvert(converter, control, typeof(string));
            //Assert.AreEqual("Hello, world!", result as string);
            Assert.NotNull(converter);

        }

        /* ----------------------------------------------------------------- */
        ///
        /// TestImage
        /// 
        /// <summary>
        /// ImageConverter クラスのテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestImage()
        {
            var converter = new CubePdf.Wpf.ImageConverter();
            var source = new System.Drawing.Bitmap(16, 16);
            var result = ExecConvert(converter, source, typeof(ImageSource));
            Assert.NotNull(result);
            Assert.NotNull(result as ImageSource);
            var image = result as ImageSource;

            result = ExecConvert(converter, null, typeof(ImageSource));
            Assert.IsNull(result);

            result = ExecConvertBack(converter, image, typeof(System.Drawing.Image));
            Assert.IsNull(result);

            result = ExecConvertBack(converter, null, typeof(System.Drawing.Image));
            Assert.IsNull(result);
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// ExecConvert
        /// 
        /// <summary>
        /// 必要な情報を補って IValueConverter.Convert メソッドを実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ExecConvert(IValueConverter converter, object value, Type target)
        {
            var culture = new CultureInfo("ja-JP");
            return converter.Convert(value, target, null, culture);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ExecConvertBack
        /// 
        /// <summary>
        /// 必要な情報を補って IValueConverter.ConvertBack メソッドを実行
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ExecConvertBack(IValueConverter converter, object value, Type target)
        {
            var culture = new CultureInfo("ja-JP");
            return converter.ConvertBack(value, target, null, culture);
        }

        #endregion
    }
}
