/* ------------------------------------------------------------------------- */
///
/// ListViewCommands.cs
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
using System.Windows;
using System.Windows.Input;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListViewCommands
    /// 
    /// <summary>
    /// ListView で PDF のサムネイルを表示、および各種操作を行うための
    /// コマンド群を定義したクラスです。コマンドに対応する実際の処理は、
    /// IListViewModel を継承したクラスで行われます。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public static class ListViewCommands
    {
        #region Properties
        public static ICommand Add     { get { return _add; } }
        public static ICommand Insert  { get { return _insert; } }
        public static ICommand Extract { get { return _extract; } }
        public static ICommand Remove  { get { return _remove; } }
        public static ICommand Move    { get { return _move; } }
        public static ICommand Rotate  { get { return _rotate; } }
        public static ICommand Undo    { get { return _undo; } }
        #endregion

        #region Variables
        private static readonly ICommand _add     = new RoutedCommand("Add",     typeof(ListViewCommands));
        private static readonly ICommand _insert  = new RoutedCommand("Insert",  typeof(ListViewCommands));
        private static readonly ICommand _extract = new RoutedCommand("Extract", typeof(ListViewCommands));
        private static readonly ICommand _remove  = new RoutedCommand("Remove",  typeof(ListViewCommands));
        private static readonly ICommand _move    = new RoutedCommand("Move",    typeof(ListViewCommands));
        private static readonly ICommand _rotate  = new RoutedCommand("Rotate",  typeof(ListViewCommands));
        private static readonly ICommand _undo    = new RoutedCommand("Undo",    typeof(ListViewCommands));
        #endregion
    }
}
