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
            switch (page.Type)
            {
                case PageType.Pdf:
                    var pdf = page as Page;
                    return (pdf != null) ? Get(pdf.FilePath, pdf.Password) : null;
                case PageType.Image:
                    return null;
                default:
                    break;
            }
            return null;
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
        public static BitmapEngine Get(string path, string password)
        {
            if (_dic.ContainsKey(path)) return _dic[path];

            try
            {
                var engine = new CubePdf.Drawing.BitmapEngine();
                engine.Open(path, password);
                _dic.Add(path, engine);
                return engine;
            }
            catch (Exception /* err */)
            {
                Decrypt(path, password);
                return Get(path, password);
            }
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
            engine.Dispose();
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

        #region Private methods

        /* ----------------------------------------------------------------- */
        ///
        /// Decrypt
        /// 
        /// <summary>
        /// PDF ファイルを復号します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void Decrypt(string path, string password)
        {
            using (var reader = new CubePdf.Editing.DocumentReader(path, password))
            {
                if (reader.EncryptionStatus == Data.EncryptionStatus.RestrictedAccess)
                {
                    throw new EncryptionException(string.Format("{0}: cannot decrypt file.", reader.FilePath));
                }

                var tmp = System.IO.Path.GetTempFileName();
                System.IO.File.Delete(tmp);

                var binder = new CubePdf.Editing.PageBinder();
                foreach (var page in reader.Pages) binder.Pages.Add(page);
                binder.Metadata = reader.Metadata;
                binder.Save(tmp);

                var engine = new CubePdf.Drawing.BitmapEngine();
                engine.Open(path, password);
                _dic.Add(path, engine);
            }
        }

        #endregion

        #region Fields
        private static Dictionary<string, BitmapEngine> _dic = new Dictionary<string, BitmapEngine>();
        #endregion
    }
}
