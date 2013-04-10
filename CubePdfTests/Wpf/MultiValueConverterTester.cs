/* ------------------------------------------------------------------------- */
///
/// Wpf/MultiValueConverterTester.cs
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
using System.Collections;
using System.Windows.Data;
using System.Globalization;
using NUnit.Framework;

namespace CubePdfTests.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// MultiValueConverterTester
    /// 
    /// <summary>
    /// IMultiValueConverter インターフェースを実装した各種コンバータクラスの
    /// テストを行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    public class MultiValueConverterTester
    {
        #region Test methods

        /* ----------------------------------------------------------------- */
        ///
        /// TestMultiAnd
        /// 
        /// <summary>
        /// MultiAndConverter クラスのテストです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void TestMultiAnd()
        {
            var converter = new CubePdf.Wpf.MultiAndConverter();

            var sources = new ArrayList();
            sources.Add(true);
            var result = ExecConvert(converter, sources.ToArray(), typeof(bool));
            Assert.IsTrue((bool)result);

            sources.Add(true);
            result = ExecConvert(converter, sources.ToArray(), typeof(bool));
            Assert.IsTrue((bool)result);

            sources.Add(false);
            result = ExecConvert(converter, sources.ToArray(), typeof(bool));
            Assert.IsFalse((bool)result);

            sources[sources.Count - 1] = null;
            result = ExecConvert(converter, sources.ToArray(), typeof(bool));
            Assert.IsTrue((bool)result);

            result = ExecConvert(converter, null, typeof(bool));
            Assert.IsFalse((bool)result);

            result = ExecConvertBack(converter, true, null);
            Assert.IsNull(result);

            result = ExecConvertBack(converter, false, null);
            Assert.IsNull(result);

            result = ExecConvertBack(converter, null, null);
            Assert.IsNull(result);
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// ExecConvert
        /// 
        /// <summary>
        /// 必要な情報を補って IMultiValueConverter.Convert メソッドを
        /// 実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ExecConvert(IMultiValueConverter converter, object[] values, Type target)
        {
            var culture = new CultureInfo("ja-JP");
            return converter.Convert(values, target, null, culture);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ExecConvertBack
        /// 
        /// <summary>
        /// 必要な情報を補って IMultiValueConverter.ConvertBack メソッドを
        /// 実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public object ExecConvertBack(IMultiValueConverter converter, object value, Type[] targets)
        {
            var culture = new CultureInfo("ja-JP");
            return converter.ConvertBack(value, targets, null, culture);
        }

        #endregion
    }
}
