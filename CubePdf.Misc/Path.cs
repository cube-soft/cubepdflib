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

            var inactivated = false;
            var buffer = new System.Text.StringBuilder();
            for (int i = 0; i < path.Length; ++i)
            {
                var current = path[i];
                foreach (var c in invalids)
                {
                    if (current == c) current = replaced;
                }

                // ドライブ指定としての : 記号かどうかを判定する
                // 拡張機能の不活性化が指定されていない場合、C:\hoge
                // 指定されている場合は \\?\C:\hoge の位置でのみ : 記号が出現する事を許す
                if (current == ':')
                {
                    int first = inactivated ? 5 : 1;
                    if (i == first && char.IsLetter(path[i - 1]) && i + 1 < path.Length && path[i + 1] == '\\')
                    {
                        // ドライブ指定なので : 記号を保持
                    }
                    else current = replaced;
                }

                // 拡張機能の不活性化指定である \\?\ のみ許す
                if (current == '?')
                {
                    if (i + 1 < path.Length && i == 2 && path[i - 1] == '\\' && path[i - 2] == '\\' && path[i + 1] == '\\')
                    {
                        inactivated = true;
                    }
                    else current = replaced;
                }

                // ホスト名指定 (最初の \\server\hoge) 以外の \ 記号の重複は取り除く
                if (current == '\\')
                {
                    if (i > 1 && path[i - 1] == '\\') continue;
                }

                buffer.Append(current);
            }

            // 末尾の . 記号や半角スペースは取り除く
            // ただし、拡張機能の不活性化が指定されている場合は保持する
            if (!inactivated)
            {
                int n = 0;
                for (int i = buffer.Length - 1; i >= 0; --i)
                {
                    if (buffer[i] == '.' || buffer[i] == ' ') ++n;
                    else break;
                }
                if (n > 0) buffer.Remove(buffer.Length - n, n);
            }

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

            var buffer = new System.Text.StringBuilder();
            for (int i = 0; i < filename.Length; ++i)
            {
                char current = filename[i];
                foreach (char c in invalids)
                {
                    if (current == c) current = replaced;
                }
                buffer.Append(current);
            }

            // 末尾の . 記号や半角スペースは取り除く
            int n = 0;
            for (int i = buffer.Length - 1; i >= 0; --i)
            {
                if (buffer[i] == '.' || buffer[i] == ' ') ++n;
                else break;
            }
            if (n > 0) buffer.Remove(buffer.Length - n, n);

            return buffer.ToString();
        }
    }
}
