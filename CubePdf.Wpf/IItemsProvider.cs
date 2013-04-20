/* ------------------------------------------------------------------------- */
///
/// IItemsProvider.cs
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

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// IItemsProvider
    /// 
    /// <summary>
    /// ListView で表示されるデータを仮想化するために、IListViewModel
    /// オブジェクトとコレクションオブジェクトの間でやり取りするための
    /// インターフェースです。
    /// 
    /// TODO: IItemsProvider を使用する事になる IListProxy インターフェース
    /// でも内部に要素を保持する可能性があるので、その情報へ参照できるように
    /// した方が良いか検討する。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public interface IItemsProvider<T>
    {
        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItemsCount
        ///
        /// <summary>
        /// ListView で表示されるはずの要素数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        int ProvideItemsCount();

        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItem
        ///
        /// <summary>
        /// ListView の index 番目に対応するデータを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        T ProvideItem(int index);
    }
}
