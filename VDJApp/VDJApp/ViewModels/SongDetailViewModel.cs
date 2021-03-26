using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class SongDetailViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public Song Song { get; set; }

        public int ImageWidth { get; set; }

        private ImageSource currentSource;

        public ImageSource CurrentSource
        {
            get { return currentSource; }
            set { SetProperty(ref currentSource, value); }
        }

        public Command SendVoteCommand
        {
            get { return new Command(async () => await SendVote()); }
        }

        private bool isVoteEnabled;

        public SongDetailViewModel(Song song = null)
        {
            Title = "Song";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            Song = song;
            ImageWidth = (int)(Application.Current.MainPage.Width * 0.5);
            LoadImage();
        }

        public async Task SendVote()
        {
            if (isVoteEnabled) return;

            isVoteEnabled = true;

            if (VDJAppSingletonInstance.SongList.Find(x => x.Id == Song.Id).WasVoted == true)
            {
                await Application.Current.MainPage.DisplayAlert("Unable to vote", "You have already voted for this song", "OK");
            }
            else
            {
                try
                {
                    HttpResponseMessage votedResponse = await VDJAppSingletonInstance.HttpClientSi.PutAsJsonAsync("api/songs", Song.Id);

                    if (votedResponse.IsSuccessStatusCode)
                    {
                        VDJAppSingletonInstance.SongList.Find(x => x.Id == Song.Id).WasVoted = true;
                        string loginFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "login.txt");
                        string userLogin = File.ReadAllText(loginFile);
                        string stringToWrite = userLogin + Song.Id + ",";
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
                    await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try again", "OK");
                }
            }

            isVoteEnabled = false;
        }

        public async void LoadImage()
        {
            if (Song.Cover == "")
            {
                string songIdToSend = Song.Id.ToString();

                try
                {
                    HttpResponseMessage coverHttpResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync($"api/Songs/cover/{songIdToSend}");

                    if (coverHttpResponse.IsSuccessStatusCode)
                    {
                        string jsonCover = await coverHttpResponse.Content.ReadAsStringAsync();
                        string[] coverConverted = JsonConvert.DeserializeObject<string[]>(jsonCover);
                        byte[] coverByte = Convert.FromBase64String(coverConverted[0]);
                        CurrentSource = ImageSource.FromStream(() => new MemoryStream(coverByte));

                        for (int i = 0; i < VDJAppSingletonInstance.SongList.Count; i++)
                        {
                            if (VDJAppSingletonInstance.SongList[i].Id == Song.Id)
                            {
                                VDJAppSingletonInstance.SongList[i].Cover = coverConverted[0];
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("Unable to load cover", "Check your network connection", "OK");
                }
            }
            else
            {
                byte[] coverByte = Convert.FromBase64String(Song.Cover);
                CurrentSource = ImageSource.FromStream(() => new MemoryStream(coverByte));
            }
        }
    }
}
