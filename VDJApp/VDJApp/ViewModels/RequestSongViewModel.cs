using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class RequestSongViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public ObservableCollection<Song> SongDatabaseObservable { get; set; }

        public List<Song> SongDatabaseToAdd { get; set; }

        private string searchTitle = string.Empty;

        public string SearchTitle
        {
            get { return searchTitle; }
            set { SetProperty(ref searchTitle, value); }
        }

        public INavigation Navigation { get; set; }

        public Command RequestByAuthorCommand
        {
            get { return new Command(async () => await RequestByAuthor()); }
        }

        private ICommand searchTitleCommand;

        public ICommand SearchTitleCommand => searchTitleCommand ?? (searchTitleCommand = new Command<string>((text) =>
        {
            if (text.Length >= 1)
            {
                SongDatabaseToAdd.Clear();
                SongDatabaseObservable.Clear();
                SongDatabaseToAdd.AddRange(VDJAppSingletonInstance.SongList.Where(i => i.Title.ToLower().Contains(text.ToLower())));
                for(int i = 0; i < SongDatabaseToAdd.Count; i++)
                {
                    SongDatabaseObservable.Add(SongDatabaseToAdd[i]);
                }
            }
            else
            {
                SongDatabaseToAdd.Clear();
                SongDatabaseObservable.Clear();
                SongDatabaseToAdd.AddRange(VDJAppSingletonInstance.SongList);
                for (int i = 0; i < SongDatabaseToAdd.Count; i++)
                {
                    SongDatabaseObservable.Add(SongDatabaseToAdd[i]);
                }
            }
        }));

        public RequestSongViewModel(INavigation navigation = null)
        {
            Title = "All songs";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            SongDatabaseToAdd = new List<Song>();
            SongDatabaseObservable = new ObservableCollection<Song>(VDJAppSingletonInstance.SongList);
            Navigation = navigation;
            MessagingCenter.Subscribe<object, object>(this, "TappedSongRequest", async (sender, args) =>
            {
                await Navigation.PushAsync(new SongSelectedRequestPage(new SongSelectedRequestViewModel((Song)args, Navigation)));
            });
        }

        public async Task RequestByAuthor()
        {
            await Navigation.PushAsync(new RequestByAuthorPage());
        }
    }
}
