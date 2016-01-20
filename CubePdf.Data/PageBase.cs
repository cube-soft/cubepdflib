/* ------------------------------------------------------------------------- */
///
/// PageBase.cs
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
    /// PageBase
    /// 
    /// <summary>
    /// PDF のページを表す基底クラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [Serializable]
    public class PageBase : IEquatable<PageBase>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// PageBase
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private PageBase() { }

        /* ----------------------------------------------------------------- */
        ///
        /// PageBase
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected PageBase(PageType type)
        {
            Type = type;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Type
        /// 
        /// <summary>
        /// オブジェクトの種類を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PageType Type { get; } = PageType.Unknown;

        /* ----------------------------------------------------------------- */
        ///
        /// Path
        /// 
        /// <summary>
        /// オブジェクト元となるファイルのパスを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath { get; set; } = string.Empty;

        /* ----------------------------------------------------------------- */
        ///
        /// Size
        /// 
        /// <summary>
        /// オブジェクトのオリジナルサイズを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Size OriginalSize { get; set; } = Size.Empty;

        /* ----------------------------------------------------------------- */
        ///
        /// PageNumber
        /// 
        /// <summary>
        /// 該当ページのページ番号を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int PageNumber { get; set; } = 0;

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
        public int Rotation { get; set; } = 0;

        /* ----------------------------------------------------------------- */
        ///
        /// Power
        /// 
        /// <summary>
        /// 表示倍率を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public double Power { get; set; } = 1.0;

        #endregion

        #region Implementations for IEquatable

        /* ----------------------------------------------------------------- */
        /// Equals
        /* ----------------------------------------------------------------- */
        public bool Equals(PageBase other)
        {
            return FilePath == other.FilePath && PageNumber == other.PageNumber;
        }

        /* ----------------------------------------------------------------- */
        /// Equals
        /* ----------------------------------------------------------------- */
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (object.ReferenceEquals(this, obj)) return true;

            var other = obj as PageBase;
            if (other == null) return false;

            return this.Equals(other);
        }

        /* ----------------------------------------------------------------- */
        /// GetHashCode
        /* ----------------------------------------------------------------- */
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
