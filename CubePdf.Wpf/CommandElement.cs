/* ------------------------------------------------------------------------- */
///
/// CommandElement.cs
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
using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// CommandElement
    /// 
    /// <summary>
    /// あるコマンドを構成するデータを格納するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class CommandElement
    {
        /* ----------------------------------------------------------------- */
        ///
        /// CommandElement (constructor)
        ///
        /// <summary>
        /// コマンドを引数に取り、オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CommandElement(ICommand command)
        {
            _command = command;
        }

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Command
        /// 
        /// <summary>
        /// コマンドを表す ICommand オブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ICommand Command
        {
            get { return _command; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Parameters
        /// 
        /// <summary>
        /// コマンドに関わるパラメータリストを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IList Parameters
        {
            get { return _params; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Text
        /// 
        /// <summary>
        /// コマンドを説明するテキストを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        #region Variables
        private ICommand _command = null;
        private IList _params = new ArrayList();
        private string _text = string.Empty;
        #endregion
    }
}
