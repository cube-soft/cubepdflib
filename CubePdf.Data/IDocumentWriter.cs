/* ------------------------------------------------------------------------- */
///
/// IDocumentWriter.cs
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
using System.Collections.Generic;

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// IDocumentReader
    /// 
    /// <summary>
    /// PDF ファイルを作成、保存するためのインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IDocumentWriter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        /// 
        /// <summary>
        /// 初期状態にリセットします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Reset();

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 指定されたパスに PDF ファイルを保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        void Save(string path);

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルのメタデータを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Metadata Metadata { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// 暗号化に関する情報をを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Encryption Encryption { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページ情報を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ICollection<PageBase> Pages { get; }
    }
}
