using System;
using System.Windows;

namespace CubePdf.Wpf
{
    public class ImageView : System.Windows.Controls.ViewBase
    {
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        protected override object DefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "ImageView"); }
        }

        protected override object ItemContainerDefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "ImageViewItem"); }
        }

        #region Dependency Properties
        public static readonly DependencyProperty ItemWidthProperty = System.Windows.Controls.WrapPanel.ItemWidthProperty.AddOwner(typeof(ImageView));
        #endregion
    }
}
