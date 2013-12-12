/* ------------------------------------------------------------------------- */
///
/// ImageContainer.cs
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
using System.ComponentModel;
using System.Drawing;

namespace CubePdf.Drawing
{
    /* --------------------------------------------------------------------- */
    ///
    /// ImageContainer
    /// 
    /// <summary>
    /// PDF ファイルの各ページに対応するイメージを管理するために使用される
    /// クラスです。BitmapEngine オブジェクトで、各ページに対応するイメージを
    /// 非同期に生成する事があるため、各イメージの生成状況を管理する際など
    /// で使用します。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [Serializable]
    public class ImageContainer : IDisposable, INotifyPropertyChanged
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// ImageContainer (constructor)
        /// 
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageContainer() { }

        /* ----------------------------------------------------------------- */
        ///
        /// ImageContainer (constructor)
        /// 
        /// <summary>
        /// イメージ、およびイメージの状態を指定してオブジェクトを初期化
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageContainer(Image image, ImageStatus status)
        {
            UpdateImage(image, status);
        }

        /* ----------------------------------------------------------------- */
        /// ~ImageContainer (destructor)
        /* ----------------------------------------------------------------- */
        ~ImageContainer()
        {
            Dispose(false);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        /// 
        /// <summary>
        /// IDisposable で定義されているメソッドの実装部分です。実際に必要な
        /// 処理は Dispose(bool) メソッドに記述して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        /// 
        /// <summary>
        /// 終了時に必要な処理を記述します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                if (_image != null) _image.Dispose();
                _image = null;
                _status = ImageStatus.None;
            }
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Status
        /// 
        /// <summary>
        /// 格納されている Image プロパティの現在の状態を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageStatus Status
        {
            get { return _status; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Image
        /// 
        /// <summary>
        /// イメージを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Image Image
        {
            get { return _image; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Text
        /// 
        /// <summary>
        /// 格納されている Image プロパティに関する情報等を表すテキストを
        /// 取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateImage
        ///
        /// <summary>
        /// Image オブジェクトを更新します。古い Image オブジェクトは
        /// Dispose されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void UpdateImage(Image image, ImageStatus status)
        {
            _status = status;
            if (_image != null) _image.Dispose();
            _image = image;
            OnPropertyChanged("Image");
        }

        /* ----------------------------------------------------------------- */
        ///
        /// DeleteImage
        /// 
        /// <summary>
        /// 現在の Image オブジェクトを削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void DeleteImage()
        {
            if (_image != null) _image.Dispose();
            _image = null;
            _status = ImageStatus.None;
            OnPropertyChanged("Image");
        }

        #endregion

        #region Implementation for INotifyPropertyChanged methods

        /* ----------------------------------------------------------------- */
        /// PropertyChanged
        /* ----------------------------------------------------------------- */
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged as PropertyChangedEventHandler;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Variables
        private ImageStatus _status = ImageStatus.None;
        private Image _image = null;
        private string _text = string.Empty;
        private bool _disposed = false;
        #endregion
    }
}
