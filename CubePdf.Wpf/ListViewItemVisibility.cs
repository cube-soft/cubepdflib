/* ------------------------------------------------------------------------- */
///
/// ListViewItemVisibility.cs
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

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListViewItemVisibility
    /// 
    /// <summary>
    /// ListView に表示させる各項目（サムネイル）の表示方法を定義した
    /// 列挙型です。各値の意味は以下の通りです。
    /// 
    /// Normal      : 通常通りのサムネイルを表示します
    /// LightWeight : パフォーマンス（省メモリ等）を重視して表示します
    /// Minimum     : 最低限のもの（枠線のみ等）のみを表示します
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum ListViewItemVisibility
    {
        Normal,
        LightWeight,
        Minimum,
    }
}
