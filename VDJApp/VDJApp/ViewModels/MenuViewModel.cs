using System.Collections.ObjectModel;
using System.Windows.Input;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public ObservableCollection<HomeMenuItem> HomeMenuItemObservable { get; set; }

        public MainPage RootPage { get; set; }

        public HomeMenuItem SelectedHomeItem { get; set; }

        public ICommand ItemSelectedCommand { get; private set; }

        public INavigation Navigation { get; set; }

        public MenuViewModel(INavigation navigation = null)
        {
            Navigation = navigation;

            HomeMenuItemObservable = new ObservableCollection<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.NowPlaying, Title="Now playing" },
                new HomeMenuItem {Id = MenuItemType.Playlist, Title="Today's playlist" },
                new HomeMenuItem {Id = MenuItemType.TopSong, Title="Top songs" },
                new HomeMenuItem {Id = MenuItemType.RequestSong, Title="Request song" },
                new HomeMenuItem {Id = MenuItemType.Chat, Title="Chat" },
                new HomeMenuItem {Id = MenuItemType.Donate, Title="Donate" },
                new HomeMenuItem {Id = MenuItemType.Info, Title="Info" }
            };

            SelectedHomeItem = HomeMenuItemObservable[6];
            ItemSelectedCommand = new Command<HomeMenuItem>(ItemSelectedMethod);
        }

        async void ItemSelectedMethod(object itemSelected)
        {
            if (itemSelected == null)
            {
                return;
            }

            int id = (int)((HomeMenuItem)itemSelected).Id;
            RootPage = Application.Current.MainPage as MainPage;
            await RootPage.NavigateFromMenu(id);
        }
    }
}

