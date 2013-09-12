/* ------------------------------------------------------------------------- */
///
/// ImageEventArgs.cs
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

namespace CubePdf.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageEventArgs
    /// 
    /// <summary>
    /// ImageCreated イベントのデータを提供するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ImageEventArgs : EventArgs
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// ImageEventArgs (constructor)
        /// 
        /// <summary>
        /// 規定の値で ImageEventArgs クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageEventArgs() : base() { }

        /* ----------------------------------------------------------------- */
        ///
        /// ImageEventArgs (constructor)
        /// 
        /// <summary>
        /// 生成するイメージのページ情報を指定して ImageEventArgs クラスを
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageEventArgs(CubePdf.Data.Page page)
            : base()
        {
            _page = page;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Image
        /// 
        /// <summary>
        /// 生成されたイメージを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Image
        /// 
        /// <summary>
        /// 生成されたイメージのページ情報を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.IPage Page
        {
            get { return _page; }
            set { _page = value; }
        }

        #endregion

        #region Variables
        private Image _image = null;
        private CubePdf.Data.IPage _page = null;
        #endregion
    }

    /* --------------------------------------------------------------------- */
    ///
    /// ImageEventHandler
    /// 
    /// <summary>
    /// ImageEventArgus が必要なイベントのためのデリゲート型です。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public delegate void ImageEventHandler(object sender, ImageEventArgs e);
}
