using System;
using System.Windows;

namespace CubePdf.Wpf
{
    public class ImageView : System.Windows.Controls.ViewBase
    {
        protected override object DefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "ImageView"); }
        }

        protected override object ItemContainerDefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "ImageViewItem"); }
        }
    }
}
