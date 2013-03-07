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
    public class Metadata
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
        /// FilePath
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
            set { _path = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Author
        /* ----------------------------------------------------------------- */
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Title
        /* ----------------------------------------------------------------- */
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Subject
        /* ----------------------------------------------------------------- */
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Creator
        /* ----------------------------------------------------------------- */
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        /* ----------------------------------------------------------------- */
        /// Producer
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
        private string _path = string.Empty;
        private string _author = string.Empty;
        private string _title = string.Empty;
        private string _subject = string.Empty;
        private string _keywords = string.Empty;
        private string _creator = string.Empty;
        private string _producer = string.Empty;
        #endregion
    }
}
