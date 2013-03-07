/* ------------------------------------------------------------------------- */
///
/// Metadata.cs
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

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// Metadata
    /// 
    /// <summary>
    /// PDF ファイルに保持可能なメタデータを表すクラスです。
    /// Metadata クラスでは、標準的なメタデータのみを扱います。
    /// eXtensible Metadata Platform (XMP) 領域に保持されているメタデータ
    /// は扱いません。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Metadata : IReadOnlyMetadata
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata (constructor)
        /// 
        /// <summary>
        /// 規定の値で Metadata クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Metadata() { }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Author
        ///
        /// <summary>
        /// 著者を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Title
        ///
        /// <summary>
        /// タイトルを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Subtitle
        ///
        /// <summary>
        /// サブタイトルを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Subtitle
        {
            get { return _subtitle; }
            set { _subtitle = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Keywords
        /// 
        /// <summary>
        /// キーワードを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Creator
        ///
        /// <summary>
        /// PDF の作成・編集を行うアプリケーション名を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Producer
        ///
        /// <summary>
        /// PDF の作成・編集を行う際に使用したプリンタドライバ、ライブラリ等
        /// の名前を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Producer
        {
            get { return _producer; }
            set { _producer = value; }
        }

        #endregion

        #region Public Methods

        /* ----------------------------------------------------------------- */
        /// ToString
        /* ----------------------------------------------------------------- */
        public override string ToString()
        {
            return String.Format("{0}({1})", _title, _author);
        }

        #endregion

        #region Variables
        private string _author = string.Empty;
        private string _title = string.Empty;
        private string _subtitle = string.Empty;
        private string _keywords = string.Empty;
        private string _creator = "CubePDF";
        private string _producer = string.Empty;
        #endregion
    }
}
