/* ------------------------------------------------------------------------- */
///
/// CommandLine.cs
///
/// Copyright (c) 2009 CubeSoft, Inc. All rights reserved.
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

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    ///  CommandLine
    ///  
    ///  <summary>
    ///  コマンドラインの引数を解析するためのクラスです。
    ///  </summary>
    ///  
    /* --------------------------------------------------------------------- */
    public class CommandLine
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Constructor
        ///
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CommandLine() { }

        /* ----------------------------------------------------------------- */
        ///
        /// Constructor
        ///
        /// <summary>
        /// 引数に指定されたプログラム引数を用いて、オブジェクトを初期化
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CommandLine(string[] args)
            : this()
        {
            Add(args);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Constructor
        ///
        /// <summary>
        /// 引数に指定されたプログラム引数、およびオプション付き引数用の
        /// 接頭辞を用いて、オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CommandLine(string[] args, char prefix)
            : this()
        {
            OptionPrefix = prefix;
            Add(args);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Values
        /// 
        /// <summary>
        /// コマンドラインの値一覧を取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// コマンドラインの値とは、直前にオプション付き引数と伴わない全ての
        /// 引数の事を指します。オプション付き引数を伴う引数については、
        /// Options プロパティから取得して下さい。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public IList<string> Values
        {
            get { return _values; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Options
        ///
        /// <summary>
        /// コマンドラインのオプション一覧を取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// コマンドラインのオプションとは、「/foo bar」のように指定される
        /// 引数の事を指します。尚、コマンドラインのオプションを表す
        /// 接頭辞は OptionPrefix プロパティで変更する事ができます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public IDictionary<string, string> Options
        {
            get { return this._options; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OptionPrefix
        /// 
        /// <summary>
        /// コマンドラインオプションを表す接頭辞を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public char OptionPrefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        /// 
        /// <summary>
        /// 現在保持している値、およびオプション引数を消去します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Clear()
        {
            _values.Clear();
            _options.Clear();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        ///
        /// <summary>
        /// 引数を解析し、解析された結果を追加します。
        /// </summary>
        /// 
        /// <remarks>
        /// 各オプション付き引数は、最大で 1 つの引数を持てる事とします。
        /// オプション付き引数に続いて 2 つ以上の引数が指定された場合、
        /// 2 つ目以降の引数は Values プロパティに格納されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Add(string[] args)
        {
            string key = string.Empty;
            for (int i = 0; i < args.Length; ++i)
            {
                if (string.IsNullOrEmpty(args[i])) continue;
                if (args[i][0] == OptionPrefix)
                {
                    if (!string.IsNullOrEmpty(key)) _options[key] =  string.Empty;
                    key = args[i].Substring(1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(key)) _options[key] =  args[i];
                    else _values.Add(args[i]);
                    key = string.Empty;
                }
            }
            if (!string.IsNullOrEmpty(key)) _options[key] = string.Empty;
        }

        #endregion

        #region Variables
        private IList<string> _values = new List<string>();
        private IDictionary<string, string> _options = new Dictionary<string, string>();
        private char _prefix = '/';
        #endregion
    }
}
