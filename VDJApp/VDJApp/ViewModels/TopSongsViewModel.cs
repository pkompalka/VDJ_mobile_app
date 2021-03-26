using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class TopSongsViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public ObservableCollection<Song> VotedSongObservable { get; set; }

        public INavigation Navigation { get; set; }

        public Command GetTop5Command
        {
            get { return new Command(async () => await GetTop5()); }
        }

        public TopSongsViewModel(INavigation navigation = null)
        {
            Title = "Top songs";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            VotedSongObservable = new ObservableCollection<Song>();
            Navigation = navigation;
            MessagingCenter.Subscribe<object, object>(this, "TappedSongVoted", async (sender, args) =>
            {
                await Navigation.PushAsync(new SongDetailPage(new SongDetailViewModel((Song)args)));
            });
        }

        public async Task GetTop5()
        {
            try
            {
                HttpResponseMessage votesResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync("api/songs/votes");

                if (votesResponse.IsSuccessStatusCode)
                {
                    string jsonVotes = await votesResponse.Content.ReadAsStringAsync();
                    List<VotedSong> votesSongList = JsonConvert.DeserializeObject<List<VotedSong>>(jsonVotes);
                    VotedSongObservable.Clear();
                    List<Song> SongListToObservable = new List<Song>();

                    for (int i = 0; i < 5; i++)
                    {
                        for (int i1 = 0; i1 < VDJAppSingletonInstance.SongList.Count; i1++)
                        {
                            if (votesSongList[i].SongId == VDJAppSingletonInstance.SongList[i1].Id)
                            {
                                VDJAppSingletonInstance.SongList[i1].Score = votesSongList[i].Votes;
                                SongListToObservable.Add(VDJAppSingletonInstance.SongList[i1]);
                                break;
                            }
                        }
                    }

                    string idToSend = "";

                    for (int i2 = 0; i2 < 5; i2++)
                    {
                        if (SongListToObservable[i2].Cover == "")
                        {
                            idToSend = idToSend + SongListToObservable[i2].Id + ",";
                        }
                    }

                    if (idToSend == "")
                    {

                    }
                    else
                    {
                        idToSend = idToSend.Remove(idToSend.Length - 1);

                        HttpResponseMessage coverHttpResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync($"api/Songs/cover/{idToSend}");

                        if (coverHttpResponse.IsSuccessStatusCode)
                        {
                            string jsonCover = await coverHttpResponse.Content.ReadAsStringAsync();
                            string[] coverConverted = JsonConvert.DeserializeObject<string[]>(jsonCover);

                            string[] idArray = idToSend.Split(',');

                            for (int i3 = 0; i3 < coverConverted.Length; i3++)
                            {
                                for (int i4 = 0; i4 < VDJAppSingletonInstance.SongList.Count; i4++)
                                {
                                    if (VDJAppSingletonInstance.SongList[i4].Id == int.Parse(idArray[i3]))
                                    {
                                        VDJAppSingletonInstance.SongList[i4].Cover = coverConverted[i3];
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        for (int i1 = 0; i1 < VDJAppSingletonInstance.SongList.Count; i1++)
                        {
                            if (votesSongList[i].SongId == VDJAppSingletonInstance.SongList[i1].Id)
                            {
                                VDJAppSingletonInstance.SongList[i1].Score = votesSongList[i].Votes;
                                byte[] coverByte = Convert.FromBase64String(VDJAppSingletonInstance.SongList[i1].Cover);
                                VDJAppSingletonInstance.SongList[i1].CoverSource = ImageSource.FromStream(() => new MemoryStream(coverByte));
                                VotedSongObservable.Add(VDJAppSingletonInstance.SongList[i1]);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to get top songs", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
            }
        }
    }
}
