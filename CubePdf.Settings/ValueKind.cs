/* ------------------------------------------------------------------------- */
///
/// DocumentReader.cs
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

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// ValueKind
    /// 
    /// <summary>
    /// Settings プロジェクトで扱える値の種類を定義した列挙型です。
    /// 各値の意味は以下の通りです。
    /// 
    /// NodeSet : 複数の値のコレクション
    /// String  : 文字列
    /// Number  : 整数値
    /// Bool    : 真偽値
    /// Unknown : 不明（エラー）
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum ValueKind : int
    {
        NodeSet,
        String,
        Number,
        Bool,
        Unknown = -1,
    }
}
