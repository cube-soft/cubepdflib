/* ------------------------------------------------------------------------- */
///
/// Encryption.cs
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
    /// Encryption
    /// 
    /// <summary>
    /// PDF の暗号化に関するデータを表すクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Encryption : IReadOnlyEncryption
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption (constructor)
        /// 
        /// <summary>
        /// 規定の値で Encryption クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Encryption() { }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// OwnerPassword
        /// 
        /// <summary>
        /// 所有者パスワードを取得、または設定します。所有者パスワードとは
        /// PDF ファイルに設定されているマスターパスワードを表し、この
        /// パスワードによって再暗号化や各種権限の変更等すべての操作が可能
        /// となります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string OwnerPassword
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UserPassword
        /// 
        /// <summary>
        /// ユーザパスワードを取得、または設定します。ユーザパスワードとは
        /// PDF ファイルを開く際に必要となるパスワードを表します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string UserPassword
        {
            get { return _user; }
            set { _user = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Method
        /// 
        /// <summary>
        /// 適用する暗号化方式を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public EncryptionMethod Method
        {
            get { return _method; }
            set { _method = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Permission
        /// 
        /// <summary>
        /// 暗号化された PDF に設定されている各種権限の状態を取得、または
        /// 設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Permission Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }

        #endregion

        #region Variables
        private string _owner = string.Empty;
        private string _user = string.Empty;
        private EncryptionMethod _method = EncryptionMethod.Standard40;
        private Permission _permission = new Permission();
        #endregion
    }
}
