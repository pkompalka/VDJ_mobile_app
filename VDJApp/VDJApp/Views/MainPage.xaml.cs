using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using VDJApp.Models;

namespace VDJApp.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        readonly Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();

        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Info, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.NowPlaying:
                        MenuPages.Add(id, new NavigationPage(new NowPlayingPage()));
                        break;
                    case (int)MenuItemType.Playlist:
                        MenuPages.Add(id, new NavigationPage(new PlaylistPage()));
                        break;
                    case (int)MenuItemType.TopSong:
                        MenuPages.Add(id, new NavigationPage(new TopSongsPage()));
                        break;
                    case (int)MenuItemType.RequestSong:
                        MenuPages.Add(id, new NavigationPage(new RequestSongPage()));
                        break;
                    case (int)MenuItemType.Chat:
                        MenuPages.Add(id, new NavigationPage(new ChatPage()));
                        break;
                    case (int)MenuItemType.Donate:
                        MenuPages.Add(id, new NavigationPage(new DonatePage()));
                        break;
                    case (int)MenuItemType.Info:
                        MenuPages.Add(id, new NavigationPage(new InfoPage()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}