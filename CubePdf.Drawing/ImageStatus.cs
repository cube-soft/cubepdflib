/* ------------------------------------------------------------------------- */
///
/// ImageStatus.cs
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

namespace CubePdf.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageStatus
    /// 
    /// <summary>
    /// ImageContainer クラスの Image プロパティの状態を定義した列挙型です。
    /// 各値の意味は以下の通りです。
    /// 
    /// None    : イメージは設定されていません (null)
    /// Created : 本来、設定されているべきイメージが設定済みです
    /// Dummy   : ダミーイメージ（サイズのみ同じ）が設定されています
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum ImageStatus
    {
        None,
        Created,
        Dummy,
    }
}
