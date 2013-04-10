/* ------------------------------------------------------------------------- */
///
/// ListViewDragBehavior.cs
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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interactivity;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListViewDragBehavior
    ///
    /// <summary>
    /// ListView のドラッグ&ドロップ時の挙動を実装したクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ListViewDragBehavior : Behavior<ListView>
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// ViewModel
        ///
        /// <summary>
        /// ListView とデータを結びつけるための ViewModel を取得、または
        /// 設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IListViewModel ViewModel
        {
            get { return (IListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        #region Event handlers

        /* ----------------------------------------------------------------- */
        ///
        /// OnMouseLeftButtonDown
        ///
        /// <summary>
        /// マウスの左ボタンが押下された時の挙動を記述するためのイベント
        /// ハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _target = -1;
            _begin  = e.GetPosition(AssociatedObject);
            _source = GetItemIndex(_begin);
            if (_source >= 0) AssociatedObject.AllowDrop = true;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnMouseMove
        ///
        /// <summary>
        /// マウスが移動中に実行する挙動を記述するためのイベントハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_source >= 0 && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(AssociatedObject, _source, DragDropEffects.Move);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnDrop
        ///
        /// <summary>
        /// マウスのドロップ時の挙動を記述するためのイベントハンドラです。
        /// 
        /// TODO: 水平方向に見て 2 つの項目の間にドロップされた場合でも
        /// 移動を受け付けるようにしているが、現在は項目間かどうかを
        /// 判断するための x 軸の量がマジックナンバーとなっている。
        /// 項目のマージンを取得して、その値を元にするように修正する。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnDrop(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(AssociatedObject);
            _target = GetItemIndex(pos);
            if (_target == -1)
            {
                int margin = (pos.X <= _begin.X) ? 5 : -5; // TODO: ListViewItem.Margin の値で計算したい
                pos.X += margin;
                _target = GetItemIndex(pos);
            }

            if (_source != -1 && _target != -1 && _source != _target) MoveItems();
            AssociatedObject.AllowDrop = false;
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetItemIndex
        ///
        /// <summary>
        /// マウスカーソルのある項目のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private int GetItemIndex(Point position)
        {
            var result = VisualTreeHelper.HitTest(AssociatedObject, position);
            if (result == null) return -1;

            var item = result.VisualHit;
            while (item != null)
            {
                if (item is ListViewItem) break;
                item = VisualTreeHelper.GetParent(item);
            }
            return (item != null) ? AssociatedObject.Items.IndexOf(((ListViewItem)item).Content) : -1;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MoveItems
        /// 
        /// <summary>
        /// ドラッグ&ドロップされた位置（から割り出したインデックス）を
        /// 元にして、現在、選択されている項目を移動します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void MoveItems()
        {
            var delta = _target - _source;

            try
            {
                var indices = new List<int>();
                foreach (var item in AssociatedObject.SelectedItems)
                {
                    var index = ViewModel.Items.IndexOf(item as System.Drawing.Image);
                    indices.Add(index);
                }

                ViewModel.BeginCommand();
                var sorted = (delta < 0) ? indices.OrderBy(i => i) : indices.OrderByDescending(i => i);
                foreach (var oldindex in sorted)
                {
                    if (oldindex < 0) continue;
                    var newindex = oldindex + delta;
                    if (newindex < 0 || newindex >= ViewModel.ItemCount) continue;
                    ViewModel.Move(oldindex, newindex);
                }
            }
            finally { ViewModel.EndCommand(); }
        }

        #endregion

        #region Methods for inheriting from the superclass

        /* ----------------------------------------------------------------- */
        /// OnAttached
        /* ----------------------------------------------------------------- */
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.Drop += OnDrop;
            base.OnAttached();
        }

        /* ----------------------------------------------------------------- */
        /// OnDetaching
        /* ----------------------------------------------------------------- */
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.Drop -= OnDrop;
            base.OnDetaching();
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                typeof(IListViewModel),
                typeof(ListViewDragBehavior),
                new PropertyMetadata(null));

        #endregion

        #region Variables
        private Point _begin = new Point();
        private int _source = -1;
        private int _target = -1;
        #endregion
    }
}
