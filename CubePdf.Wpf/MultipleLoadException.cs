/* ------------------------------------------------------------------------- */
///
/// MultipleLoadException.cs
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

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// MultipleLoadException
    /// 
    /// <summary>
    /// 同名パスのファイルが既にロードされている時に送出される例外クラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class MultipleLoadException : Exception
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// MultipleLoadException (constructor)
        ///
        /// <summary>
        /// メッセージ文字列がシステムによって提供されたメッセージに設定
        /// された MultipleLoadException クラスの新しいインスタンスを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public MultipleLoadException() { }

        /* ----------------------------------------------------------------- */
        ///
        /// MultipleLoadException (constructor)
        ///
        /// <summary>
        /// メッセージ文字列を引数 message に指定された MultipleLoadException
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public MultipleLoadException(string message) : base(message) { }

        /* ----------------------------------------------------------------- */
        ///
        /// MultipleLoadException (constructor)
        ///
        /// <summary>
        /// 指定したエラーメッセージと、この例外の原因である内部例外への
        /// 参照を使用して、MultipleLoadException クラスの新しいインスタンス
        /// を初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public MultipleLoadException(string message, Exception inner) : base(message) { }

        /* ----------------------------------------------------------------- */
        ///
        /// MultipleLoadException (constructor)
        ///
        /// <summary>
        /// 指定したエラーメッセージと原因となったファイルへのパスを使用して
        /// MultipleLoadException クラスの新しいインスタンスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public MultipleLoadException(string message, string path) : base(message)
        {
            _path = path;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// 原因となったファイルへのパスを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
            set { _path = value; }
        }

        #endregion

        #region Variables
        private string _path = string.Empty;
        #endregion
    }
}
