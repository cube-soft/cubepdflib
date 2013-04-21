/* ------------------------------------------------------------------------- */
///
/// Page.cs
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
    /// Page
    /// 
    /// <summary>
    /// PDF のページ毎の情報を保持するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Page : IPage
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Page (constructor)
        ///
        /// <summary>
        /// 規定の値で Page クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Page() { }

        /* ----------------------------------------------------------------- */
        ///
        /// Page (constructor)
        /// 
        /// <summary>
        /// ファイルパス、ページ番号を指定して Page クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Page(string path, int pagenum)
        {
            _path = path;
            _pagenum = pagenum;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Page (constructor)
        /// 
        /// <summary>
        /// コピー元となる IReadOnlyPage オブジェクトを指定して Page クラス
        /// を初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Page(IPage cp)
        {
            _path = cp.FilePath;
            _pagenum = cp.PageNumber;
            _size = cp.OriginalSize;
            _rotation = cp.Rotation;
            _power = cp.Power;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// 該当ページのファイルパスを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath
        {
            get { return _path; }
            set { _path = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// PageNumber
        /// 
        /// <summary>
        /// 該当ページのページ番号を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int PageNumber
        {
            get { return _pagenum; }
            set { _pagenum = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OriginalSize
        /// 
        /// <summary>
        /// 該当ページのサイズ（幅、および高さ）を取得、または設定します。
        /// 
        /// NOTE: OriginalSize プロパティは、そのページの元々の幅と高さを
        /// 表します。PDF 閲覧ソフト等で実際に表示されるイメージのサイズは
        /// 回転角や表示倍率等の関係で、OriginalSize プロパティで取得できる
        /// 値と異なる事があります。該当ページが PDF ビューワ等で表示される
        /// 際の幅や高さは ViewSize プロパティで取得して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Size OriginalSize
        {
            get { return _size; }
            set { _size = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Rotation
        /// 
        /// <summary>
        /// 該当ページを表示する際の回転角を取得、または設定します (degree)。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Power
        /// 
        /// <summary>
        /// 該当ページを表示する際の倍率を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ViewSize
        /// 
        /// <summary>
        /// 該当ページを表示する際のサイズ（幅、および高さ）を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Size ViewSize
        {
            get
            {
                var degree = _rotation;
                if (degree < 0) degree += 360;
                else if (degree >= 360) degree -= 360;

                var radian = Math.PI * degree / 180.0;
                var sin = Math.Abs(Math.Sin(radian));
                var cos = Math.Abs(Math.Cos(radian));
                var width = _size.Width * cos + _size.Height * sin;
                var height = _size.Width * sin + _size.Height * cos;
                return new Size((int)(width * _power), (int)(height * _power));
            }
        }

        #endregion

        #region Public Methods

        /* --------------------------------------------------------------------- */
        /// ToString
        /* --------------------------------------------------------------------- */
        public override string ToString()
        {
            if (_path.Length == 0) return "(empty instance of the CubePdf.Data.Page)";
            return String.Format("{0}({1}): Width => {2}, Height => {3}, Rotation => {4}",
                _path, _pagenum, _size.Width, _size.Height, _rotation);
        }

        #endregion

        #region Implementations for IEquatable<IReadOnlyPage>

        /* ----------------------------------------------------------------- */
        /// Equals
        /* ----------------------------------------------------------------- */
        public bool Equals(IPage other)
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

            var other = obj as IPage;
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

        #region Variables
        private string _path = string.Empty;
        private int _pagenum = 1;
        private Size _size = new Size();
        private int _rotation = 0;
        private double _power = 1.0;
        #endregion
    }
}
