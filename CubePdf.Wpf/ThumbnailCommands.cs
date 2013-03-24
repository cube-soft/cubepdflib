using System;
using System.Windows;
using System.Windows.Input;

namespace CubePdf.Wpf
{
    public static class ThumbnailCommands
    {
        #region Available Commands
        public static ICommand Add { get { return _add; } }
        public static ICommand Insert { get { return _insert; } }
        public static ICommand Remove { get { return _remove; } }
        public static ICommand Extract { get { return _extract; } }
        public static ICommand Move { get { return _move; } }
        public static ICommand Rotate { get { return _rotate; } }
        #endregion

        #region Static variables
        private static readonly ICommand _add = new RoutedCommand("Add", typeof(ThumbnailViewModel));
        private static readonly ICommand _insert = new RoutedCommand("Insert", typeof(ThumbnailViewModel));
        private static readonly ICommand _remove = new RoutedCommand("Remove", typeof(ThumbnailViewModel));
        private static readonly ICommand _extract = new RoutedCommand("Extract", typeof(ThumbnailViewModel));
        private static readonly ICommand _move = new RoutedCommand("Move", typeof(ThumbnailViewModel));
        private static readonly ICommand _rotate = new RoutedCommand("Rotate", typeof(ThumbnailViewModel));
        #endregion
    }
}
