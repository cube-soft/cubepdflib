﻿/* ------------------------------------------------------------------------- */
/*
 *  File.cs
 *
 *  Copyright (c) 2009 - 2013 CubeSoft, Inc. All rights reserved.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see < http://www.gnu.org/licenses/ >.
 */
/* ------------------------------------------------------------------------- */

namespace CubePdf.Misc
{
    /* --------------------------------------------------------------------- */
    ///
    /// File
    /// 
    /// <summary>
    /// File の移動、コピー、削除等の処理をラップしたクラスです。
    /// </summary>
    /// 
    /// <remarks>
    /// 各種、確認ダイアログを表示させるかどうかの挙動を簡単に変えるために
    /// 各種処理をラップしています。確認ダイアログを表示させる場合は
    /// Microsoft.VisualBasic.FileIO.FileSystem クラスを、表示させない場合は
    /// System.IO.File クラスを使用します。
    /// </remarks>
    ///
    /* --------------------------------------------------------------------- */
    public abstract class File
    {
        /* ----------------------------------------------------------------- */
        ///
        /// Exists
        ///
        /// <summary>
        /// 引数に指定されたファイルが存在するかどうかを確認します。
        /// </summary>
        /// 
        /// <remarks>
        /// Exists メソッドに関しては、単に System.IO.Exists メソッドを
        /// 実行するだけの実装となっています。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Delete
        ///
        /// <summary>
        /// 引数に指定されたファイルを削除します。
        /// </summary>
        /// 
        /// <remarks>
        /// show_prompt が true の場合、ファイルがロックされている等の理由で
        /// ファイルが削除できなかった際に確認ダイアログが表示されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public static void Delete(string path, bool show_prompt)
        {
            try { System.IO.File.Delete(path); }
            catch (System.IO.IOException err)
            {
                if (show_prompt && ShowPrompt(path)) Delete(path, show_prompt);
                else throw err;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Copy
        ///
        /// <summary>
        /// 引数 src に指定されたファイルを dest へコピーします。
        /// </summary>
        /// 
        /// <remarks>
        /// show_prompt が true の場合、ファイルがロックされている等の理由で
        /// ファイルのコピーに失敗した際に確認ダイアログが表示されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public static void Copy(string src, string dest, bool show_prompt)
        {
            try { System.IO.File.Copy(src, dest, true); }
            catch (System.IO.IOException err)
            {
                if (show_prompt && ShowPrompt(dest)) Copy(src, dest, show_prompt);
                else throw err;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Move
        ///
        /// <summary>
        /// 引数 src に指定されたファイルを dest へ移動します。
        /// </summary>
        /// 
        /// <remarks>
        /// show_prompt が true の場合、ファイルがロックされている等の理由で
        /// ファイルの移動に失敗した際に確認ダイアログが表示されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public static void Move(string src, string dest, bool show_prompt)
        {
            try
            {
                System.IO.File.Delete(dest);
                System.IO.File.Move(src, dest);
            }
            catch (System.IO.IOException err)
            {
                if (show_prompt && ShowPrompt(dest)) Move(src, dest, show_prompt);
                else throw err;
            }
        }

        #region Private methods

        /* ----------------------------------------------------------------- */
        ///
        /// ShowPrompt
        /// 
        /// <summary>
        /// 警告ダイアログを表示します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static bool ShowPrompt(string path)
        {
            var result = System.Windows.Forms.MessageBox.Show(
                string.Format(Properties.Resources.AccessDenied, path),
                Properties.Resources.AccessDeniedTitle,
                System.Windows.Forms.MessageBoxButtons.OKCancel,
                System.Windows.Forms.MessageBoxIcon.Warning
            );
            return result == System.Windows.Forms.DialogResult.OK;
        }

        #endregion
    }
}
