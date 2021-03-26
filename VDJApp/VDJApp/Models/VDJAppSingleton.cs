using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VDJApp.Models
{
    public sealed class VDJAppSingleton
    {
        private static VDJAppSingleton instance;

        private static readonly object padlock = new object();

        public HttpClient HttpClientSi { get; set; }

        public List<Song> SongList { get; set; }

        public int RequestNumber { get; set; }

        public int ChatNumber { get; set; }

        public string Username { get; set; }

        public string VotedSongs { get; set; }

        private VDJAppSingleton()
        {

        }
          
        public static VDJAppSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new VDJAppSingleton();

                            instance.HttpClientSi = new HttpClient();
                            instance.SongList = new List<Song>();
                            instance.RequestNumber = 0;
                            instance.ChatNumber = 0;
                            instance.Username = "";
                            instance.VotedSongs = "";

                            instance.HttpClientSi.BaseAddress = new Uri("https://azure.net/");
                            instance.HttpClientSi.DefaultRequestHeaders.Accept.Clear();
                            instance.HttpClientSi.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        }
                    }
                }
                return instance;
            }
        }

        public void ClearInstance()
        {
            Instance.SongList = new List<Song>();
            Instance.RequestNumber = 0;
            Instance.ChatNumber = 0;
            Instance.Username = "";
            Instance.VotedSongs = "";
        }

        public int ReloadInstance(string instanceString)
        {
            string[] separator = new string[] { "($||$)" };
            string[] splitInstanceString = instanceString.Split(separator, StringSplitOptions.None);

            DateTime loginTime = Convert.ToDateTime(splitInstanceString[0]);
            loginTime = loginTime.AddHours(12);

            int isAllowedToLogin = DateTime.Compare(loginTime, DateTime.Now);

            if(isAllowedToLogin == -1)
            {
                ClearInstance();
                return -1;
            }
            else
            {
                Instance.SongList = new List<Song>();
                Instance.Username = splitInstanceString[1];
                Instance.RequestNumber = int.Parse(splitInstanceString[2]);
                if(splitInstanceString[3] != "")
                {
                    splitInstanceString[3] = splitInstanceString[3].Remove(splitInstanceString[3].Length - 1);
                    Instance.VotedSongs = splitInstanceString[3];
                }
                return 1;
            }
        }

        public void LoadVotes()
        {
            string[] votedSongsId = Instance.VotedSongs.Split(',');

            if(votedSongsId[0] == "")
            {

            }
            else
            {
                for (int i = 0; i < votedSongsId.Length; i++)
                {
                    int songId = int.Parse(votedSongsId[i]);
                    Instance.SongList.Find(x => x.Id == songId).WasVoted = true;
                }
            }
        }

        public async Task LoadSongDatabase()
        {
            try
            {
                HttpResponseMessage songDatabaseResponse = await Instance.HttpClientSi.GetAsync("api/songs/all");

                if (songDatabaseResponse.IsSuccessStatusCode)
                {
                    string jsonSongList = await songDatabaseResponse.Content.ReadAsStringAsync();
                    Instance.SongList = JsonConvert.DeserializeObject<List<Song>>(jsonSongList);
                    Instance.SongList.Sort((p, q) => p.Title.CompareTo(q.Title));

                    for (int i = 0; i < Instance.SongList.Count; i++)
                    {
                        int leadArtistNumber = Instance.SongList[i].LeadAuthor.Split(',').Length - 1;
                        if (leadArtistNumber == 0)
                        {
                            Instance.SongList[i].LeadAuthorList.Add(Instance.SongList[i].LeadAuthor);
                        }
                        else
                        {
                            string tmpLeadAuthorString = Instance.SongList[i].LeadAuthor;
                            for (int i1 = 0; i1 < leadArtistNumber; i1++)
                            {
                                int tmpIndexOf = tmpLeadAuthorString.IndexOf(",");
                                string leadArtistToList = tmpLeadAuthorString.Substring(0, tmpIndexOf);
                                Instance.SongList[i].LeadAuthorList.Add(leadArtistToList);
                                tmpLeadAuthorString = tmpLeadAuthorString.Remove(0, tmpIndexOf + 2);
                            }
                            Instance.SongList[i].LeadAuthorList.Add(tmpLeadAuthorString);
                        }

                        if (Instance.SongList[i].FeatureAuthor != "")
                        {
                            int featArtistNumber = Instance.SongList[i].FeatureAuthor.Split(',').Length - 1;
                            if (featArtistNumber == 0)
                            {
                                Instance.SongList[i].FeatureAuthorList.Add(Instance.SongList[i].FeatureAuthor);
                            }
                            else
                            {
                                string tmpFeatAuthorString = Instance.SongList[i].FeatureAuthor;
                                for (int i1 = 0; i1 < featArtistNumber; i1++)
                                {
                                    int tmpFIndexOf = tmpFeatAuthorString.IndexOf(",");
                                    string featArtistToList = tmpFeatAuthorString.Substring(0, tmpFIndexOf);
                                    Instance.SongList[i].FeatureAuthorList.Add(featArtistToList);
                                    tmpFeatAuthorString = tmpFeatAuthorString.Remove(0, tmpFIndexOf + 2);
                                }
                                Instance.SongList[i].FeatureAuthorList.Add(tmpFeatAuthorString);
                            }
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Unable to connect to server", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
            }
        }
    }
}
