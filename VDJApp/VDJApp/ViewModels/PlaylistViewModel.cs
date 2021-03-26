using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public ObservableCollection<Song> SongPlaylistObservable { get; set; }

        public INavigation Navigation { get; set; }

        public Command LoadPlaylistCommand
        {
            get { return new Command(async () => await LoadPlaylist()); }
        }

        public Command ClearPlaylistCommand
        {
            get { return new Command(() => ClearPlaylist()); }
        }

        public PlaylistViewModel(INavigation navigation = null)
        {
            Title = "Today's playlist";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            SongPlaylistObservable = new ObservableCollection<Song>();
            Navigation = navigation;
            MessagingCenter.Subscribe<object, object>(this, "TappedSongPlaylist", async (sender, args) =>
            {
                await Navigation.PushAsync(new SongDetailPage(new SongDetailViewModel((Song)args)));
            });
        }
        
        public async Task LoadPlaylist()
        {
            try
            {
                HttpResponseMessage playlistResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync("api/playlists");

                if (playlistResponse.IsSuccessStatusCode)
                {
                    string jsonPlaylist = await playlistResponse.Content.ReadAsStringAsync();
                    List<Playlist> playlistList = JsonConvert.DeserializeObject<List<Playlist>>(jsonPlaylist);
                    SongPlaylistObservable.Clear();
                    bool WasSongPlayed = true;

                    for (int i = 0; i < playlistList.Count; i++)
                    {
                        for (int i1 = 0; i1 < VDJAppSingletonInstance.SongList.Count; i1++)
                        {
                            if (playlistList[i].SongId == VDJAppSingletonInstance.SongList[i1].Id)
                            {
                                VDJAppSingletonInstance.SongList[i1].ColorPlayed = "#FF1A1818";
                                if (WasSongPlayed == true)
                                {
                                    WasSongPlayed = playlistList[i].WasPlayed;

                                    if (WasSongPlayed == false)
                                    {
                                        if (SongPlaylistObservable.Count == 0)
                                        {

                                        }
                                        else
                                        {
                                            SongPlaylistObservable.Last().ColorPlayed = "Green";
                                        }
                                    }
                                }

                                SongPlaylistObservable.Add(VDJAppSingletonInstance.SongList[i1]);

                                if (WasSongPlayed == true && i == playlistList.Count - 1)
                                {
                                    SongPlaylistObservable.Last().ColorPlayed = "Green";
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to load playlist", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
            }
        }

        public void ClearPlaylist()
        {
            SongPlaylistObservable.Clear();
        }
    }
}