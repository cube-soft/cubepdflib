/* ------------------------------------------------------------------------- */
///
/// Environment.cs
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
using IWshRuntimeLibrary;

namespace CubePdf.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// Environment
    /// 
    /// <summary>
    /// System.Environment クラスを利用した各種メソッドを定義するための
    /// クラスです。Windows システムに関連する各種処理を提供します。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static class Environment
    {
        /* ----------------------------------------------------------------- */
        ///
        /// GetRecentFiles
        /// 
        /// <summary>
        /// システムの「最近開いたファイル」から pattern に一致するファイル
        /// 一覧を取得します（.lnk は自動的に付与されます）。
        /// </summary>
        /// 
        /// <remarks>
        /// 取得されるパスは、リンク先の最終的なファイルへのパスです。
        /// 「最近開いたファイル」のうち、既に存在しないファイルは結果に
        /// 含まれません。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public static IList<string> GetRecentFiles(string pattern)
        {
            var dest   = new List<string>();
            var shell  = new IWshShell_Class();
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Recent);
            var links  = System.IO.Directory.GetFiles(folder + "\\", pattern + ".lnk");

            foreach (var link in links)
            {
                var shortcut = shell.CreateShortcut(link) as IWshShortcut_Class;
                if (shortcut == null || !System.IO.File.Exists(shortcut.TargetPath)) continue;
                dest.Add(shortcut.TargetPath);
            }

            return dest;
        }
    }
}
