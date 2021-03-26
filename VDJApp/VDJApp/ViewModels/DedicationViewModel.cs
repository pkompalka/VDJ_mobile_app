using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class DedicationViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public Song Song { get; set; }

        public string Dedication { get; set; } = string.Empty;

        public Command RequestSongCommand
        {
            get { return new Command(async () => await RequestSong()); }
        }

        public INavigation Navigation { get; set; }

        public DedicationViewModel(Song song = null, INavigation navigation = null)
        {
            Title = "Dedication";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            Song = song;
            Navigation = navigation;
        }

        public async Task RequestSong()
        {
            if(VDJAppSingletonInstance.RequestNumber > 2)
            {
                await Application.Current.MainPage.DisplayAlert("Unable to send request", "You have sent too many requests", "OK");
            }
            else
            {
                if(Dedication.Length > 150)
                {
                    await Application.Current.MainPage.DisplayAlert("Unable to send request", "Dedication too long (max 150 characters)", "OK");
                }
                else
                {
                    Request request = new Request
                    {
                        Nick = VDJAppSingletonInstance.Username,
                        SongId = Song.Id,
                        Dedication = Dedication
                    };

                    try
                    {
                        HttpResponseMessage requestResponse = await VDJAppSingletonInstance.HttpClientSi.PostAsJsonAsync("api/requests", request);

                        if (requestResponse.IsSuccessStatusCode)
                        {
                            VDJAppSingletonInstance.RequestNumber = VDJAppSingletonInstance.RequestNumber + 1;
                            string loginFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "login.txt");

                            string userLogin = File.ReadAllText(loginFile);
                            string[] separator = new string[] { "($||$)" };
                            string[] splitInstanceString = userLogin.Split(separator, StringSplitOptions.None);
                            int requestNumber = int.Parse(splitInstanceString[2]) + 1;
                            string stringToWrite = splitInstanceString[0] + "($||$)" + splitInstanceString[1] + "($||$)" + requestNumber + "($||$)" + splitInstanceString[3];

                            File.WriteAllText(loginFile, stringToWrite);

                            await Application.Current.MainPage.DisplayAlert("Success", "Request sent successfully", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Failed to send request", "OK");
                        }
                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
                    }
                }
            }
        }
    }
}
