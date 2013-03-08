/* ------------------------------------------------------------------------- */
///
/// DocumentReader.cs
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

namespace CubePdf.Editing
{
    /* --------------------------------------------------------------------- */
    ///
    /// Translator
    /// 
    /// <summary>
    /// CubePdf.Data で定義している各データ型と iTextSharp 内部で使用されて
    /// いる型（または値）の相互変換を補助するためのクラスです。
    /// 
    /// NOTE: このクラスは CubePdf.Editing プロジェクト内部でのみ使用します。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    internal class Translator
    {
        /* ----------------------------------------------------------------- */
        ///
        /// ToIText
        /// 
        /// <summary>
        /// 引数に指定された CubePdf.Data.EncryptionMethod オブジェクトに
        /// 対応する（iTextSharp で定義されている）値を返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static int ToIText(CubePdf.Data.EncryptionMethod value)
        {
            switch (value)
            {
                case Data.EncryptionMethod.Standard40:
                    return iTextSharp.text.pdf.PdfWriter.STANDARD_ENCRYPTION_40;
                case Data.EncryptionMethod.Standard128:
                    return iTextSharp.text.pdf.PdfWriter.STANDARD_ENCRYPTION_128;
                case Data.EncryptionMethod.Aes128:
                    return iTextSharp.text.pdf.PdfWriter.ENCRYPTION_AES_128;
                case Data.EncryptionMethod.Aes256:
                    return iTextSharp.text.pdf.PdfWriter.ENCRYPTION_AES_256;
                default:
                    break;
            }
            return iTextSharp.text.pdf.PdfWriter.STANDARD_ENCRYPTION_40;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToEncryptionMethod
        /// 
        /// <summary>
        /// 引数に指定された値に対応する CubePdf.Data.EncryptionMethod
        /// オブジェクトを返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static CubePdf.Data.EncryptionMethod ToEncryptionMethod(int value)
        {
            switch (value)
            {
                case iTextSharp.text.pdf.PdfWriter.STANDARD_ENCRYPTION_40:
                    return CubePdf.Data.EncryptionMethod.Standard40;
                case iTextSharp.text.pdf.PdfWriter.STANDARD_ENCRYPTION_128:
                    return CubePdf.Data.EncryptionMethod.Standard128;
                case iTextSharp.text.pdf.PdfWriter.ENCRYPTION_AES_128:
                    return CubePdf.Data.EncryptionMethod.Aes128;
                case iTextSharp.text.pdf.PdfWriter.ENCRYPTION_AES_256:
                    return CubePdf.Data.EncryptionMethod.Aes256;
                default:
                    break;
            }
            return Data.EncryptionMethod.Standard40;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToIText
        /// 
        /// <summary>
        /// 引数に指定された CubePdf.Data.Permission オブジェクトをに対応
        /// する（iTextSharp で定義されている）値を返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static int ToIText(CubePdf.Data.Permission value)
        {
            // TODO: implementation
            return 0;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToEncryptionMethod
        /// 
        /// <summary>
        /// 引数に指定された値に対応する CubePdf.Data.Permission オブジェクト
        /// を返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static CubePdf.Data.Permission ToPermission(int value)
        {
            // TODO: implementation
            return new CubePdf.Data.Permission();
        }
    }
}
