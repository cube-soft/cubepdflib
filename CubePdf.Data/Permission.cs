/* ------------------------------------------------------------------------- */
///
/// Permission.cs
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

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// Permission
    /// 
    /// <summary>
    /// 暗号化されている PDF ファイルで許可されている権限を表すクラスです。
    /// 
    /// NOTE: iTextSharp の現在のバージョンでは、ExtractPage, Signature,
    /// TemplatePage プロパティに相当するパーミッション設定が存在しません。
    /// そのため、これらのプロパティの値は無視される可能性が高いです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Permission : IReadOnlyPermission
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// Permission (constructor)
        ///
        /// <summary>
        /// 規定の値で Permission クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Permission() { }

        /* ----------------------------------------------------------------- */
        ///
        /// Permission (constructor)
        /// 
        /// <summary>
        /// コピー元となる IReadOnlyPermission オブジェクトを指定して
        /// Permission クラスを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Permission(IReadOnlyPermission cp)
        {
            this.Printing = cp.Printing;
            this.Assembly = cp.Assembly;
            this.ModifyContents = cp.ModifyContents;
            this.CopyContents = cp.CopyContents;
            this.Accessibility = cp.Accessibility;
            this.ExtractPage = cp.ExtractPage;
            this.ModifyAnnotations = cp.ModifyAnnotations;
            this.InputFormFields = cp.InputFormFields;
            this.Signature = cp.Signature;
            this.TemplatePage = cp.TemplatePage;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Printing
        ///
        /// <summary>
        /// 印刷操作が許可されているかどうかを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Printing { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// DegradedPrinting
        /// 
        /// <summary>
        /// テイン品質の印刷操作が許可されているかどうかを取得、または設定
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool DegradedPrinting { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Assembly
        /// 
        /// <summary>
        /// 文書アセンブリ（ページの挿入・削除・回転、しおりとサムネイルの
        /// 作成）操作が許可されているかどうかを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Assembly { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// ModifyContents
        /// 
        /// <summary>
        /// 内容の編集操作が許可されているかどうかを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool ModifyContents { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// CopyContents
        /// 
        /// <summary>
        /// 内容の選択/コピー操作が許可されているかどうかを取得、または設定
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool CopyContents { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Accessibility
        /// 
        /// <summary>
        /// アクセシビリティ（視覚に障害を持つユーザに対して、読み上げ機能
        /// を提供する）のための内容の抽出操作が許可されているかどうかを
        /// 取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Accessibility { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractPage
        /// 
        /// <summary>
        /// ページの抽出操作が許可されているかどうかを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool ExtractPage { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// ModifyAnnotations
        /// 
        /// <summary>
        /// 注釈の追加・編集操作が許可されているかを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool ModifyAnnotations { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// InputFormFields
        /// 
        /// <summary>
        /// フォームフィールドへの入力操作が許可されているかどうかを取得、
        /// または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool InputFormFields { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Signature
        /// 
        /// <summary>
        /// 既存の署名フィールドへの署名が許可されているかどうかを取得、
        /// または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Signature { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// TemplatePage
        /// 
        /// <summary>
        /// コンテンツの動的な作成等に利用するテンプレートページの作成が
        /// 許可されているかどうかを取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool TemplatePage { get; set; }

        #endregion
    }
}
