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
    public class LoginViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        private bool isButtonNotClicked;

        public bool IsButtonNotClicked
        {
            get { return isButtonNotClicked; }
            set { SetProperty(ref isButtonNotClicked, value); }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        public string IsUsernameUnique { get; set; }

        private string username = string.Empty;

        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        private string password = string.Empty;

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string noLoginMessage = string.Empty;

        public string NoLoginMessage
        {
            get { return noLoginMessage; }
            set { SetProperty(ref noLoginMessage, value); }
        }

        public Command TryLoginCommand
        {
            get { return new Command(async () => await TryLogin()); }
        }

        public LoginViewModel()
        {
            Title = "VDJApp";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            IsButtonNotClicked = true;
            IsLoading = false;
        }

        public async Task TryLogin()
        {
            Username = Username.Trim();
            if(Username.Length < 3)
            {
                NoLoginMessage = "Username too short!";
            }
            else if(Username.Length > 20)
            {
                NoLoginMessage = "Username too long!";
            }
            else
            {
                NoLoginMessage = "";
                IsLoading = true;
                IsButtonNotClicked = false;

                try
                {
                    HttpResponseMessage passwordHttpResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync("api/Users/password");

                    if (passwordHttpResponse.IsSuccessStatusCode)
                    {
                        string jsonCorrectPassword = await passwordHttpResponse.Content.ReadAsStringAsync();
                        string CorrectPassword = JsonConvert.DeserializeObject<string>(jsonCorrectPassword);

                        if (Password == CorrectPassword)
                        {
                            HttpResponseMessage isUniqueResponse = await VDJAppSingletonInstance.HttpClientSi.PostAsJsonAsync("api/Users/adduser", Username);

                            if (isUniqueResponse.IsSuccessStatusCode)
                            {
                                IsUsernameUnique = await isUniqueResponse.Content.ReadAsStringAsync();

                                if (IsUsernameUnique == "0")
                                {
                                    NoLoginMessage = "Username already taken!";
                                }
                                else
                                {
                                    VDJAppSingletonInstance.Username = Username;
                                    string loginDate = DateTime.Now.ToString();
                                    string stringToWrite = loginDate + "($||$)" + Username + "($||$)" + "0" + "($||$)";
                                    string loginFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "login.txt");
                                    File.WriteAllText(loginFile, stringToWrite);

                                    await VDJAppSingletonInstance.LoadSongDatabase();
                                    App.Current.MainPage = new MainPage();
                                    await Application.Current.MainPage.DisplayAlert("Vote for favourite songs!", "By pressing VOTE button in the top right corner you can vote for songs you like and help them get into top songs chart.", "OK");
                                }
                            }
                            else
                            {
                                NoLoginMessage = "Server error! Try again";
                            }
                        }
                        else
                        {
                            NoLoginMessage = "Wrong password!";
                        }
                    }
                    else
                    {
                        NoLoginMessage = "Server error! Try again";
                    }
                }
                catch
                {
                    NoLoginMessage = "Unable to connect, check your network connection";
                }
                IsLoading = false;
                IsButtonNotClicked = true;
            }
        }
    }
}
