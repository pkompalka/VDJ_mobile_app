using Xamarin.Forms;
using VDJApp.Views;
using VDJApp.Models;

namespace VDJApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new StartPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}