using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class InfoViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        private string username = string.Empty;

        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        private string requestsNumberInfo = string.Empty;

        public string RequestsNumberInfo
        {
            get { return requestsNumberInfo; }
            set { SetProperty(ref requestsNumberInfo, value); }
        }

        private string usersNumberInfo = string.Empty;

        public string UsersNumberInfo
        {
            get { return usersNumberInfo; }
            set { SetProperty(ref usersNumberInfo, value); }
        }

        public Command LogOutCommand
        {
            get { return new Command(async () => await LogOut()); }
        }

        public Command LoadInfoCommand
        {
            get { return new Command(async () => await LoadInfo()); }
        }

        public InfoViewModel()
        {
            Title = "VDJApp";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            Username = "Hello " + VDJAppSingletonInstance.Username;
        }

        public async Task LoadInfo()
        {
            int requestsNumber = 3 - VDJAppSingletonInstance.RequestNumber;
            RequestsNumberInfo = "Number of song requests you can send: " + requestsNumber;

            try
            {
                HttpResponseMessage userNumberResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync("api/Users/number");

                if (userNumberResponse.IsSuccessStatusCode)
                {
                    string jsonNumber = await userNumberResponse.Content.ReadAsStringAsync();
                    int userNumber = JsonConvert.DeserializeObject<int>(jsonNumber);
                    UsersNumberInfo = "There are " + userNumber + " people using app today!";
                }

                if (VDJAppSingletonInstance.SongList.Count == 0)
                {
                    await VDJAppSingletonInstance.LoadSongDatabase();
                    VDJAppSingletonInstance.LoadVotes();
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
            }
        }

        public async Task LogOut()
        {
            bool logOutAnswer = await Application.Current.MainPage.DisplayAlert("Are you sure?", "You will not be able to log back", "LOG OUT", "CANCEL");
            if(logOutAnswer == true)
            {
                string loginFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "login.txt");
                File.WriteAllText(loginFile, "");
                VDJAppSingletonInstance.SongList = new List<Song>();
                VDJAppSingletonInstance.RequestNumber = 0;
                VDJAppSingletonInstance.ChatNumber = 0;
                VDJAppSingletonInstance.Username = "";
                App.Current.MainPage = new LoginPage();
            }
        }
    }
}
