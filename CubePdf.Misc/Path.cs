/* ------------------------------------------------------------------------- */
///
/// Path.cs
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
using System.Collections.Generic;
using System.Text;

namespace CubePdf.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// Path
    /// 
    /// <summary>
    /// Path に関連する処理を行うメソッド群を定義するためのクラスです。
    /// </summary>
    /// 
    /* --------------------------------------------------------------------- */
    public abstract class Path
    {
        /* ----------------------------------------------------------------- */
        ///
        /// IsValid
        /// 
        /// <summary>
        /// 引数に指定された文字列がパスとして使用できるものであるかどうか
        /// 判別します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static bool IsValid(string path)
        {
            char[] invalids = { '/', '*', '"', '<', '>', '|' };
            if (string.IsNullOrEmpty(path)) return false;

            var inactivated = false;
            for (int i = 0; i < path.Length; ++i)
            {
                switch (path[i])
                {
                    case ':':  // ドライブ指定 (C:\foo) かどうかを判定
                        int first = inactivated ? 5 : 1;
                        if (!(i == first && char.IsLetter(path[i - 1]) && i + 1 < path.Length && path[i + 1] == '\\')) return false;
                        break;
                    case '?':  // 拡張機能の不活性化指定 (\\?\) かどうかを判定
                        if (i + 1 < path.Length && i == 2 && path[i - 1] == '\\' && path[i - 2] == '\\' && path[i + 1] == '\\') inactivated = true;
                        else return false;
                        break;
                    default:
                        if (System.Array.IndexOf(invalids, path[i]) >= 0) return false;
                        break;
                }
            }
            return inactivated || (path[path.Length - 1] != '.' && path[path.Length - 1] != ' ');
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IsValid
        /// 
        /// <summary>
        /// 引数に指定された文字列がファイル名として使用できるものであるか
        /// どうか判別します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static bool IsValidFilename(string path)
        {
            char[] invalids = { '/', '*', '"', '<', '>', '|', '?', ':', '\\' };
            if (string.IsNullOrEmpty(path)) return false;
            if (path[path.Length - 1] == '.' || path[path.Length - 1] == ' ') return false;

            foreach (var c in path)
            {
                if (System.Array.IndexOf(invalids, c) >= 0) return false;
            }
            return true;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Normalize
        /// 
        /// <summary>
        /// 引数 path をパスとして使用可能な文字列に変換します。
        /// path に含まれるパスに使用できない文字は replaced に置換されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static string Normalize(string path, char replaced)
        {
            char[] invalids = { '/', '*', '"', '<', '>', '|' };
            char[] all = { '/', '*', '"', '<', '>', '|', ':', '?', '\\' };
            if (System.Array.IndexOf(all, replaced) >= 0) throw new ArgumentException();

            var inactivated = false;
            var buffer = new System.Text.StringBuilder();
            for (int i = 0; i < path.Length; ++i)
            {
                var c = (System.Array.IndexOf(invalids, path[i]) >= 0) ? replaced : path[i];
                switch (c)
                {
                    case ':':  // ドライブ指定 (C:\foo) かどうかを判定
                        int first = inactivated ? 5 : 1;
                        if (!(i == first && char.IsLetter(path[i - 1]) && i + 1 < path.Length && path[i + 1] == '\\')) c = replaced;
                        break;
                    case '?':  // 拡張機能の不活性化指定 (\\?\) かどうかを判定
                        if (i + 1 < path.Length && i == 2 && path[i - 1] == '\\' && path[i - 2] == '\\' && path[i + 1] == '\\') inactivated = true;
                        else c = replaced;
                        break;
                    case '\\': // ホスト名指定 (\\server\foo) 以外の \ 記号の重複は取り除く
                        if (i > 1 && path[i - 1] == '\\') continue;
                        break;
                    default:
                        break;
                }
                buffer.Append(c);
            }

            // 末尾の . 記号や半角スペースは取り除く
            // ただし、拡張機能の不活性化が指定されている場合は保持する
            if (!inactivated) TrimRight(buffer);
            return buffer.ToString();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// NormalizeFilename
        /// 
        /// <summary>
        /// 引数 filename をファイル名として使用可能な文字列に変換します。
        /// filename に含まれるファイル名に使用できない文字は replaced に
        /// 置換されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static string NormalizeFilename(string filename, char replaced)
        {
            char[] invalids = { '/', '*', '"', '<', '>', '|', '?', ':', '\\' };
            if (System.Array.IndexOf(invalids, replaced) >= 0) throw new ArgumentException();

            var buffer = new System.Text.StringBuilder();
            foreach (var c in filename)
            {
                var normalized = (System.Array.IndexOf(invalids, c) >= 0) ? replaced : c;
                buffer.Append(normalized);
            }
            TrimRight(buffer);
            return buffer.ToString();
        }

        #region Private methods

        /* ----------------------------------------------------------------- */
        ///
        /// TrimRight
        /// 
        /// <summary>
        /// 末尾に存在する . 記号や半角スペースを取り除きます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static int TrimRight(System.Text.StringBuilder buffer)
        {
            int n = 0;
            for (int i = buffer.Length - 1; i >= 0; --i)
            {
                if (buffer[i] == '.' || buffer[i] == ' ') ++n;
                else break;
            }
            if (n > 0) buffer.Remove(buffer.Length - n, n);
            return n;
        }

        #endregion
    }
}
