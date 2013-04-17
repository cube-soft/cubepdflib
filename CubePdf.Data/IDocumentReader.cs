/* ------------------------------------------------------------------------- */
///
/// IDocumentReader.cs
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

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// IDocumentReader
    /// 
    /// <summary>
    /// PDF ファイルの読み込むためのインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IDocumentReader : IDisposable
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// PDF ファイルを開きます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Open(string path, string password = "");

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在、開いている PDF ファイルを閉じます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Close();

        /* ----------------------------------------------------------------- */
        ///
        /// GetPage
        /// 
        /// <summary>
        /// 指定されたページ番号に対応するページ情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        CubePdf.Data.IPage GetPage(int pagenum);

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// PDF ファイルのパスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string FilePath { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Password
        /// 
        /// <summary>
        /// PDF ファイルを開く際に指定されたパスワードを取得します。
        /// 指定されたパスワードがオーナパスワードなのかユーザパスワード
        /// なのかの判断については、EncryptionStatus の情報から判断します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Password { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// PageCount
        /// 
        /// <summary>
        /// PDF ファイルのページ数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int PageCount { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルのメタデータを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        IMetadata Metadata { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionStatus
        /// 
        /// <summary>
        /// 暗号化されている PDF ファイルへのアクセス（許可）状態を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        EncryptionStatus EncryptionStatus { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionMethod
        /// 
        /// <summary>
        /// 暗号化方式を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        EncryptionMethod EncryptionMethod { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Permission
        /// 
        /// <summary>
        /// PDF ファイルに設定されている各種操作の権限に関する情報を取得
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        IPermission Permission { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページ情報へアクセスするための反復子を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        IEnumerable<IPage> Pages { get; }
    }
}
