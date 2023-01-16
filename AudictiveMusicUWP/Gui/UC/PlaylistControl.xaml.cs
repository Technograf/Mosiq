using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class PlaylistControl : UserControl
    {
        public ObservableCollection<Song> PlaylistList = new ObservableCollection<Song>();
        //private Compositor _compositor;
        //private SpriteVisual blurSprite;
        //private CompositionEffectBrush _brush;

        private int tempDragOldIndex;
        private int tempDragNewIndex;
        private object tempDragItem;

        private bool reordering { get => tempDragItem != null; }

        public enum EditMode
        {
            Enabled,
            Disabled
        }

        public EditMode editMode
        {
            get;
            set;
        }

        public static readonly DependencyProperty CurrentTrackProperty = 
            DependencyProperty.Register("CurrentTrackIndex", typeof(int), typeof(PlaylistControl), new PropertyMetadata(0));

        public PlaylistControl()
        {
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;
            this.InitializeComponent();
            MusicPlaylist.ItemsSource = PlaylistList;
            CurrentTrackIndex = -1;
            editMode = EditMode.Disabled;

            //if (ApplicationInfo.Current.IsMobile == false)
                //_compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void PlaylistList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                tempDragOldIndex = e.OldStartingIndex;
                tempDragItem = e.OldItems[0];
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                MusicPlaylist.SelectionChanged -= Playlist_SelectionChanged;
                tempDragNewIndex = e.NewStartingIndex;
                MessageService.SendMessageToBackground(new EditPlaylistMessage(Mode.DragAndDrop, tempDragOldIndex, tempDragNewIndex));
                tempDragItem = null;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move
                || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                int newIndex = e.NewStartingIndex;

                
            }
        }

        public void Clear()
        {
            MusicPlaylist.SelectionChanged -= Playlist_SelectionChanged;
            PlaylistList.Clear();
            MusicPlaylist.SelectionChanged += Playlist_SelectionChanged;
        }

        public int Count
        {
            get
            {
                return PlaylistList.Count;
            }
        }

        public void SetIndex(int index)
        {

        }

        public void Insert(int position, Song item)
        {
            PlaylistList.CollectionChanged -= PlaylistList_CollectionChanged;
            PlaylistList.Insert(position, item);
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;
        }

        public void Add(Song item)
        {
            PlaylistList.CollectionChanged -= PlaylistList_CollectionChanged;
            PlaylistList.Add(item);
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;
        }

        public void SetLoadingState(bool state)
        {
            progress.IsActive = state;
        }

        public async void ScrollToSelectedIndex()
        {
            try
            {
                if (MusicPlaylist.Items.Count - 1 >= CurrentTrackIndex)
                    await MusicPlaylist.ScrollToIndex(CurrentTrackIndex);
            }
            catch
            {

            }
        }

        public int CurrentTrackIndex
        {
            get { return (int)this.GetValue(CurrentTrackProperty); }
            set
            {
                this.SetValue(CurrentTrackProperty, value);
                if (editMode == EditMode.Disabled)
                {
                    if (value <= PlaylistList.Count - 1)
                    {
                        MusicPlaylist.SelectionChanged -= Playlist_SelectionChanged;
                        MusicPlaylist.SelectedIndex = value;
                        MusicPlaylist.SelectionChanged += Playlist_SelectionChanged;
                    }
                }
            }
        }

        private void UpdatePreviousAndNext()
        {
            //if (PlaylistList.Count == 1)
            //{
            //    ApplicationData.Current.LocalSettings.Values["PreviousSong"] = PlaylistList[0].SongURI;
            //    ApplicationData.Current.LocalSettings.Values["NextSong"] = PlaylistList[0].SongURI;
            //}
            //else
            //{
            //    string prev, next = string.Empty;
            //    if (CurrentTrackIndex > 0)
            //        prev = PlaylistList[CurrentTrackIndex - 1].SongURI;
            //    else
            //        prev = PlaylistList[PlaylistList.Count - 1].SongURI;

            //    ApplicationData.Current.LocalSettings.Values["PreviousSong"] = prev;

            //    if (CurrentTrackIndex < PlaylistList.Count - 1)
            //        next = PlaylistList[CurrentTrackIndex + 1].SongURI;
            //    else
            //        next = PlaylistList[0].SongURI;

            //    ApplicationData.Current.LocalSettings.Values["NextSong"] = next;
            //}
        }

        private void RemoveSelectedSongs_Click(object sender, RoutedEventArgs e)
        {
            int index = PlaylistList.IndexOf(MusicPlaylist.SelectedItem as Song);
            PlaylistList.CollectionChanged -= PlaylistList_CollectionChanged;
            PlaylistList.RemoveAt(index);
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;

            UpdatePreviousAndNext();

            MessageService.SendMessageToBackground(new EditPlaylistMessage(Mode.Remove, index));
        }

        private void MoveSongDownButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistList.CollectionChanged -= PlaylistList_CollectionChanged;
            int index = MusicPlaylist.SelectedIndex;
            var item = MusicPlaylist.SelectedItem;
            PlaylistList.Remove(item as Song);
            PlaylistList.Insert(index + 1, item as Song);
            MusicPlaylist.SelectedIndex = index + 1;
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;

            UpdatePreviousAndNext();

            MessageService.SendMessageToBackground(new EditPlaylistMessage(Mode.MoveDown, index));
        }

        private void MoveSongUpButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistList.CollectionChanged -= PlaylistList_CollectionChanged;

            int index = MusicPlaylist.SelectedIndex;
            var item = MusicPlaylist.SelectedItem;
            PlaylistList.Remove(item as Song);
            PlaylistList.Insert(index - 1, item as Song);
            MusicPlaylist.SelectedIndex = index - 1;
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;

            UpdatePreviousAndNext();

            MessageService.SendMessageToBackground(new EditPlaylistMessage(Mode.MoveUp, index));
        }

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicPlaylist.SelectedItem == null)
                return;

            if (reordering)
                return;

            MessageService.SendMessageToBackground(new JumpToIndexMessage(MusicPlaylist.SelectedIndex));

            Debug.WriteLine("THE INDEX TO MOVE IS: " + MusicPlaylist.SelectedIndex);
        }

        private void SavePlaylist_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            foreach (Song item in PlaylistList)
            {
                list.Add(item.SongURI);
            }

            if (PageHelper.MainPage != null)
            {
                PageHelper.MainPage.CreateAddToPlaylistPopup(list);
            }
        }

        private void MusicPlaylist_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            ShowDropArea();

            e.Data.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;

            Song song = e.Items.First() as Song;
            int index = PlaylistList.IndexOf(song);

            e.Data.SetData("indexToRemove", index);
        }

        private void ShowDropArea()
        {
            dropArea.IsHitTestVisible = true;
            Storyboard sb = this.Resources["showDropAreaAnimation"] as Storyboard;
            sb.Begin();
        }


        private async void dropArea_Drop(object sender, DragEventArgs e)
        {
            //HideDropArea();
            object obj = await e.DataView.GetDataAsync("indexToRemove");
            int index = Convert.ToInt32(obj);

            tempDragItem = null;
            tempDragNewIndex = 0;
            tempDragOldIndex = 0;

            PlaylistList.CollectionChanged -= PlaylistList_CollectionChanged;
            PlaylistList.RemoveAt(index);
            PlaylistList.CollectionChanged += PlaylistList_CollectionChanged;

            UpdatePreviousAndNext();

            MessageService.SendMessageToBackground(new EditPlaylistMessage(Mode.Remove, index));

            HideDropAreaOverlayColor();
        }

        private void HideDropArea()
        {
            dropArea.IsHitTestVisible = false;

            Storyboard sb = this.Resources["hideDropAreaAnimation"] as Storyboard;
            sb.Begin();
        }

        private void MusicPlaylist_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            HideDropArea();
        }

        private void dropArea_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            /*e.DragUIOverride.IsCaptionVisible = */e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.Caption = ApplicationInfo.Current.Resources.GetString("Remove/Content");

            Storyboard sb = new Storyboard();
            DoubleAnimation ca = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(ca, dropAreaOverlay);
            Storyboard.SetTargetProperty(ca, "Opacity");

            sb.Children.Add(ca);
            sb.Begin();
        }

        private void MusicPlaylist_Drop(object sender, DragEventArgs e)
        {
            HideDropArea();
        }

        private void MusicPlaylist_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            HideDropArea();
        }

        private void dropArea_DragLeave(object sender, DragEventArgs e)
        {
            HideDropAreaOverlayColor();
        }

        private void HideDropAreaOverlayColor()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation ca = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(ca, dropAreaOverlay);
            Storyboard.SetTargetProperty(ca, "Opacity");

            sb.Children.Add(ca);
            sb.Begin();
        }
    }
}
