/* ------------------------------------------------------------------------- */
///
/// UpdateChecker.cs
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
using System.Diagnostics;
using System.Collections.Generic;

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// UpdateChecker
    ///
    /// <summary>
    /// www.cube-soft.jp に問い合わせて、バージョンアップが行われているか
    /// どうかを確認するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class UpdateChecker
    {
        #region Initializing and Terminating

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateChecker (constructor)
        /// 
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public UpdateChecker() { }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateChecker (constructor)
        /// 
        /// <summary>
        /// 引数に指定されたサブキー名を利用して、オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public UpdateChecker(string subkey)
        {
            InitializeVariables(subkey);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// ProductName
        /// 
        /// <summary>
        /// 製品名を取得、または設定します。このプロパティに設定する値は、
        /// URL の一部として使用されます。したがって、例えば CubePDF の場合、
        /// 設定すべき値は "cubepdf" となります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string ProductName
        {
            get { return _product; }
            set { _product = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Version
        /// 
        /// <summary>
        /// 現在インストールされているバージョンを取得、または設定します。
        /// コンストラクタにレジストリのサブキーを設定した場合は、
        /// オブジェクトの初期化時にバージョン情報が取得されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CheckInterval
        /// 
        /// <summary>
        /// バージョンチェックを行うインターバルを取得、または設定します。
        /// 前回に GetResponse メソッドを実行してから CheckInterval 日
        /// 経過していない場合、GetResponse メソッドの実行はスキップされます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int CheckInterval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// LastCheckUpdate
        /// 
        /// <summary>
        /// 最後にアップデートの確認を行った日時を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime LastCheckUpdate
        {
            get { return _last; }
        }

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Notify
        /// 
        /// <summary>
        /// サーバへリクエストを送ります。サーバからのレスポンスは無視され
        /// ます。このメソッドは、主にインストール直後に使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Notify()
        {
            var request = GetRequest("flag=install");
            using (var response = request.GetResponse())
            {
                // do nothing
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetResponse
        /// 
        /// <summary>
        /// サーバへアップデートチェックのためのリクエストを送り、現在の
        /// バージョンに対応するレスポンスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IDictionary<string, string> GetResponse()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() ||
                DateTime.Now <= _last.AddDays(_interval)) return null;

            try
            {
                var request = GetRequest("");
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line.Length > 0 && line[0] == '[' && line[line.Length - 1] == ']')
                        {
                            var compare = line.Substring(1, line.Length - 2);
                            if (compare == _version) return ParseResponse(reader);
                        }
                        line = reader.ReadLine();
                    }
                }
                return null;
            }
            finally
            {
                _last = DateTime.Now;
                if (!string.IsNullOrEmpty(_subkey))
                {
                    var registry = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(_subkey);
                    registry.SetValue(REG_LASTCHECK, _last.ToString());
                }
            }
        }

        #endregion

        #region Private methods

        /* ----------------------------------------------------------------- */
        ///
        /// InitializeVariables
        /// 
        /// <summary>
        /// 引数に指定されたレジストリのサブキーを利用して、メンバ変数を
        /// 初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InitializeVariables(string subkey)
        {
            _subkey = subkey;

            try
            {
                var registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subkey, false);
                _version = (string)registry.GetValue(REG_VERSION, string.Empty);
                registry.Close();
            }
            catch (Exception err) { Trace.TraceError(err.ToString()); }

            try {
                var registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subkey, false);
                var date = (string)registry.GetValue(REG_LASTCHECK, string.Empty);
                if (!string.IsNullOrEmpty(date)) _last = DateTime.Parse(date);
            }
            catch (Exception err) { Trace.TraceError(err.ToString()); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetRequest
        /// 
        /// <summary>
        /// サーバへのリクエストを生成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private System.Net.WebRequest GetRequest(string suffix)
        {
            var url = string.Format("http://{0}/{1}/update.php?ver={2}",
                Properties.Resources.DomainName, _product, System.Web.HttpUtility.UrlEncode(_version));
            if (!string.IsNullOrEmpty(suffix)) url += "&" + suffix;
            return System.Net.WebRequest.Create(url);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ParseResponse
        /// 
        /// <summary>
        /// サーバから取得したレスポンスを解析します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private IDictionary<string, string> ParseResponse(System.IO.StreamReader reader)
        {
            var dest = new Dictionary<string, string>();
            var line = reader.ReadLine();
            while (line != null)
            {
                if (line.Length > 0)
                {
                    if (line[0] == '[' && line[line.Length - 1] == ']') break;

                    var pos = line.IndexOf('=');
                    if (pos >= 0)
                    {
                        var key = line.Substring(0, pos);
                        var value = line.Substring(pos + 1, line.Length - (pos + 1));
                        if (dest.ContainsKey(key)) dest[key] = value;
                        else dest.Add(key, value);
                    }
                }
                line = reader.ReadLine();
            }
            return dest;
        }

        #endregion

        #region Variables
        private string _subkey = string.Empty;
        private string _product = string.Empty;
        private string _version = string.Empty;
        private int _interval = 0;
        private DateTime _last = new DateTime();
        #endregion

        #region Constant variables
        private static readonly string REG_VERSION   = "Version";
        private static readonly string REG_LASTCHECK = "LastCheckUpdate";
        #endregion
    }
}
