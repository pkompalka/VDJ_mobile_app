using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class SongSelectedRequestViewModel : BaseViewModel
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

        public Command DedicationSongCommand
        {
            get { return new Command(async () => await DedicationSong()); }
        }

        public INavigation Navigation { get; set; }

        public SongSelectedRequestViewModel(Song song = null, INavigation navigation = null)
        {
            Title = "Song";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            Song = song;
            Navigation = navigation;
            ImageWidth = (int)(Application.Current.MainPage.Width * 0.5);
            LoadImage();
        }

        public async Task DedicationSong()
        {
            await Navigation.PushAsync(new DedicationPage(Song));
        }

        public async Task LoadImage()
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
