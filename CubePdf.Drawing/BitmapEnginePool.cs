/* ------------------------------------------------------------------------- */
///
/// BitmapEnginePool.cs
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
using CubePdf.Data;

namespace CubePdf.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// BitmapEnginePool
    /// 
    /// <summary>
    /// BitmapEngine のオブジェクト一覧を管理するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static class BitmapEnginePool
    {
        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Get
        /// 
        /// <summary>
        /// BitmapEngine オブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static BitmapEngine Get(PageBase page)
        {
            return (page.Type == PageType.Pdf) ? Get(page.FilePath) : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Get
        /// 
        /// <summary>
        /// BitmapEngine オブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static BitmapEngine Get(string path)
        {
            if (_dic.ContainsKey(path)) return _dic[path];

            try
            {
                var engine = new CubePdf.Drawing.BitmapEngine();
                engine.Open(path);
                _dic.Add(path, engine);
                return engine;
            }
            catch (Exception /* err */) { return null; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        /// 
        /// <summary>
        /// 指定されたパスに対応する BitmapEngine オブジェクトを開放して
        /// 一覧から削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Remove(string path)
        {
            if (!_dic.ContainsKey(path)) return;

            var engine = _dic[path];
            _dic.Remove(path);
            lock (engine) engine.Dispose();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        /// 
        /// <summary>
        /// 全ての BitmapEngine オブジェクトを開放します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Clear()
        {
            var gc = _dic;
            _dic = new Dictionary<string, BitmapEngine>();
            foreach (var item in gc) item.Value.Dispose();
            gc.Clear();
        }

        #endregion

        #region Fields
        private static Dictionary<string, BitmapEngine> _dic = new Dictionary<string, BitmapEngine>();
        #endregion
    }
}
