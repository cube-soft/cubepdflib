/* ------------------------------------------------------------------------- */
///
/// IPage.cs
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
using System.Drawing;

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// IPage
    /// 
    /// <summary>
    /// PDF のページを表すインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IPage : IEquatable<IPage>
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Type
        /// 
        /// <summary>
        /// オブジェクトの種類を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        PageType Type { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Path
        /// 
        /// <summary>
        /// オブジェクト元となるファイルのパスを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string FilePath { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Size
        /// 
        /// <summary>
        /// オブジェクトのオリジナルサイズを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Size OriginalSize { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Rotation
        /// 
        /// <summary>
        /// オブジェクトを表示する際の回転角を取得または設定します。
        /// </summary>
        /// 
        /// <remarks>
        /// 値は度単位 (degree) で設定して下さい。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        int Rotation { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Power
        /// 
        /// <summary>
        /// 表示倍率を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        double Power { get; set; }
    }
}
