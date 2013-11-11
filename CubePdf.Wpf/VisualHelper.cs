/* ------------------------------------------------------------------------- */
///
/// VisualHelper
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
using System.Windows;
using System.Windows.Media;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// VisualHelper
    /// 
    /// <summary>
    /// WPF の各種コンポーネントに関する補助メソッド群を定義したクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public abstract class VisualHelper
    {
        /* ----------------------------------------------------------------- */
        ///
        /// FindVisualChild
        /// 
        /// <summary>
        /// 子要素のうち最初に見つかった T 型のオブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static T FindVisualChild<T>(System.Windows.DependencyObject obj) where T : System.Windows.DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i)
            {
                var child = VisualTreeHelper.GetChild(obj, i) as T;
                if (child != null) return child;
                else
                {
                    var grandchild = FindVisualChild<T>(child);
                    if (grandchild != null) return grandchild;
                }
            }
            return null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FindVisualParent
        /// 
        /// <summary>
        /// 親要素のうち最初に見つかった T 型のオブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static T FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                if (obj is T) break;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return obj as T;
        }

    }
}
