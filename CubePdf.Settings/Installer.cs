using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// Installer
    ///
    /// <summary>
    /// link.cube-soft.jp/install.phpに問い合わせる為のクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Installer
    {
        #region Initialization and Termination

        public Installer() { }

        #endregion

        #region Properties

        public string Medium
        {
            get { return _medium; }
            set { _medium = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        #endregion

        #region Public Methods

        public void Notify()
        {
            var request = GetRequest();
            using (var response = request.GetResponse())
            {
                using (var sr = new System.IO.StreamReader(response.GetResponseStream(), Encoding.GetEncoding("Shift_JIS")))
                {
                    // とりあえずレスポンスを吐き出すだけ
                    Trace.WriteLine(sr.ReadToEnd());
                }
            }
        }

        #endregion

        #region Private Methods

        private System.Net.WebRequest GetRequest()
        {
            var url = string.Format("http://link.cube-soft.jp/install.php?utm_medium={0}&utm_content={1}",
                                    _medium, _content.Replace("\"", ""));
            var dest = System.Net.WebRequest.Create(url);
            dest.Proxy = null;
            Trace.WriteLine(url);
            return dest;
        }

        #endregion

        #region Variables

        private string _medium = string.Empty;
        private string _content = string.Empty;

        #endregion

    }
}
