using System;
using System.IO;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public INavigation Navigation { get; set; }

        public Command LoadAppCommand
        {
            get { return new Command( () => LoadApp()); }
        }

        public StartViewModel(INavigation navigation = null)
        {
            Navigation = navigation;
        }

        public void LoadApp()
        {
            string loginFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "login.txt");
            bool doesFileExist = File.Exists(loginFile);
            if (doesFileExist == false)
            {
                File.WriteAllText(loginFile, "");
            }

            string userLogin = File.ReadAllText(loginFile);
            if (userLogin == "")
            {
                App.Current.MainPage = new LoginPage();
            }
            else
            {
                VDJAppSingletonInstance = VDJAppSingleton.Instance;
                int allowedToLog = VDJAppSingletonInstance.ReloadInstance(userLogin);
                if (allowedToLog == -1)
                {
                    App.Current.MainPage = new LoginPage();
                }
                else
                {
                    App.Current.MainPage = new MainPage();
                }
            }
        }
    }
}
