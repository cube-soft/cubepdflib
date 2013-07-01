/* ------------------------------------------------------------------------- */
///
/// FileFormat.cs
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

namespace CubePdf.Settings
{
    /* --------------------------------------------------------------------- */
    ///
    /// FileFormat
    /// 
    /// <summary>
    /// 設定を保存するためのファイルの種類を定義するための列挙型です。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum FileFormat
    {
        Xml,
        //Json,
        Unknown = -1,
    }
}
