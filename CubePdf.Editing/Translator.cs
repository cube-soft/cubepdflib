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
using System.Drawing;
using CubePdf.Data;
using iTextSharp.text.pdf;

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
        /// ToSize
        /// 
        /// <summary>
        /// 引数に指定された iTextSharp の Rectangle オブジェクトから PDF
        /// のサイズ情報を抽出して返します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static Size ToSize(iTextSharp.text.Rectangle rect)
        {
            return new Size((int)rect.Width, (int)rect.Height);
        }

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
        public static int ToIText(EncryptionMethod value)
        {
            switch (value)
            {
                case EncryptionMethod.Standard40:  return PdfWriter.STANDARD_ENCRYPTION_40;
                case EncryptionMethod.Standard128: return PdfWriter.STANDARD_ENCRYPTION_128;
                case EncryptionMethod.Aes128:      return PdfWriter.ENCRYPTION_AES_128;
                case EncryptionMethod.Aes256:      return PdfWriter.ENCRYPTION_AES_256;
                default: break;
            }
            return -1;
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
        public static EncryptionMethod ToEncryptionMethod(int value)
        {
            switch (value)
            {
                case PdfWriter.STANDARD_ENCRYPTION_40:  return EncryptionMethod.Standard40;
                case PdfWriter.STANDARD_ENCRYPTION_128: return EncryptionMethod.Standard128;
                case PdfWriter.ENCRYPTION_AES_128:      return EncryptionMethod.Aes128;
                case PdfWriter.ENCRYPTION_AES_256:      return EncryptionMethod.Aes256;
                default: break;
            }
            return EncryptionMethod.Unknown;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToIText
        /// 
        /// <summary>
        /// 引数に指定された CubePdf.Data.Permission オブジェクトをに対応
        /// する（iTextSharp で定義されている）値を返します。
        /// 
        /// NOTE: Signature, TemplatePage のパーミッションに関しては、
        /// iTextSharp が未実装なので無視します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static int ToIText(Permission value)
        {
            int dest = 0;

            if (value.Printing)          dest |= PdfWriter.AllowPrinting;
            if (value.DegradedPrinting)  dest |= PdfWriter.AllowDegradedPrinting;
            if (value.Assembly)          dest |= PdfWriter.AllowAssembly;
            if (value.ModifyContents)    dest |= PdfWriter.AllowModifyContents;
            if (value.CopyContents)      dest |= PdfWriter.AllowCopy;
            if (value.InputFormFields)   dest |= PdfWriter.AllowFillIn;
            if (value.ModifyAnnotations) dest |= PdfWriter.AllowModifyAnnotations;
            if (value.Accessibility)     dest |= PdfWriter.AllowScreenReaders;
            // if (value.Signature) dest |= ???
            // if (value.TemplatePage) dest |= ???

            return 0;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ToPermission
        /// 
        /// <summary>
        /// 引数に指定された値に対応する CubePdf.Data.Permission オブジェクト
        /// を返します。
        /// 
        /// TODO: うまく機能してない？原因等を調査する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static Permission ToPermission(int value)
        {
            var dest = new Permission();

            if ((value & PdfWriter.AllowPrinting) != 0)          dest.Printing = true;
            if ((value & PdfWriter.AllowDegradedPrinting) != 0)  dest.DegradedPrinting = true;
            if ((value & PdfWriter.AllowAssembly) != 0)          dest.Assembly = true;
            if ((value & PdfWriter.AllowModifyContents) != 0)    dest.ModifyContents = true;
            if ((value & PdfWriter.AllowCopy) != 0)              dest.CopyContents = true;
            if ((value & PdfWriter.AllowFillIn) != 0)            dest.InputFormFields = true;
            if ((value & PdfWriter.AllowModifyAnnotations) != 0) dest.ModifyAnnotations = true;
            if ((value & PdfWriter.AllowScreenReaders) != 0)     dest.Accessibility = true;
            // if ((value & ???) != 0) dest.Signature = true;
            // if ((value & ???) != 0) dest.TemplatePage = true;
            
            return new CubePdf.Data.Permission();
        }
    }
}
