using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class NowPlayingViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public Song CurrentSong { get; set; }

        public Song NextSong { get; set; }

        public int ImageWidth { get; set; }

        public int ImageNextWidth { get; set; }

        public bool IsVoteEnabled { get; set; }

        private string currentSongTitle = string.Empty;

        public string CurrentSongTitle
        {
            get { return currentSongTitle; }
            set { SetProperty(ref currentSongTitle, value); }
        }

        private string currentSongAuthor = string.Empty;

        public string CurrentSongAuthor
        {
            get { return currentSongAuthor; }
            set { SetProperty(ref currentSongAuthor, value); }
        }

        private string currentSongAlbum = string.Empty;

        public string CurrentSongAlbum
        {
            get { return currentSongAlbum; }
            set { SetProperty(ref currentSongAlbum, value); }
        }

        private string nextSongInfo = string.Empty;

        public string NextSongInfo
        {
            get { return nextSongInfo; }
            set { SetProperty(ref nextSongInfo, value); }
        }

        private string byString = string.Empty;

        public string ByString
        {
            get { return byString; }
            set { SetProperty(ref byString, value); }
        }

        private string fromAlbumString = string.Empty;

        public string FromAlbumString
        {
            get { return fromAlbumString; }
            set { SetProperty(ref fromAlbumString, value); }
        }

        private ImageSource currentSource;

        public ImageSource CurrentSource
        {
            get { return currentSource; }
            set { SetProperty(ref currentSource, value); }
        }

        private ImageSource nextSource;

        public ImageSource NextSource
        {
            get { return nextSource; }
            set { SetProperty(ref nextSource, value); }
        }

        public Command SendVoteCommand
        {
            get { return new Command(async () => await SendVote()); }
        }

        public Command GetCurrentSongCommand
        {
            get { return new Command(async () => await GetCurrentSong()); }
        }

        public NowPlayingViewModel()
        {
            Title = "Current song";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            CurrentSong = new Song();
            NextSong = new Song();
            ImageWidth = (int)(Application.Current.MainPage.Width * 0.5);
            ImageNextWidth = (int)(Application.Current.MainPage.Width * 0.2);
        }

        public async Task SendVote()
        {
            if (IsVoteEnabled) return;

            IsVoteEnabled = true;

            if (CurrentSongTitle == "No song is playing right now!")
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No song to vote for!", "OK");
            }
            else
            {
                if (VDJAppSingletonInstance.SongList.Find(x => x.Id == CurrentSong.Id).WasVoted == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Unable to vote", "You have already voted for this song", "OK");
                }
                else
                {
                    try
                    {
                        HttpResponseMessage votedResponse = await VDJAppSingletonInstance.HttpClientSi.PutAsJsonAsync("api/songs", CurrentSong.Id);

                        if (votedResponse.IsSuccessStatusCode)
                        {
                            VDJAppSingletonInstance.SongList.Find(x => x.Id == CurrentSong.Id).WasVoted = true;
                            string loginFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "login.txt");
                            string userLogin = File.ReadAllText(loginFile);
                            string stringToWrite = userLogin + CurrentSong.Id + ",";
                            File.WriteAllText(loginFile, stringToWrite);
                            await Application.Current.MainPage.DisplayAlert("Success", "Vote sent successfully", "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Failed to send vote", "OK");
                        }
                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
                    }
                }
            }

            IsVoteEnabled = false;
        }

        public async Task GetCurrentSong()
        {
            try
            {
                HttpResponseMessage currentSongsResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync("api/currentsongs");

                if (currentSongsResponse.IsSuccessStatusCode)
                {
                    string jsonCurrent = await currentSongsResponse.Content.ReadAsStringAsync();
                    int[] currentSongsArray = JsonConvert.DeserializeObject<int[]>(jsonCurrent);

                    if (currentSongsArray[0] == -1 && currentSongsArray[1] == -1)
                    {
                        CurrentSongTitle = "No song is playing right now!";
                        CurrentSongAuthor = "";
                        CurrentSongAlbum = "";
                        CurrentSource = null;
                        ByString = "";
                        FromAlbumString = "";
                        NextSongInfo = "Starting soon!";
                        NextSource = null;
                    }
                    else
                    {
                        if (currentSongsArray[1] == -1)
                        {
                            for (int i = 0; i < VDJAppSingletonInstance.SongList.Count; i++)
                            {
                                if (currentSongsArray[0] == VDJAppSingletonInstance.SongList[i].Id)
                                {
                                    CurrentSong = VDJAppSingletonInstance.SongList[i];
                                    break;
                                }
                            }

                            if (CurrentSong.Cover == "")
                            {
                                string songIdToSend = CurrentSong.Id.ToString();

                                HttpResponseMessage coverHttpResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync($"api/Songs/cover/{songIdToSend}");

                                if (coverHttpResponse.IsSuccessStatusCode)
                                {
                                    string jsonCover = await coverHttpResponse.Content.ReadAsStringAsync();
                                    string[] coverConverted = JsonConvert.DeserializeObject<string[]>(jsonCover);
                                    byte[] coverByte = Convert.FromBase64String(coverConverted[0]);
                                    CurrentSource = ImageSource.FromStream(() => new MemoryStream(coverByte));

                                    for (int i = 0; i < VDJAppSingletonInstance.SongList.Count; i++)
                                    {
                                        if (VDJAppSingletonInstance.SongList[i].Id == CurrentSong.Id)
                                        {
                                            VDJAppSingletonInstance.SongList[i].Cover = coverConverted[0];
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] coverByte = Convert.FromBase64String(CurrentSong.Cover);
                                CurrentSource = ImageSource.FromStream(() => new MemoryStream(coverByte));
                            }

                            CurrentSongTitle = CurrentSong.Title;
                            CurrentSongAuthor = CurrentSong.FullAuthor;
                            CurrentSongAlbum = CurrentSong.Album;
                            ByString = "by";
                            FromAlbumString = "From album:";
                            NextSongInfo = "No next song!";
                            NextSource = null;
                        }
                        else
                        {
                            for (int i = 0; i < VDJAppSingletonInstance.SongList.Count; i++)
                            {
                                if (currentSongsArray[0] == VDJAppSingletonInstance.SongList[i].Id)
                                {
                                    CurrentSong = VDJAppSingletonInstance.SongList[i];
                                    break;
                                }
                            }

                            for (int i1 = 0; i1 < VDJAppSingletonInstance.SongList.Count; i1++)
                            {
                                if (currentSongsArray[1] == VDJAppSingletonInstance.SongList[i1].Id)
                                {
                                    NextSong = VDJAppSingletonInstance.SongList[i1];
                                    break;
                                }
                            }

                            if (CurrentSong.Cover == "")
                            {
                                string songIdToSend = CurrentSong.Id.ToString();

                                HttpResponseMessage coverHttpResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync($"api/Songs/cover/{songIdToSend}");

                                if (coverHttpResponse.IsSuccessStatusCode)
                                {
                                    string jsonCover = await coverHttpResponse.Content.ReadAsStringAsync();
                                    string[] coverConverted = JsonConvert.DeserializeObject<string[]>(jsonCover);
                                    byte[] coverByte = Convert.FromBase64String(coverConverted[0]);
                                    CurrentSource = ImageSource.FromStream(() => new MemoryStream(coverByte));

                                    for (int i = 0; i < VDJAppSingletonInstance.SongList.Count; i++)
                                    {
                                        if (VDJAppSingletonInstance.SongList[i].Id == CurrentSong.Id)
                                        {
                                            VDJAppSingletonInstance.SongList[i].Cover = coverConverted[0];
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] coverByte = Convert.FromBase64String(CurrentSong.Cover);
                                CurrentSource = ImageSource.FromStream(() => new MemoryStream(coverByte));
                            }

                            if (NextSong.Cover == "")
                            {
                                string songIdToSend = NextSong.Id.ToString();

                                HttpResponseMessage coverHttpResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync($"api/Songs/cover/{songIdToSend}");

                                if (coverHttpResponse.IsSuccessStatusCode)
                                {
                                    string jsonCover = await coverHttpResponse.Content.ReadAsStringAsync();
                                    string[] coverConverted = JsonConvert.DeserializeObject<string[]>(jsonCover);
                                    byte[] coverByte = Convert.FromBase64String(coverConverted[0]);
                                    NextSource = ImageSource.FromStream(() => new MemoryStream(coverByte));

                                    for (int i = 0; i < VDJAppSingletonInstance.SongList.Count; i++)
                                    {
                                        if (VDJAppSingletonInstance.SongList[i].Id == NextSong.Id)
                                        {
                                            VDJAppSingletonInstance.SongList[i].Cover = coverConverted[0];
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] coverNextByte = Convert.FromBase64String(NextSong.Cover);
                                NextSource = ImageSource.FromStream(() => new MemoryStream(coverNextByte));
                            }

                            CurrentSongTitle = CurrentSong.Title;
                            CurrentSongAuthor = CurrentSong.FullAuthor;
                            CurrentSongAlbum = CurrentSong.Album;
                            ByString = "by";
                            FromAlbumString = "From album:";
                            NextSongInfo = "Next song: " + NextSong.Title + " by " + NextSong.FullAuthor;
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to get current song", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
            }
        }
    }
}
