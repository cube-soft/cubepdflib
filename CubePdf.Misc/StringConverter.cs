/* ------------------------------------------------------------------------- */
///
/// StringConverter.cs
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
using System.Runtime.InteropServices;
using System.Text;

namespace CubePdf.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// StringConverter
    /// 
    /// <summary>
    /// 文字列を扱う際の補助関数群が定義されたクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static class StringConverter
    {
        /* ----------------------------------------------------------------- */
        ///
        /// FormatByteSize
        /// 
        /// <summary>
        /// ファイルサイズ（バイト）をプロパティ欄に表示される書式（例えば、
        /// 1.23 KB, 45.6 MB 等）に変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static string FormatByteSize(long filesize)
        {
            var capacity = 64;
            var buffer = new StringBuilder(capacity);
            Win32Api.StrFormatByteSize(filesize, buffer, capacity);
            return buffer.ToString();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ParseRange
        /// 
        /// <summary>
        /// 範囲を表す文字列を解析し、対応する範囲の配列に変換します。
        /// 範囲を表す文字列は以下の規則に従う事とします。
        /// 
        /// range  = value , [ { "," value } ]
        /// value  = number | number , "-" , number
        /// number = digit , [ { digit } ]
        /// digit  = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static IList<int> ParseRange(string str)
        {
            try
            {
                var dest = new List<int>();

                var range = str.Split(',');
                foreach (var value in range)
                {
                    if (value.IndexOf('-') != -1)
                    {
                        var numbers = value.Split('-');
                        if (numbers.Length != 2) throw new ArgumentException();
                        for (int i = int.Parse(numbers[0]); i <= int.Parse(numbers[1]); ++i) dest.Add(i);
                    }
                    else dest.Add(int.Parse(value));
                }
                dest.Sort();

                return dest;
            }
            catch (Exception /* err */)
            {
                throw new ArgumentException(Properties.Resources.ParseRangeException);
            }
        }

        #region Win32 APIs

        internal class Win32Api {
            [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
            public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);
        }

        #endregion
    }
}
