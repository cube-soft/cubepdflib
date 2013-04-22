/* ------------------------------------------------------------------------- */
///
/// IPage.cs
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
using System.Drawing;

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// IPage
    /// 
    /// <summary>
    /// ページに関する情報を提供するためのインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IPage : IEquatable<IPage>
    {
        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// 該当ページのファイルパスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string FilePath { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Password
        /// 
        /// <summary>
        /// 該当ページの PDF ファイルのパスワードを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Password { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// PageNumber
        /// 
        /// <summary>
        /// 該当ページのページ番号を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int PageNumber { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// OriginalSize
        /// 
        /// <summary>
        /// 該当ページのサイズ（幅、および高さ）を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Size OriginalSize { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// ViewSize
        /// 
        /// <summary>
        /// 該当ページを表示する際のサイズ（幅、および高さ）を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Size ViewSize { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Rotation
        /// 
        /// <summary>
        /// 該当ページを表示する際の回転角を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int Rotation { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Power
        /// 
        /// <summary>
        /// 該当ページを表示する際の倍率を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        double Power { get; }
    }
}
