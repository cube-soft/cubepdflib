/* ------------------------------------------------------------------------- */
///
/// InstallChecker.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// InstallChecker
    ///
    /// <summary>
    /// インストールの確認を行うためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class InstallChecker
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// InstallChecker (constructor)
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public InstallChecker() { }

        /* ----------------------------------------------------------------- */
        ///
        /// InstallChecker (constructor)
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public InstallChecker(string[] args) { }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Medium
        /// 
        /// <summary>
        /// インストール元ソフトウェアの種別を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Medium
        {
            get { return _medium; }
            set { _medium = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Content
        /// 
        /// <summary>
        /// インストール内容を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        #endregion

        #region Public Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Notify
        /// 
        /// <summary>
        /// サーバへリクエストを送ります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Notify()
        {
            var request = GetRequest();
            using (var response = request.GetResponse())
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
            {
                Debug.WriteLine(reader.ReadToEnd());
            }
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetRequest
        /// 
        /// <summary>
        /// サーバへのリクエストを生成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private System.Net.WebRequest GetRequest()
        {
            var url = string.Format("{0}?utm_medium={1}&utm_content={2}",
                _EndPoint, _medium, _content.Replace("\"", ""));
            var dest = System.Net.WebRequest.Create(url);
            dest.Proxy = null;
            Debug.WriteLine(url);
            return dest;
        }

        #endregion

        #region Variables
        private string _medium = string.Empty;
        private string _content = string.Empty;
        #endregion

        #region Static variables
        private static readonly string _EndPoint = "http://link.cube-soft.jp/install.php";
        #endregion
    }
}
