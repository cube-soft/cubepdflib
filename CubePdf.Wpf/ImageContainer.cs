using System;
using System.Windows.Media;

namespace CubePdf
{
    internal enum ImageStatus
    {
        None,
        Dummy,
        Cached,
    }

    internal class ImageContainer
    {
        public ImageStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private ImageStatus _status = ImageStatus.None;
        private ImageSource _image = null;
    }
}
