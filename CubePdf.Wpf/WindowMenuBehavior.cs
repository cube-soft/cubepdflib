/* ------------------------------------------------------------------------- */
///
/// WindowMenuBehavior.cs
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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// WindowMenuBehavior
    /// 
    /// <summary>
    /// ウィンドウの最大化や最小化ボタンを無効にするためのクラスです。
    /// http://d.hatena.ne.jp/hilapon/20110726/1311643302
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class WindowMenuBehavior
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// MinimizeBox
        /// 
        /// <summary>
        /// 最小化ボタンの状態（有効/無効）を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region MinimizeBox

        [AttachedPropertyBrowsableForType(typeof(WindowMenuBehavior))]
        public static bool GetMinimizeBox(DependencyObject obj)
        {
            return (bool)obj.GetValue(MinimizeBoxProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(WindowMenuBehavior))]
        public static void SetMinimizeBox(DependencyObject obj, bool value)
        {
            obj.SetValue(MinimizeBoxProperty, value);
        }

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// MaximizeBox
        /// 
        /// <summary>
        /// 最大化ボタンの状態（有効/無効）を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region MaximizeBox

        [AttachedPropertyBrowsableForType(typeof(WindowMenuBehavior))]
        public static bool GetMaximizeBox(DependencyObject obj)
        {
            return (bool)obj.GetValue(MaximizeBoxProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(WindowMenuBehavior))]
        public static void SetMaximizeBox(DependencyObject obj, bool value)
        {
            obj.SetValue(MaximizeBoxProperty, value);
        }

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// ControlBox
        /// 
        /// <summary>
        /// 終了ボタンの状態（有効/無効）を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region ControlBox

        [AttachedPropertyBrowsableForType(typeof(WindowMenuBehavior))]
        public static bool GetControlBox(DependencyObject obj)
        {
            return (bool)obj.GetValue(ControlBoxProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(WindowMenuBehavior))]
        public static void SetControlBox(DependencyObject obj, bool value)
        {
            obj.SetValue(ControlBoxProperty, value);
        }

        #endregion

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// SourceInitialized
        /// 
        /// <summary>
        /// それぞれの DependencyProperty が初期化される際に実行される
        /// コールバック関数です。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static void SourceInitialized(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            if (window == null) return;

            window.SourceInitialized += (obj, args) => {
                var w = obj as Window;
                if (w == null) return;

                var handle = (new WindowInteropHelper(w)).Handle;
                var original = (WindowStyleFlag)Win32Api.GetWindowLong(handle, GWL_STYLE);
                var current = GetWindowStyle(w, original, e);
                if (original != current) Win32Api.SetWindowLong(handle, GWL_STYLE, current);                
            };
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetWindowStyle
        /// 
        /// <summary>
        /// 現在の設定に対応する WindowStyleFlag を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static WindowStyleFlag GetWindowStyle(DependencyObject obj, WindowStyleFlag original, DependencyPropertyChangedEventArgs e)
        {
            var style = WindowStyleFlag.WS_NONE;
            var enabled = false;

            switch (e.Property.Name)
            {
            case "MinimizeBox":
                style = WindowStyleFlag.WS_MINIMIZEBOX;
                enabled = (bool)obj.GetValue(MinimizeBoxProperty);
                break;
            case "MaximizeBox":
                style = WindowStyleFlag.WS_MAXIMIZEBOX;
                enabled = (bool)obj.GetValue(MaximizeBoxProperty);
                break;
            case "ControlBox":
                style = WindowStyleFlag.WS_SYSMENU;
                enabled = (bool)obj.GetValue(ControlBoxProperty);
                break;
            }
            return enabled ? (original | style) : (original & ~style);
        }

        #endregion

        #region Internal classes

        [Flags]
        internal enum WindowStyleFlag : uint
        {
            WS_NONE = 0x00000, WS_SYSMENU = 0x80000, WS_MINIMIZEBOX = 0x20000, WS_MAXIMIZEBOX = 0x10000,
        }

        internal class Win32Api
        {
            [DllImport("user32")]
            public static extern uint GetWindowLong(IntPtr hWnd, int index);

            [DllImport("user32")]
            public static extern uint SetWindowLong(IntPtr hWnd, int index, WindowStyleFlag dwLong);
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty MinimizeBoxProperty =
            DependencyProperty.RegisterAttached("MinimizeBox",
                typeof(bool),
                typeof(WindowMenuBehavior),
                new UIPropertyMetadata(true, SourceInitialized));

        public static readonly DependencyProperty MaximizeBoxProperty =
            DependencyProperty.RegisterAttached("MaximizeBox",
                typeof(bool),
                typeof(WindowMenuBehavior),
                new UIPropertyMetadata(true, SourceInitialized));

        public static readonly DependencyProperty ControlBoxProperty =
            DependencyProperty.RegisterAttached("ControlBox",
                typeof(bool),
                typeof(WindowMenuBehavior),
                new UIPropertyMetadata(true, SourceInitialized));

        #endregion

        #region Static variables
        private static readonly int GWL_STYLE = -16;
        #endregion
    }
}
