using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VDJApp.Models;
using VDJApp.Views;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class RequestByAuthorViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public ObservableCollection<Song> SongDatabaseObservable { get; set; }

        public List<GroupedSong> GroupedSongDatabaseList { get; set; }

        public ObservableCollection<GroupedSong> GroupedSongDatabaseObservable { get; set; }

        public List<GroupedSong> GroupedSongDatabaseToAdd { get; set; }

        public INavigation Navigation { get; set; }

        private ICommand searchAuthorCommand;

        public ICommand SearchAuthorCommand => searchAuthorCommand ?? (searchAuthorCommand = new Command<string>((text) =>
        {
            if (text.Length >= 1)
            {
                GroupedSongDatabaseToAdd.Clear();
                GroupedSongDatabaseObservable.Clear();
                GroupedSongDatabaseToAdd.AddRange(GroupedSongDatabaseList.Where(i => i.NameOfGroup.ToLower().Contains(text.ToLower())));
                for (int i = 0; i < GroupedSongDatabaseToAdd.Count; i++)
                {
                    GroupedSongDatabaseObservable.Add(GroupedSongDatabaseToAdd[i]);
                }
            }
            else
            {
                GroupedSongDatabaseToAdd.Clear();
                GroupedSongDatabaseObservable.Clear();
                GroupedSongDatabaseToAdd.AddRange(GroupedSongDatabaseList);
                for (int i = 0; i < GroupedSongDatabaseToAdd.Count; i++)
                {
                    GroupedSongDatabaseObservable.Add(GroupedSongDatabaseToAdd[i]);
                }
            }
        }));

        public RequestByAuthorViewModel(INavigation navigation = null)
        {
            Title = "All songs";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            SongDatabaseObservable = new ObservableCollection<Song>(VDJAppSingletonInstance.SongList);
            GroupedSongDatabaseList = new List<GroupedSong>();
            GroupedSongDatabaseObservable = new ObservableCollection<GroupedSong>();
            GroupedSongDatabaseToAdd = new List<GroupedSong>();
            GroupDatabase();
            Navigation = navigation;
            MessagingCenter.Subscribe<object, object>(this, "TappedSongRequestAuthor", async (sender, args) =>
            {
                await Navigation.PushAsync(new SongSelectedRequestPage(new SongSelectedRequestViewModel((Song)args, Navigation)));
            });
        }

        public void GroupDatabase()
        {
            for(int i = 0; i < SongDatabaseObservable.Count; i++)
            {
                AddSongToGroup(SongDatabaseObservable[i], SongDatabaseObservable[i].LeadAuthorList);
                AddSongToGroup(SongDatabaseObservable[i], SongDatabaseObservable[i].FeatureAuthorList);
            }

            GroupedSongDatabaseList.Sort((p, q) => p.NameOfGroup.CompareTo(q.NameOfGroup));

            for (int i1 = 0; i1 < GroupedSongDatabaseList.Count; i1++)
            {
                GroupedSongDatabaseObservable.Add(GroupedSongDatabaseList[i1]);
            }
        }

        public void AddSongToGroup(Song songToAdd, List<string> authorList)
        {
            for (int i = 0; i < authorList.Count; i++)
            {
                bool isAuthorUniqueFlag = false;

                for (int i1 = 0; i1 < GroupedSongDatabaseList.Count; i1++)
                {
                    if (GroupedSongDatabaseList[i1].NameOfGroup == authorList[i])
                    {
                        GroupedSongDatabaseList[i1].Add(songToAdd);
                        isAuthorUniqueFlag = true;
                        break;
                    }
                }

                if (isAuthorUniqueFlag == false)
                {
                    GroupedSong songGroup = new GroupedSong() { NameOfGroup = authorList[i] };
                    songGroup.Add(songToAdd);
                    GroupedSongDatabaseList.Add(songGroup);
                }
            }
        }
    }
}