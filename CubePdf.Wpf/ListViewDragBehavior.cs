/* ------------------------------------------------------------------------- */
///
/// ListViewDragBehavior.cs
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
        /* ----------------------------------------------------------------- */
        ///
        /// ListViewDragBehavior
        /// 
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListViewDragBehavior()
            : base()
        {
            _canvas.Visibility = Visibility.Collapsed;
            _rect.BorderBrush = SystemColors.HotTrackBrush;
            _rect.BorderThickness = new Thickness(1);
            _rect.Background = SystemColors.HotTrackBrush.Clone();
            _rect.Background.Opacity = 0.1;
            _rect.CornerRadius = new CornerRadius(1);
            _rect.DragOver += (sender, e) => { this.OnDragOver(sender, e); };
            _rect.Drop += (sender, e) => { this.OnDrop(sender, e); };
            _canvas.Children.Add(_rect);
        }

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
        public ListViewModel ViewModel
        {
            get { return (ListViewModel)GetValue(ViewModelProperty); }
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
        /// 
        /// NOTE: ドラッグ&ドラップによる範囲選択時、スクロールバーへの
        /// クリックが無効になる事があるので、スクロールバー分の幅の領域
        /// へのクリックは無視しています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ondrag = true;
            _position = e.GetPosition(AssociatedObject);
            var item = GetItem(_position);
            _source = (item != null) ? AssociatedObject.Items.IndexOf(item) : -1;
            _target = -1;

            if (_source >= 0)
            {
                AssociatedObject.AllowDrop = true;
                if (AssociatedObject.SelectedItems.Count > 1 && AssociatedObject.SelectedItems.Contains(item))
                {
                    var srcdata = new CubePdf.Data.Page(ViewModel.FilePath, _source);
                    var data = new DataObject(DataFormats.Serializable, srcdata);
                    var effects = (DragDropEffects.Copy | DragDropEffects.Move);
                    DragDrop.DoDragDrop(AssociatedObject, data, effects);
                }
            }
            else if (_position.X <= AssociatedObject.ActualWidth - SCROLLBAR_WIDTH)
            {
                AssociatedObject.CaptureMouse();
                RefreshDragSelection(_position, _position);
                _canvas.Visibility = Visibility.Visible;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnMouseLeftButtonUp
        ///
        /// <summary>
        /// マウスの左ボタンが離された時の挙動を記述するためのイベントハンドラ
        /// です。ドラッグ&ドロップ時の挙動は OnDrop で行われます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _canvas.Visibility = Visibility.Collapsed;
            if (!_ondrag) return;
            _ondrag = false;

            if (_source == -1)
            {
                AssociatedObject.ReleaseMouseCapture();
                SelectRange();
            }
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
            if (!_ondrag) return;

            if (e.LeftButton == MouseButtonState.Pressed && _source >= 0)
            {
                var srcdata = ViewModel.GetPage(_source + 1);
                //var srcdata = new CubePdf.Data.Page(ViewModel.FilePath, _source);
                var data = new DataObject(DataFormats.Serializable, srcdata);
                var effects = (DragDropEffects.Copy | DragDropEffects.Move);
                DragDrop.DoDragDrop(AssociatedObject, data, effects);
            }
            else RefreshDragSelection(_position, e.GetPosition(AssociatedObject));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnDragEnter
        ///
        /// <summary>
        /// 
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnDragEnter(object sender, DragEventArgs e)
        {   
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnDragOver
        ///
        /// <summary>
        /// マウスのドラッグ時の挙動を記述するためのイベントハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (!_ondrag) return;

            var pos = e.GetPosition(AssociatedObject);
            RefreshDragScroll(pos);
            RefreshMovingPosition(pos);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnDrop
        ///
        /// <summary>
        /// マウスのドロップ時の挙動を記述するためのイベントハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OnDrop(object sender, DragEventArgs e)
        {
            _canvas.Visibility = Visibility.Collapsed;
            //if (!_ondrag) return;
            _ondrag = false;

            e.Effects = DecideDragAction(sender, e);

            if (_source != -1 && e.Effects == DragDropEffects.Move)
            {
                MoveItems(e.GetPosition(AssociatedObject));
            }else{
                var a = e.Data.GetData(DataFormats.Serializable) as CubePdf.Data.IPage;
                if (a != null)
                {
                    ViewModel.Add(a);
                }
            }
            
            _source = -1;
            //AssociatedObject.AllowDrop = false;
        }

        #endregion

        #region Methods for moving items

        /* ----------------------------------------------------------------- */
        ///
        /// RefreshDragScroll
        /// 
        /// <summary>
        /// ドラッグ時にスクロールを行います。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RefreshDragScroll(Point current)
        {
            var sv = VisualHelper.FindVisualChild<ScrollViewer>(AssociatedObject);
            if (sv == null) return;

            var height = AssociatedObject.ActualHeight;
            var margin = height / 5.0;
            var offset = Math.Max(ViewModel.MaxItemHeight / 10, 50);

            if (current.Y < margin) sv.ScrollToVerticalOffset(sv.VerticalOffset - offset);
            else if (current.Y > height - margin) sv.ScrollToVerticalOffset(sv.VerticalOffset + offset);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RefreshMovingPosition
        /// 
        /// <summary>
        /// ドロップ時に挿入される位置の表示を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RefreshMovingPosition(Point current)
        {
            var index = GetTargetItemIndex(current);
            if (_source == -1 || index == -1 || _source == index)
            {
                _canvas.Visibility = Visibility.Collapsed;
                return;
            }
            _canvas.Visibility = Visibility.Visible;
            current = PointToWindow(current);

            var item   = AssociatedObject.Items[index];
            var lvi    = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            var rect   = GetItemBounds(item);
            var center = PointToWindow(new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));

            double width = rect.Width + lvi.Margin.Right + lvi.Padding.Right + lvi.BorderThickness.Left + lvi.BorderThickness.Right;
            double height = rect.Height / 2.0;
            double x = (current.X >= center.X) ? center.X : center.X - width;
            double y = center.Y - rect.Height / 4.0;

            Canvas.SetLeft(_rect, x - CURSOR_ADJUSTMENT);
            Canvas.SetTop(_rect, y - CURSOR_ADJUSTMENT);
            _rect.Width = width;
            _rect.Height = height;
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
        private void MoveItems(Point current)
        {
            var index = GetTargetItemIndex(current);
            if (index == -1) return;

            var rect = GetItemBounds(AssociatedObject.Items[index]);
            var center = PointToWindow(new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
            var pos = PointToWindow(current);

            if (_source < index && pos.X < center.X) _target = Math.Max(index - 1, 0);
            else if (_source > index && pos.X >= center.X) _target = Math.Min(index + 1, AssociatedObject.Items.Count);
            else _target = index;
            if (_source == _target) return;

            try
            {
                var delta = _target - _source;
                var indices = new List<int>();
                foreach (var item in AssociatedObject.SelectedItems) indices.Add(ViewModel.IndexOf(item));
                
                ViewModel.BeginCommand();
                var sorted = (delta < 0) ? indices.OrderBy(i => i) : indices.OrderByDescending(i => i);
                foreach (var oldindex in sorted)
                {
                    if (oldindex < 0) continue;
                    var newindex = oldindex + delta;
                    if (newindex < 0 || newindex >= ViewModel.PageCount) continue;
                    ViewModel.Move(oldindex, newindex);
                }
            }
            finally { ViewModel.EndCommand(); }
        }

        #endregion

        #region Methods for drag selection

        /* ----------------------------------------------------------------- */
        ///
        /// RefreshDragSelection
        ///
        /// <summary>
        /// マウスドラッグによる選択領域の描画を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void RefreshDragSelection(Point start, Point last)
        {
            start = PointToWindow(start);
            last = PointToWindow(last);

            double x = Math.Min(start.X, last.X);
            double y = Math.Min(start.Y, last.Y);
            double width = (start.X < last.X) ? last.X - start.X : start.X - last.X;
            double height = (start.Y < last.Y) ? last.Y - start.Y : start.Y - last.Y;

            Canvas.SetLeft(_rect, x - CURSOR_ADJUSTMENT);
            Canvas.SetTop(_rect, y - CURSOR_ADJUSTMENT);
            _rect.Width = width;
            _rect.Height = height;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SelectRange
        /// 
        /// <summary>
        /// ドラッグ&ドロップで指定した選択領域内に存在する ListView の
        /// 項目を選択状態にします。各項目が選択領域に含まれるかどうかの
        /// 判定基準は、各項目の中心点を利用しています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void SelectRange()
        {
            var x = Canvas.GetLeft(_rect);
            var y = Canvas.GetTop(_rect);
            var width = _rect.Width;
            var height = _rect.Height;
            var pt = PointFromWindow(new Point(x, y));
            var area = new Rect(pt.X, pt.Y, width, height);
            
            AssociatedObject.SelectedItems.Clear();
            foreach (var item in AssociatedObject.Items)
            {
                var rect = GetItemBounds(item);
                var center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                if (area.Contains(center)) AssociatedObject.SelectedItems.Add(item);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetItemBounds
        /// 
        /// <summary>
        /// 指定された項目（AssociatedObject.Items の各要素）の描画領域を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Rect GetItemBounds(object item)
        {
            var lvi = AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
            if (lvi == null) return new Rect();

            var dest = VisualTreeHelper.GetDescendantBounds(lvi);
            var transform = lvi.TransformToVisual((Visual)AssociatedObject).Transform(new Point());
            dest.Offset(transform.X, transform.Y);
            return dest;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// PointToWindow
        /// 
        /// <summary>
        /// AssociatedObject をベースにした座標をアプリケーションのメイン
        /// ウィンドウをベースにした座標へ変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Point PointToWindow(Point pt)
        {
            var tmp = AssociatedObject.PointToScreen(pt);
            return Application.Current.MainWindow.PointFromScreen(tmp);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// PointToWindow
        /// 
        /// <summary>
        /// アプリケーションのメインウィンドウをベースにした座標を
        /// AssociatedObject をベースにした座標へ変換します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Point PointFromWindow(Point pt)
        {
            var tmp = Application.Current.MainWindow.PointToScreen(pt);
            return AssociatedObject.PointFromScreen(tmp);
        }

        #endregion

        #region Other methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetItem
        ///
        /// <summary>
        /// マウスカーソルのある項目を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private object GetItem(Point current)
        {
            var result = VisualTreeHelper.HitTest(AssociatedObject, current);
            if (result == null) return null;

            var item = VisualHelper.FindVisualParent<ListViewItem>(result.VisualHit);
            return (item != null) ? item.Content : null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetItemIndex
        ///
        /// <summary>
        /// マウスカーソルのある項目のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private int GetItemIndex(Point current)
        {
            var result = VisualTreeHelper.HitTest(AssociatedObject, current);
            if (result == null) return -1;

            var item = VisualHelper.FindVisualParent<ListViewItem>(result.VisualHit);
            return (item != null) ? AssociatedObject.Items.IndexOf(item.Content) : -1;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetTargetItemIndex
        ///
        /// <summary>
        /// マウスカーソルのある項目のインデックスを取得します。
        /// マウスカーソルが項目間にポイントされている場合は、直近に存在
        /// する項目のインデックスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private int GetTargetItemIndex(Point current)
        {
            var dest = GetItemIndex(current);
            if (dest == -1)
            {
                var rect = GetItemBounds(AssociatedObject.Items[0]);
                if (AssociatedObject.ActualWidth - current.X < rect.Width)
                {
                    current.X = AssociatedObject.ActualWidth - rect.Width;
                }
                else
                {
                    var lvi = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(_source) as ListViewItem;
                    if (lvi != null) current.X -= lvi.Margin.Right;
                }
                dest = GetItemIndex(current);
            }
            return dest;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// DecideDragAction
        ///
        /// <summary>
        /// 行われているドラッグドロップが、CopyなのかMoveなのかを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        DragDropEffects DecideDragAction(object sender, DragEventArgs e)
        {
            var additem = e.Data.GetData(DataFormats.Serializable) as CubePdf.Data.IPage;
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                if (sender != e.Source)
                {
                    return DragDropEffects.Copy;
                }
                else
                {
                    return DragDropEffects.Move;
                }
            }
            else
            {
                return DragDropEffects.None;
            }
        }

        #endregion

        #region Attached/Detach methods

        /* ----------------------------------------------------------------- */
        /// OnAttached
        /* ----------------------------------------------------------------- */
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.DragEnter += OnDragEnter;

            var panel = VisualHelper.FindVisualParent<Grid>(AssociatedObject);
            if (panel != null) panel.Children.Add(_canvas);

            base.OnAttached();
        }

        /* ----------------------------------------------------------------- */
        /// OnDetaching
        /* ----------------------------------------------------------------- */
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.DragEnter -= OnDragEnter;

            var panel = VisualHelper.FindVisualParent<Grid>(AssociatedObject);
            if (panel != null) panel.Children.Remove(_canvas);

            base.OnDetaching();
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                typeof(ListViewModel),
                typeof(ListViewDragBehavior),
                new PropertyMetadata(null));

        #endregion

        #region Variables
        private bool _ondrag = false;
        private Canvas _canvas = new Canvas();
        private Border _rect = new Border();
        private Point _position = new Point();
        private int _source = -1;
        private int _target = -1;
        #endregion

        #region Constant variables
        private static readonly int SCROLLBAR_WIDTH = 18;
        private static readonly int CURSOR_ADJUSTMENT = 7;
        #endregion
    }
}
