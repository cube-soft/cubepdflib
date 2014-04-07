/* ------------------------------------------------------------------------- */
///
/// UserCancelledException.cs
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

namespace CubePdf.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// UserCancelledException
    /// 
    /// <summary>
    /// ユーザが処理をキャンセルした場合に例外を送出するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class UserCancelledException : Exception
    {
        /* ----------------------------------------------------------------- */
        ///
        /// UserCancelledException (constructor)
        /// 
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public UserCancelledException() : base() { }

        /* ----------------------------------------------------------------- */
        ///
        /// UserCancelledException (constructor)
        /// 
        /// <summary>
        /// 指定したエラーメッセージを使用して、オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public UserCancelledException(string message) : base(message) { }

        /* ----------------------------------------------------------------- */
        ///
        /// UserCancelledException (constructor)
        /// 
        /// <summary>
        /// 指定したエラーメッセージと、この例外の原因である内部例外への
        /// 参照を使用して、オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public UserCancelledException(string message, Exception innerException) : base(message, innerException) { }
    }
}
