/* ------------------------------------------------------------------------- */
///
/// IMetadata.cs
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

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// IMetadata
    /// 
    /// <summary>
    /// PDF ファイルのメタデータを提供するためのインターフェースです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IMetadata
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Version
        /// 
        /// <summary>
        /// PDF ファイルのバージョンを取得します。現時点で有効な
        /// PDF バージョンは 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7,
        /// 1.7 Adobe Extension Level 3, 1.7 Adobe Extension Level 5 の
        /// 10 種類です。Adobe Extension Level の値は Build プロパティで
        /// 保持する事とします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        Version Version { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Author
        ///
        /// <summary>
        /// 著者を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Author { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Title
        ///
        /// <summary>
        /// タイトルを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Title { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Subtitle
        ///
        /// <summary>
        /// サブタイトルを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Subtitle { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Keywords
        /// 
        /// <summary>
        /// キーワードを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Keywords { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Creator
        ///
        /// <summary>
        /// PDF の作成・編集を行うアプリケーション名を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Creator { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Producer
        ///
        /// <summary>
        /// PDF の作成・編集を行う際に使用したプリンタドライバ、ライブラリ等
        /// の名前を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        string Producer { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// ViewerPreferences
        ///
        /// <summary>
        /// PDF のページレイアウトおよび開き方を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int ViewerPreferences { get; }
    }
}
