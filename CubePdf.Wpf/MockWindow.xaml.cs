using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CubePdf.Wpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MockWindow : Window
    {
        public MockWindow()
        {
            InitializeComponent();
            _engine.ImageCreated += new CubePdf.Drawing.ImageEventHandler(BitmapEngine_ImageCreated);
            this.MainListView.ItemsSource = _source;
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            // NOTE: bool? て何？
            // TODO: キャンセルボタンが押された場合、直ちに return する。
            dialog.Filter = "PDF ファイル(*.pdf)|*.pdf|すべてのファイル(*.*)|*.*";
            dialog.ShowDialog();

            // TODO: Generic.xaml で定義している ListView の各項目の幅を取得する方法を調査して置き換える。
            var bound = new System.Drawing.Size(256, 256);

            _engine.Open(dialog.FileName);
            if (_source.Count > 0) _source.Clear();
            foreach (var page in _engine.Pages.Values)
            {
                var power  = this.CalculatePower(page, bound);
                var width  = (int)(page.ViewSize.Width * power);
                var height = (int)(page.ViewSize.Height * power);
                var dummy  = this.ToBitmapImage(new System.Drawing.Bitmap(width, height));
                _source.Add(dummy);
                _engine.CreateImageAsync(page.PageNumber, power);
            }
            this.MainListView.Items.Refresh();
        }

        // 非同期で作成していた PDF の各ページの画像が生成された時に実行されるハンドラ
        // TODO: エラー処理
        private void BitmapEngine_ImageCreated(object sender, CubePdf.Drawing.ImageEventArgs e)
        {
            _source[e.Page.PageNumber - 1] = this.ToBitmapImage(e.Image);
            this.MainListView.Items.Refresh();
        }

        #region Private Methods

        // WPF 用のイメージに変換する。
        private BitmapImage ToBitmapImage(System.Drawing.Image src)
        {
            var stream = new System.IO.MemoryStream();
            src.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var dest = new BitmapImage();
            dest.BeginInit();
            dest.StreamSource = new System.IO.MemoryStream(stream.ToArray());
            dest.EndInit();
            return dest;
        }

        // サイズから生成する画像の倍率を計算する。
        private double CalculatePower(CubePdf.Data.IReadOnlyPage page, System.Drawing.Size bound)
        {
            var horizontal = bound.Width / (double)page.ViewSize.Width;
            var vertical = bound.Height / (double)page.ViewSize.Height;
            return (horizontal < vertical) ? horizontal : vertical;
        }

        #endregion

        #region Variables

        private CubePdf.Drawing.BitmapEngine _engine = new Drawing.BitmapEngine();

        // NOTE: ObservableCollection<> は、コレクションに対して、追加、削除、のような変更が
        // あった際にイベントを通知する機能を持っている。将来的には、List<> ではなく
        // こちらを使用するかもしれない。
        // private ObservableCollection<BitmapSource> _source = new ObservableCollection<BitmapSource>();
        private List<BitmapSource> _source = new List<BitmapSource>();

        #endregion
    }
}
