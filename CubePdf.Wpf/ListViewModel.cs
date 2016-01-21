/* ------------------------------------------------------------------------- */
///
/// ListViewModel.cs
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
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using Cube;
using CubePdf.Data;
using CubePdf.Data.Extensions;

namespace CubePdf.Wpf
{
    /* --------------------------------------------------------------------- */
    ///
    /// ListViewModel
    /// 
    /// <summary>
    /// ListView で表示される PDF ファイルの各ページの情報、およびイメージ
    /// データ等を管理するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [Serializable]
    public class ListViewModel : IDocumentReader, IDocumentWriter,
        IItemsProvider<CubePdf.Drawing.ImageContainer>, INotifyPropertyChanged, IDisposable
    {
        #region Initialization and Termination

        /* ----------------------------------------------------------------- */
        ///
        /// ListViewModel (constructor)
        ///
        /// <summary>
        /// 既定の値でオブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListViewModel()
        {
            _images  = new ListProxy<CubePdf.Drawing.ImageContainer>(this);
            _created = new IndexTable(_images);
            _creator.Creating += ImageCreator_Creating;
            _creator.Created  += ImageCreator_Created; 
        }

        /* ----------------------------------------------------------------- */
        ///
        /// destructor
        /// 
        /// <summary>
        /// NOTE: クラスで必要な終了処理は、デストラクタではなく Dispose(bool)
        /// メソッドに記述して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ~ListViewModel()
        {
            Dispose(false);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        /// 
        /// <summary>
        /// NOTE: IDisposable で定義されているメソッドの実装部分です。実際に
        /// 必要な処理は Dispose(bool) メソッドに記述して下さい。
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
                CloseDocument();
                DeleteBackup();
            }
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// FilePath
        /// 
        /// <summary>
        /// ベースとなる PDF ファイル（Open メソッドで指定されたファイル）の
        /// パスを取得します（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FilePath { get; private set; } = string.Empty;

        /* ----------------------------------------------------------------- */
        ///
        /// Metadata
        /// 
        /// <summary>
        /// PDF ファイルの文書プロパティを取得、または設定します
        /// （IDocumentWriter から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Metadata Metadata
        {
            get { return _metadata; }
            set
            {
                UpdateHistory(ListViewCommands.Metadata, _metadata);
                _metadata = value;
                if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Encryption
        /// 
        /// <summary>
        /// PDF ファイルのセキュリティに関する情報を取得、または設定します
        /// （IDocumentWriter から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.Encryption Encryption
        {
            get { return _encrypt; }
            set
            {
                UpdateHistory(ListViewCommands.Encryption, _encrypt);
                _encrypt = value;
                if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionStatus
        /// 
        /// <summary>
        /// 暗号化されている PDF ファイルへのアクセス（許可）状態を
        /// 取得します（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Data.EncryptionStatus EncryptionStatus { get; private set; } = EncryptionStatus.NotEncrypted;

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF の各ページ情報へアクセスするための反復子を取得します
        /// （IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IReadOnlyCollection<PageBase> Pages
        {
            get { return _pages; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Pages
        /// 
        /// <summary>
        /// PDF ファイルの各ページ情報を取得、または設定します
        /// （IDocumentWriter から継承されます）。ListViewModel クラスでは、
        /// IDocumentReader.Pages プロパティが優先されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        ICollection<PageBase> IDocumentWriter.Pages
        {
            get { return _pages; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IsModified
        /// 
        /// <summary>
        /// 現在、開かれている PDF ファイルに対して何らかの変更が加えられた
        /// かどうかを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool IsModified
        {
            get { return _modified || History.Count > 0; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MaxHistoryCount
        /// 
        /// <summary>
        /// 記録可能な履歴の最大値を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int MaxHistoryCount
        {
            get { return _maxundo; }
            set
            {
                if (_maxundo != value)
                {
                    _maxundo = value;
                    OnPropertyChanged("HistoryLimit");
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// History
        /// 
        /// <summary>
        /// これまでに実行した処理の履歴を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ObservableCollection<CommandElement> History { get; } = new ObservableCollection<CommandElement>();

        /* ----------------------------------------------------------------- */
        ///
        /// UndoHistory
        /// 
        /// <summary>
        /// 直前に実行した Undo 処理の履歴を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ObservableCollection<CommandElement> UndoHistory { get; } = new ObservableCollection<CommandElement>();

        /* ----------------------------------------------------------------- */
        ///
        /// ViewSize
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの幅/高さの基準となる値を取得、
        /// または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ViewSize
        {
            get { return _viewsize; }
            set
            {
                if (_viewsize != value)
                {
                    _viewsize = value;
                    OnPropertyChanged("ItemWidth");
                    OnPropertyChanged("ItemHeight");
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemWidth
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの幅の最大値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ItemWidth
        {
            get
            {
                if (_maxwidth > _maxheight) return ViewSize;
                else return (int)(ViewSize * (_maxwidth / (double)_maxheight));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemHeight
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの高さの最大値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ItemHeight
        {
            get
            {
                if (_maxheight > _maxwidth) return ViewSize;
                else return (int)(ViewSize * (_maxheight / (double)_maxwidth));
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemVisibility
        /// 
        /// <summary>
        /// ListView で表示されるサムネイルの表示方法を取得、または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ListViewItemVisibility ItemVisibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged("ItemVisibility");
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Items
        /// 
        /// <summary>
        /// ListView に表示するサムネイル一覧を取得します。Items 中の
        /// 各サムネイルは必ずしも表示可能なデータとなっているとは
        /// 限りませんが、ListView で問題なく表示されるように実装されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IListProxy<CubePdf.Drawing.ImageContainer> Items
        {
            get { return _images; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// View
        /// 
        /// <summary>
        /// このオブジェクトで制御する ListView オブジェクトを取得、または
        /// 設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public System.Windows.Controls.ListView View { get; set; } = null;

        /* ----------------------------------------------------------------- */
        ///
        /// BackupFolder
        /// 
        /// <summary>
        /// 上書き保存を行う際、上書き前のファイルのバックアップを保存する
        /// フォルダを取得、または設定します。バックアップファイルを作成
        /// しない場合は空文字（または、BackupDays に 0)を設定して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string BackupFolder { get; set; } = string.Empty;

        /* ----------------------------------------------------------------- */
        ///
        /// BackupDays
        /// 
        /// <summary>
        /// バックアップファイルを残す日数を取得、または設定します。
        /// バックアップファイルを作成しない場合は 0 を設定して下さい。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int BackupDays { get; set; } = 0;

        #endregion

        #region Public methods

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// 引数に指定された IDocumentReader オブジェクトからページ情報を
        /// 読み込んで、ListView へ表示可能な状態にします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(CubePdf.Data.IDocumentReader reader)
        {
            if (_pages.Count > 0) CloseDocument();
            OpenDocument(reader);
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルを開き、画面に表示可能な状態にする
        /// ための準備を行います（IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(string path, string password = "")
        {
            if (_pages.Count > 0) CloseDocument();
            using (var reader = new CubePdf.Editing.DocumentReader(path, password))
            {
                OpenDocument(reader);
            }
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        /// 
        /// <summary>
        /// 現在開いている PDF ファイルを閉じます（IDocumentReader から
        /// 継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Close()
        {
            CloseDocument();
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルに保存します（IDocumentWriter から
        /// 継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save(string path)
        {
            var binder = new CubePdf.Editing.PageBinder();
            SaveDocument(path, binder);
            ReOpenDocument(path, binder);
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        /// 
        /// <summary>
        /// 現在のページ構成でファイルを上書き保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save()
        {
            Save(FilePath);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SaveOnClose
        /// 
        /// <summary>
        /// ファイルを閉じる際に、現在の状態で保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void SaveOnClose(string path = "")
        {
            if (String.IsNullOrEmpty(path)) path = FilePath;
            var binder = new CubePdf.Editing.PageBinder();
            SaveDocument(path, binder);
            CloseDocument();
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトに対応する PDF ページをページ末尾に
        /// 追加します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Add(PageBase item) { Insert(_pages.Count, item); }
        public void Add(CubePdf.Data.IDocumentReader reader) { Insert(_pages.Count, reader); }
        public void Add(string path, string password = "") { Insert(_pages.Count, path, password); }

        /* ----------------------------------------------------------------- */
        ///
        /// InsertImage
        /// 
        /// <summary>
        /// 引数に指定された画像ファイルを index の位置に挿入します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void InsertImage(int index, string path)
        {
            using (var image = Bitmap.FromFile(path))
            {
                try
                {
                    BeginCommand();

                    var guid = image.FrameDimensionsList[0];
                    var dim = new System.Drawing.Imaging.FrameDimension(guid);
                    for (var i = 0; i < image.GetFrameCount(dim); ++i)
                    {
                        image.SelectActiveFrame(dim, i);
                        Insert(index + i, new ImagePage
                        {
                            FilePath = path,
                            PageNumber = i + 1,
                            OriginalSize = new Size(image.Width, image.Height),
                            Rotation = 0
                        });
                    }
                }
                finally { EndCommand(); }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された PDF ページオブジェクトを index の位置に挿入
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert(int index, PageBase item)
        {
            lock (_pages)
            lock (_images)
            {
                _creator.Remove(index, _pages.Count);
                _pages.Insert(index, item);
                UpdateImageSize(item);
                _images.Insert(index, new Drawing.ImageContainer());
                _created.ItemInserted(index);
                UpdateImageText(index);
                UpdateHistory(ListViewCommands.Insert, new KeyValuePair<int, PageBase>(index, item));
            }
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された IDocumentReader オブジェクトの各ページを index
        /// の位置に挿入します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert(int index, CubePdf.Data.IDocumentReader reader)
        {
            try
            {
                BeginCommand();
                InsertDocument(index, reader);
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページを index の位置に挿入
        /// します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Insert(int index, string path, string password = "")
        {
            using (var reader = new CubePdf.Editing.DocumentReader(path, password))
            {
                Insert(index, reader);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Extract
        /// 
        /// <summary>
        /// 引数に指定されたオブジェクトい対応する各 PDF ページを新しい
        /// PDF ファイルとして path に保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Extract(IList items, string path)
        {
            var binder = new CubePdf.Editing.PageBinder();
            foreach (var item in items)
            {
                var index = IndexOf(item);
                if (index >= 0 && index < _pages.Count) binder.Pages.Add(_pages[index]);
            }

            var tmp = System.IO.Path.GetTempFileName();
            binder.Save(tmp);
            CubePdf.Misc.File.Move(tmp, path, true);
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractImage
        /// 
        /// <summary>
        /// 引数に指定された PDF ファイルの各ページに含まれる画像を
        /// direcotry 下に保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void ExtractImage(IList items, string directory)
        {
            var worker = new BackgroundImageExtractor();
            worker.Completed += (s, e) => OnRunCompleted(new EventArgs());
            worker.ProgressChanged += (s, e) =>
            {
                var page = e.Value.Page;
                var basename = System.IO.Path.GetFileNameWithoutExtension(page.FilePath);
                var count = e.Value.Images.Count;
                for (var i = 0; i < count; ++i)
                {
                    var dest = Unique(directory, basename, page.PageNumber, i, count);
                    e.Value.Images[i].Save(dest, System.Drawing.Imaging.ImageFormat.Png);
                }
            };

            foreach (var item in items)
            {
                var index = IndexOf(item);
                if (index >= 0 && index < _pages.Count) worker.Pages.Add(_pages[index]);
            }
            worker.RunAsync();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Split
        /// 
        /// <summary>
        /// 引数に指定されたサムネイルに対応する PDF ページを direcotry 下に
        /// 1 ページずつ別ファイルとして保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Split(IList items, string directory)
        {
            foreach (var obj in items) SaveDocument(directory, IndexOf(obj));
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RemoveAt
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイルに相当する
        /// PDF ページを削除します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _pages.Count) return;

            lock (_pages)
            lock (_images)
            {
                var page = _pages[index];
                _pages.RemoveAt(index);
                var image = _images.RawAt(index);
                _images.RemoveAt(index);
                if (image != null) image.Dispose();
                _created.ItemRemoved(index);
                _creator.Remove(index, _pages.Count);
                UpdateImageText(index);
                UpdateHistory(ListViewCommands.Remove, new KeyValuePair<int, PageBase>(index, page));
            }
            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Move
        /// 
        /// <summary>
        /// ListView に表示されている oldindex 番目のサムネイルに相当する
        /// PDF ページを同 newindex へ移動させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Move(int oldindex, int newindex)
        {
            if (oldindex < 0 || oldindex >= _pages.Count ||
                newindex < 0 || newindex >= _pages.Count || oldindex == newindex) return;

            lock (_pages)
            lock (_images)
            {
                var page = _pages[oldindex];
                _pages.RemoveAt(oldindex);
                _pages.Insert(newindex, page);
                _images.Move(oldindex, newindex);

                if (oldindex <= newindex) UpdateImageText(oldindex, newindex);
                else UpdateImageText(newindex, oldindex);
                UpdateHistory(ListViewCommands.Move, new KeyValuePair<int, int>(oldindex, newindex));
            }

            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// RotateAt
        /// 
        /// <summary>
        /// ListView に表示されている index 番目のサムネイルに相当する
        /// PDF ページを degree 度回転させます。角度は、現在表示されている
        /// 画像に対する相対度数で指定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void RotateAt(int index, int degree)
        {
            if (index < 0 || index >= _pages.Count) return;

            lock (_pages)
            lock (_images)
            {
                var page = _pages[index].Copy();
                page.Rotation += degree;
                if (page.Rotation < 0) page.Rotation += 360;
                if (page.Rotation >= 360) page.Rotation -= 360;
                _pages[index] = page;
                _images.RawAt(index).DeleteImage();

                // NOTE: 非同期で内容（イメージ）の差し替えを行うと、GUI への
                // 反応が遅れる場合があるので、暫定的に Remove&Insert を行っている。
                var image = _images.RawAt(index);
                var selected = (View != null) ? View.SelectedItems.Contains(image) : false;
                _images.RemoveAt(index);
                _images.Insert(index, image);
                if (selected) View.SelectedItems.Add(image);

                UpdateHistory(ListViewCommands.Rotate, new KeyValuePair<int, int>(index, degree));
            }

            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// BeginCommand
        /// 
        /// <summary>
        /// 一連の処理が始まる事を表します。主に Undo の際の処理粒度を調整
        /// する目的で使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void BeginCommand() { _status = CommandStatus.Begin; }

        /* ----------------------------------------------------------------- */
        ///
        /// EndCommand
        /// 
        /// <summary>
        /// 一連の処理が終わる事を表します。主に Undo の際の処理粒度を調整
        /// する目的で使用されます。このメソッドは、終了時に必ず
        /// RunCompleted イベントを発生させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void EndCommand()
        {
            _status = CommandStatus.End;
            OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        /// 
        /// <summary>
        /// 直前の操作を取り消します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Undo()
        {
            if (History.Count == 0) return;

            try
            {
                _undostatus = UndoStatus.Undo;
                var element = History[0];
                History.Remove(element);
                if (element.Command == ListViewCommands.Insert) UndoInsert(element.Parameters);
                else if (element.Command == ListViewCommands.Remove) UndoRemove(element.Parameters);
                else if (element.Command == ListViewCommands.Move) UndoMove(element.Parameters);
                else if (element.Command == ListViewCommands.Rotate) UndoRotate(element.Parameters);
                else if (element.Command == ListViewCommands.Metadata) UndoMetadata(element.Parameters);
                else if (element.Command == ListViewCommands.Encryption) UndoEncryption(element.Parameters);
            }
            finally { _undostatus = UndoStatus.Normal; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Redo
        /// 
        /// <summary>
        /// 取り消した操作を再実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Redo()
        {
            if (UndoHistory.Count == 0) return;

            try
            {
                _undostatus = UndoStatus.Redo;
                var element = UndoHistory[0];
                UndoHistory.Remove(element);
                if (element.Command == ListViewCommands.Insert) UndoInsert(element.Parameters);
                else if (element.Command == ListViewCommands.Remove) UndoRemove(element.Parameters);
                else if (element.Command == ListViewCommands.Move) UndoMove(element.Parameters);
                else if (element.Command == ListViewCommands.Rotate) UndoRotate(element.Parameters);
                else if (element.Command == ListViewCommands.Metadata) UndoMetadata(element.Parameters);
                else if (element.Command == ListViewCommands.Encryption) UndoEncryption(element.Parameters);
            }
            finally { _undostatus = UndoStatus.Normal; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        /// 
        /// <summary>
        /// 初期状態にリセットします（IDocumentWriter から継承されます）。
        /// 表示に関わるオブジェクトがクリアされます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Reset()
        {
            _creator.Clear();

            lock (_images) ClearImage();

            _maxwidth = 0;
            _maxheight = 0;

            OnPropertyChanged("ItemWidth");
            OnPropertyChanged("ItemHeight");

            if (_status == CommandStatus.End) OnRunCompleted(new EventArgs());
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Refresh
        /// 
        /// <summary>
        /// 画面に表示されている各サムネイル項目のうち、生成されていない
        /// ものを再度リクエストキューに登録します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Refresh()
        {
            var range = GetVisibleRange();
            if (range.Key < 0 || range.Value >= _pages.Count) return;
            for (int i = range.Key; i <= range.Value; ++i) UpdateImage(i);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPage
        /// 
        /// <summary>
        /// 指定されたページ番号に対応するページ情報を取得します
        /// （IDocumentReader から継承されます）。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PageBase GetPage(int pagenum)
        {
            return _pages[pagenum - 1];
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetImage
        /// 
        /// <summary>
        /// ListView で表示されているサムネイルに対応するプレビュー用の
        /// イメージを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Image GetImage(int index, Size bound)
        {
            if (index < 0 || index >= _pages.Count) return null;

            var page = _pages[index].Copy();
            page.Power = GetPower(page, bound);
            return _creator.Create(page);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IndexOf
        /// 
        /// <summary>
        /// ListView で表示されている項目、またはページ情報に対応する
        /// インデックスを取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// TODO: 将来的にこのメソッドは削除したい。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public int IndexOf(object item) { return IndexOf(item as CubePdf.Drawing.ImageContainer); }
        public int IndexOf(CubePdf.Drawing.ImageContainer item) { return _images.IndexOf(item); }

        #endregion

        #region Public Events

        /* ----------------------------------------------------------------- */
        ///
        /// RunCompleted
        /// 
        /// <summary>
        /// ListView クラスの各種メソッドの処理が終了した時に発生する
        /// イベントです。RunCompleted イベントが発生するタイミングは、
        /// BeginCommand メソッドを実行した場合は EndCommand メソッドを
        /// 実行した直後、それ以外は各メソッドを実行の実行が終了した時と
        /// なります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event EventHandler RunCompleted;
        protected virtual void OnRunCompleted(EventArgs e)
        {
            if (RunCompleted != null) RunCompleted(this, e);
        }

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// IItemsProvider(Image)
        /// 
        /// <summary>
        /// IItemsProvider インターフェースの実装を行います。
        /// ListViewModel クラスでは仮想化は行わないので、全てのメソッドで
        /// NotImplementationException が送出されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        #region Implementations for IItemsProvider<Image>

        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItemsCount
        /// 
        /// <summary>
        /// Items の項目数を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int ProvideItemsCount()
        {
            return _pages.Count;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ProvideItem
        /// 
        /// <summary>
        /// 指定されたインデックスに対応する項目を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public CubePdf.Drawing.ImageContainer ProvideItem(int index)
        {
            if (index < 0 || index >= _images.RawCount) return null;
            UpdateImageSize(_pages[index]);
            
            var range = GetVisibleRange();
            if (index < range.Key || index > range.Value) return _images.RawAt(index);
            UpdateImage(index);
            return _images.RawAt(index);
        }

        #endregion

        #region Implementation for INotifyPropertyChanged

        /* ----------------------------------------------------------------- */
        ///
        /// PropertyChanged
        ///
        /// <summary>
        /// ListViewModel のプロパティに変更があると発生するイベントです。
        /// </summary>
        ///
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

        #region Event handlers for BitmapEngine

        /* ----------------------------------------------------------------- */
        ///
        /// ImageCreator_Creating
        /// 
        /// <summary>
        /// サムネイルを作成する直前に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ImageCreator_Creating(object sender, DataCancelEventArgs<ImageEntry> e)
        {
            var index = e.Value.Index;
            var page  = e.Value.Page;

            if (_visibility == ListViewItemVisibility.Minimum ||
                index < 0 || index >= _pages.Count)
            {
                e.Cancel = true;
                return;
            }

            var range = GetVisibleRange();
            if (index < range.Key || index > range.Value || !page.Equals(_pages[index]) ||
                _images.RawAt(index).Status == Drawing.ImageStatus.Created)
            {
                if (index < range.Key || index > range.Value) _images.RawAt(index).DeleteImage();
                e.Cancel = true;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ImageCreator_Created
        /// 
        /// <summary>
        /// サムネイルが作成された時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ImageCreator_Created(object sender, DataEventArgs<ImageEntry> e)
        {
            var index = e.Value.Index;
            var page  = e.Value.Page;
            var image = e.Value.Image;

            if (image == null) return;
            if (index >= 0 && index < _images.RawCount) _images.RawAt(index).DeleteImage();

            var range = GetVisibleRange();
            if (index >= range.Key && index <= range.Value && page.Equals(_pages[index]))
            {
                lock (_images)
                {
                    _images.RawAt(index).UpdateImage(image, Drawing.ImageStatus.Created);
                    _created.Capacity = (int)((range.Value - range.Key + 1) * 1.5);
                    _created.Update(index);
                }
            }
            else image.Dispose();
        }

        #endregion

        #region Private methods for data access and converting

        /* ----------------------------------------------------------------- */
        ///
        /// GetSize
        /// 
        /// <summary>
        /// 引数に指定されたページオブジェクトの縦横比を保ったまま、
        /// 表示用の長方形に収まる最大サイズを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Size GetSize(PageBase page)
        {
            if (page.ViewSize().Width > page.ViewSize().Height)
            {
                var width  = ItemWidth;
                var height = page.ViewSize().Height * (width / (double)page.ViewSize().Width);
                return new Size(width, (int)height);
            }
            else
            {
                var height = ItemHeight;
                var width  = page.ViewSize().Width * (height / (double)page.ViewSize().Height);
                return new Size((int)width, height);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetVisibleRange
        /// 
        /// <summary>
        /// 実際に画面に表示される項目の範囲を取得します。
        /// </summary>
        /// 
        /// <remarks>
        /// TODO:
        /// - margin の値は、テストを基に現状でもっともずれの少ない算出
        ///   方法を用いている。実際には、項目の余白、および枠線の値を
        ///   基に算出する必要があると思われる。
        /// </remarks>
        /// 
        /* ----------------------------------------------------------------- */
        private KeyValuePair<int, int> GetVisibleRange()
        {
            var all = new KeyValuePair<int, int>(0, _pages.Count - 1);
            if (View == null) return all;

            try
            {
                var scroll = VisualHelper.FindVisualChild<System.Windows.Controls.ScrollViewer>(View);
                if (scroll == null) return all;
                
                var width  = (double)Math.Max(ItemWidth, 1);
                var height = (double)Math.Max(ItemHeight, 1);
                var margin = 1.5 * width / 100.0; // NOTE: empirically
                var column = (int)(View.ActualWidth / (width + margin));
                var row    = (int)(View.ActualHeight / (height + margin));
                var index  = (int)(scroll.VerticalOffset / height) * column;
                if (index < 0 || index > _pages.Count) return all;

                var dest = new KeyValuePair<int, int>(index, Math.Min(index + column * (row + 2), _pages.Count - 1));
                Debug.WriteLine(string.Format("col:{0}({1}/{2}), row:{3}({4}/{5}) => [{6}-{7}]",
                    column, View.ActualWidth,  width  + margin,
                    row,    View.ActualHeight, height + margin,
                    dest.Key, dest.Value));
                return dest;
            }
            catch (Exception err)
            {
                Trace.WriteLine(err.ToString());
                return all;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPower
        /// 
        /// <summary>
        /// Size(ItemWidth, ItemHeight) の長方形に収まるような最大倍率を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private double GetPower(PageBase page)
        {
            return GetPower(page, new Size(ItemWidth, ItemHeight));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetPower
        /// 
        /// <summary>
        /// bound に収まるような最大倍率を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private double GetPower(PageBase page, Size bound)
        {
            var horizontal = bound.Width / (double)page.ViewSize().Width;
            var vertical = bound.Height / (double)page.ViewSize().Height;
            return (horizontal < vertical) ? horizontal : vertical;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetDummyImage
        /// 
        /// <summary>
        /// ListView に本来表示される画像と縦横比の近いダミー画像を取得
        /// します。生成された画像の縦横比は、若干ずれる事があります。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Image GetDummyImage(PageBase page)
        {
            var tolerance = 0.1f;
            var size = GetSize(page);            
            var w = size.Width / (double)size.Height;
            var h = 1;
            while (Math.Abs(w - Math.Round(w)) > tolerance)
            {
                w *= (h + 1) / (double)h;
                h++;
            }
            return new Bitmap((int)Math.Round(w), h);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetLoadingImage
        /// 
        /// <summary>
        /// ListView に本来表示される画像と縦横比の等しいローディング中を
        /// 表す画像を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Image GetLoadingImage(PageBase page)
        {
            var size  = GetSize(page);
            var image = (size.Width > Properties.Resources.LoadingLarge.Width) ? Properties.Resources.LoadingLarge :
                        (size.Width > Properties.Resources.LoadingMiddle.Width) ? Properties.Resources.LoadingMiddle :
                Properties.Resources.LoadingSmall;

            var x = Math.Max((size.Width - image.Width) / 2.0, 0);
            var y = Math.Max((size.Height - image.Height) / 2.0, 0);
            var pos = new Point((int)x, (int)y);
            var dest = new Bitmap(size.Width, size.Height);
            var graphic = Graphics.FromImage(dest);
            graphic.DrawImage(image, pos);
            graphic.Dispose();
            return dest;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetCommandText
        /// 
        /// <summary>
        /// 各コマンドを説明するテキストを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetCommandText(CubePdf.Wpf.CommandElement element)
        {
            if (element.Command == ListViewCommands.Insert) return Properties.Resources.InsertText;
            else if (element.Command == ListViewCommands.Remove) return Properties.Resources.RemoveText;
            else if (element.Command == ListViewCommands.Move) return Properties.Resources.MoveText;
            else if (element.Command == ListViewCommands.Rotate) return Properties.Resources.RotateText;
            else if (element.Command == ListViewCommands.Metadata) return Properties.Resources.MetadataText;
            else if (element.Command == ListViewCommands.Encryption) return Properties.Resources.EncryptionText;
            return string.Empty;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Unique
        /// 
        /// <summary>
        /// 一意のパス名を取得します。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        private string Unique(string folder, string basename, int pagenum, int index, int count)
        {
            var digit = string.Format("D{0}", count.ToString("D").Length);
            for (var i = 1; i < 1000; ++i)
            {
                var filename = (i == 1) ?
                               string.Format("{0}-{1}-{2}.png", basename, pagenum, index.ToString(digit)) :
                               string.Format("{0}-{1}-{2} ({3}).png", basename, pagenum, index.ToString(digit), i);
                var dest = System.IO.Path.Combine(folder, filename);
                if (!System.IO.File.Exists(dest)) return dest;
            }

            return System.IO.Path.Combine(folder, System.IO.Path.GetRandomFileName());
        }

        #endregion

        #region Private methods for documents

        /* ----------------------------------------------------------------- */
        ///
        /// OpenDocument
        /// 
        /// <summary>
        /// 引数に指定された IDocumentReader から必要な情報をコピーして
        /// ファイルを開きます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void OpenDocument(CubePdf.Data.IDocumentReader reader)
        {

            // Properties for IDocumentReader
            FilePath = reader.FilePath;
            _metadata = reader.Metadata;
            _encrypt = reader.Encryption;
            EncryptionStatus = reader.EncryptionStatus;
            _pages.Capacity = reader.Pages.Count + 1;

            InsertDocument(_pages.Count, reader);

            // Properties for others
            History.Clear();
            UndoHistory.Clear();
            _modified = false;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ReOpenDocument
        /// 
        /// <summary>
        /// 保存された内容でオブジェクトを再構成します。サムネイル用の
        /// イメージは保存前のものがそのまま利用できるので、それ以外の
        /// 情報を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ReOpenDocument(string path, CubePdf.Editing.PageBinder binder)
        {
            FilePath = path;

            var password = Encryption.IsEnabled && !string.IsNullOrEmpty(Encryption.OwnerPassword) ?
                           Encryption.OwnerPassword :
                           string.Empty;

            lock (_pages)
            {
                for (var i = 0; i < _pages.Count; ++i)
                {
                    var size = _pages[i].OriginalSize;
                    _pages[i] = new Page
                    {
                        FilePath     = path,
                        Password     = password,
                        PageNumber   = i + 1,
                        OriginalSize = size
                    };
                }
            }

            History.Clear();
            UndoHistory.Clear();
            _modified = false;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SaveDocument
        /// 
        /// <summary>
        /// 現在の構成で指定されたパスに保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void SaveDocument(string path, CubePdf.Editing.PageBinder binder)
        {
            lock (_pages)
            {
                foreach (var page in _pages) binder.Pages.Add(page);
                binder.Metadata = Metadata;
                binder.Encryption = Encryption;

                var tmp = System.IO.Path.GetTempFileName();
                binder.UseSmartCopy = true;
                binder.Save(tmp);

                _creator.Clear();
                CubePdf.Drawing.BitmapEnginePool.Clear();
                if (path == FilePath) CreateBackup();
                CubePdf.Misc.File.Move(tmp, path, true);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SaveDocument
        /// 
        /// <summary>
        /// 指定されたディレクトリにインデックスが指し示すページ内容を新たな
        /// PDF ファイルとして保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void SaveDocument(string directory, int index)
        {
            if (index < 0 || index >= _pages.Count) return;

            var page = _pages[index];
            var binder = new CubePdf.Editing.PageBinder();
            binder.Pages.Add(page);

            var format = string.Format("{{0}}-{{1:D{0}}}.pdf", _pages.Count.ToString().Length);
            var filename = string.Format(format, System.IO.Path.GetFileNameWithoutExtension(FilePath), index + 1);
            var dest = System.IO.Path.Combine(directory, filename);

            var tmp = System.IO.Path.GetTempFileName();
            binder.Save(tmp);
            CubePdf.Misc.File.Move(tmp, dest, true);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CloseDocument
        /// 
        /// <summary>
        /// 現在の状態を破棄して PDF ファイルを閉じます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CloseDocument()
        {
            _modified = false;
            FilePath = string.Empty;
            _metadata = null;
            _encrypt = null;
            EncryptionStatus = Data.EncryptionStatus.NotEncrypted;
            _maxwidth = 0;
            _maxheight = 0;
            History.Clear();
            UndoHistory.Clear();

            lock (_pages) _pages.Clear();
            _creator.Clear();
            lock (_images)
            {
                ClearImage();
                foreach (var image in _images) image.Dispose();
                _images.Clear();
            }
            CubePdf.Drawing.BitmapEnginePool.Clear();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InsertDocument
        /// 
        /// <summary>
        /// 引数に指定された IDocumentReader オブジェクトの各ページを index
        /// の位置に挿入します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void InsertDocument(int index, CubePdf.Data.IDocumentReader reader)
        {
            _creator.Remove(index, _pages.Count);
            var first = index;

            lock (_pages)
            lock (_images)
            {
                foreach (var page in reader.Pages)
                {
                    _pages.Insert(index, page);
                    UpdateImageSize(page);
                    _images.Insert(index, new Drawing.ImageContainer());
                    UpdateHistory(ListViewCommands.Insert, new KeyValuePair<int, PageBase>(index, page));
                    ++index;
                }
                _created.ItemInserted(first, reader.Pages.Count);
            }
            UpdateImageText(first);
        }

        #endregion

        #region Private methods for images

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateImage
        /// 
        /// <summary>
        /// 引数に指定されたインデックスに対応するイメージを更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateImage(int index)
        {
            lock (_images)
            {
                var element = _images.RawAt(index);
                if (_visibility == ListViewItemVisibility.Minimum)
                {
                    if (element.Status != Drawing.ImageStatus.Dummy)
                    {
                        element.UpdateImage(GetDummyImage(_pages[index]), Drawing.ImageStatus.Dummy);
                    }
                }
                else
                {
                    if (element.Status == Drawing.ImageStatus.None || element.Status == Drawing.ImageStatus.Dummy)
                    {
                        element.UpdateImage(GetLoadingImage(_pages[index]), Drawing.ImageStatus.Loading);
                    }
                    CreateImageAsync(index, _pages[index]);
                }
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CreateImageAsync
        /// 
        /// <summary>
        /// 非同期でサムネイルを作成します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CreateImageAsync(int index, PageBase page)
        {
            if (_images.RawAt(index).Status == Drawing.ImageStatus.Created) return;
            if (_visibility == ListViewItemVisibility.Minimum)
            {
                _creator.Clear();
                return;
            }

            var copy = page.Copy();
            copy.Power = GetPower(page);
            _creator.CreateAsync(copy, index);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ClearImage
        /// 
        /// <summary>
        /// 現在、保持している全ての Image オブジェクトを解放します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void ClearImage()
        {
            lock (_images)
            {
                for (int i = 0; i < _images.RawCount; ++i)
                {
                    if (_images.RawAt(i).Status != Drawing.ImageStatus.None)
                    {
                        Debug.WriteLine(string.Format("Image[{0}] {1} (in ClearImage)", i, _images.RawAt(i).Status.ToString()));
                    }
                    _images.RawAt(i).DeleteImage();
                }
            }

            Debug.WriteLine(string.Format("IndexTable: Count = {0}, Capacity = {1}", _created.Indices.Count, _created.Capacity));
            var debug = new System.Text.StringBuilder();
            foreach (var index in _created.Indices) debug.Append(string.Format(" {0}", index));
            Debug.WriteLine(string.Format("Indices: {0}", debug.ToString()));
            _created.Clear();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateImageSize
        /// 
        /// <summary>
        /// イメージのサイズを更新します。
        /// </summary>
        /// 
        /// <remarks>
        /// 幅/高さそれぞれの最大値を記憶しておき、それらの値を元に表示
        /// サイズを決定します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateImageSize(PageBase page)
        {
            var update = false;
            if (page.ViewSize().Width > _maxwidth)
            {
                _maxwidth = page.ViewSize().Width;
                update = true;
            }

            if (page.ViewSize().Height > _maxheight)
            {
                _maxheight = page.ViewSize().Height;
                update = true;
            }

            if (update)
            {
                OnPropertyChanged("ItemWidth");
                OnPropertyChanged("ItemHeight");
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateImageText
        /// 
        /// <summary>
        /// [first, last] の範囲にある ImageContainer の Text プロパティを
        /// 更新します。Text プロパティには、ページ番号を表す文字列が設定
        /// されます。last が省略された（または、-1 が指定された）場合は、
        /// [first, Items.Count) までの Text プロパティを更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateImageText(int first, int last = -1)
        {
            if (last == -1) last = _images.RawCount - 1;
            for (int i = first; i <= last; ++i) _images.RawAt(i).Text = (i + 1).ToString();
        }

        #endregion

        #region Private methods for backup

        /* ----------------------------------------------------------------- */
        ///
        /// CreateBackup
        /// 
        /// <summary>
        /// 現在、開いているファイルのバックアップを作成します。
        /// バックアップファイルは日付毎に管理し、既に同名のファイルが存在
        /// している場合は上書き処理等は行いません。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void CreateBackup()
        {
            if (string.IsNullOrEmpty(BackupFolder) || BackupDays <= 0) return;

            var dir = System.IO.Path.Combine(BackupFolder, DateTime.Today.ToString("yyyyMMdd"));
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);

            var filename = System.IO.Path.GetFileName(FilePath);
            var dest = System.IO.Path.Combine(dir, filename);
            if (!System.IO.File.Exists(dest)) System.IO.File.Copy(FilePath, dest);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// DeleteBackup
        /// 
        /// <summary>
        /// バックアップ保存期間の過ぎているファイルを消去します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void DeleteBackup()
        {
            if (string.IsNullOrEmpty(BackupFolder) || !System.IO.Directory.Exists(BackupFolder) || BackupDays <= 0) return;

            var expire = DateTime.Today.AddDays(-BackupDays).ToString("yyyyMMdd");
            foreach (var path in System.IO.Directory.GetDirectories(BackupFolder))
            {
                try
                {
                    var leaf = System.IO.Path.GetFileName(path);
                    if (leaf.CompareTo(expire) >= 0) continue;
                    System.IO.Directory.Delete(path, true);
                }
                catch (Exception err) { Trace.TraceError(err.ToString()); }
            }
        }

        #endregion

        #region Private methods for Undo/Redo

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateHistory
        /// 
        /// <summary>
        /// 履歴を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UpdateHistory(ICommand command, object parameter) { UpdateHistory(command, new ArrayList() { parameter }); }
        private void UpdateHistory(ICommand command, IList parameters)
        {
            var history = (_undostatus == UndoStatus.Undo) ? UndoHistory : History;
            if (_status != CommandStatus.Continue) history.Insert(0, new CommandElement(command));
            var element = history[0];
            if (command != element.Command) throw new ArgumentException(Properties.Resources.HistoryCommandException);

            foreach (var param in parameters) element.Parameters.Add(param);
            element.Text = GetCommandText(element);

            if (_status == CommandStatus.Begin) _status = CommandStatus.Continue;            
            if (_undostatus == UndoStatus.Normal) UndoHistory.Clear();
            if (History.Count > _maxundo)
            {
                History.RemoveAt(History.Count - 1);
                _modified = true;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoInsert
        ///
        /// <summary>
        /// 挿入操作を取り消します。
        /// パラメータ (parameters) は、インデックスと挿入された PDF ページ
        /// オブジェクトのペア (KeyValuePair(int, CubePdf.Data.Page))
        /// オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoInsert(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                for (int i = parameters.Count - 1; i >= 0; --i)
                {
                    var param = (KeyValuePair<int, PageBase>)parameters[i];
                    RemoveAt(param.Key);
                }
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoRemove
        ///
        /// <summary>
        /// 削除操作を取り消します。
        /// パラメータ (parameters) は、インデックスと削除された PDF ページ
        /// オブジェクトのペア (KeyValuePair(int, CubePdf.Data.Page))
        /// オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoRemove(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                for (int i = parameters.Count - 1; i >= 0; --i)
                {
                    var param = (KeyValuePair<int, PageBase>)parameters[i];
                    Insert(param.Key, param.Value);
                }
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoMove
        ///
        /// <summary>
        /// 移動操作を取り消します。
        /// パラメータ (parameters) は、移動元/移動先のインデックスのペア
        /// (KeyValuePair(int, int)) オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoMove(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                for (int i = parameters.Count - 1; i >= 0; --i)
                {
                    var param = (KeyValuePair<int, int>)parameters[i];
                    Move(param.Value, param.Key);
                }
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoRotate
        ///
        /// <summary>
        /// 回転操作を取り消します。
        /// パラメータ (parameters) は、インデックスと回転度数のペア
        /// (KeyValuePair(int, int)) オブジェクトが 1 つ以上指定されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoRotate(IList parameters)
        {
            if (parameters == null) return;
            try
            {
                BeginCommand();
                foreach (KeyValuePair<int, int> param in parameters) RotateAt(param.Key, -param.Value);
            }
            finally { EndCommand(); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoMetadata
        ///
        /// <summary>
        /// Metadata の変更を取り消します。
        /// パラメータ (parameters) の数は 1 つのみで、変更前の Metadata
        /// オブジェクトが格納されています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoMetadata(IList parameters)
        {
            if (parameters == null) return;
            var metadata = parameters[0] as CubePdf.Data.Metadata;
            if (metadata == null) return;

            Metadata = metadata;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoEncryption
        ///
        /// <summary>
        /// Encryption の変更を取り消します。
        /// パラメータ (parameters) の数は 1 つのみで、変更前の Encryption
        /// オブジェクトが格納されています。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void UndoEncryption(IList parameters)
        {
            if (parameters == null) return;
            var encrypt = parameters[0] as CubePdf.Data.Encryption;
            if (encrypt == null) return;

            Encryption = encrypt;
        }

        #endregion

        #region Internal classes

        /* ----------------------------------------------------------------- */
        ///
        /// CommandStatus
        ///
        /// <summary>
        /// 処理の状態を判別するための Enum 型です。主に、BeginCommand(),
        /// EndCommand() を判別するために使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        internal enum CommandStatus { Begin, Continue, End }

        /* ----------------------------------------------------------------- */
        ///
        /// UndoStatus
        /// 
        /// <summary>
        /// 実行されたコマンドが通常の処理なのか Undo/Redo なのかを判別する
        /// ために使用されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        internal enum UndoStatus { Normal, Undo, Redo }

        #endregion

        #region Variables

        #region Implementations for IDocumentReader and IDocumentWriter
        private Metadata _metadata = null;
        private Encryption _encrypt = null;
        private PageCollection _pages = new PageCollection();
        #endregion

        #region Others
        private bool _disposed = false;
        private int _viewsize = 0;
        private int _maxwidth = 0;
        private int _maxheight = 0;
        private int _maxundo = 30;
        private bool _modified = false;
        private ListViewItemVisibility _visibility = ListViewItemVisibility.Normal;
        private IListProxy<CubePdf.Drawing.ImageContainer> _images = null;
        private ImageCreator _creator = new ImageCreator();
        private IndexTable _created = null;
        private CommandStatus _status = CommandStatus.End;
        private UndoStatus _undostatus = UndoStatus.Normal;
        #endregion

        #endregion
    }
}
