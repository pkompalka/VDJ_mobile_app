using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace VDJApp.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Album { get; set; } = "";

        public string LeadAuthor { get; set; }

        public string FeatureAuthor { get; set; }

        public string FullAuthor { get; set; } = "";

        public List<string> LeadAuthorList { get; set; }

        public List<string> FeatureAuthorList { get; set; }

        public int Score { get; set; }

        public string Cover { get; set; } = "";

        public bool WasVoted { get; set; } = false;

        public string ColorPlayed { get; set; } = "#FF1A1818";

        public ImageSource CoverSource { get; set; }

        public int ImageHeight { get; set; }

        public ICommand SongTappedVotedCommand { get; set; }

        public ICommand SongTappedPlaylistCommand { get; set; }

        public ICommand SongTappedRequestCommand { get; set; }

        public ICommand SongTappedRequestAuthorCommand { get; set; }

        public Song()
        {
            LeadAuthorList = new List<string>();
            FeatureAuthorList = new List<string>();
            ImageHeight = (int)(Application.Current.MainPage.Height * 0.2);
            SongTappedVotedCommand = new Command(SongTappedVoted);
            SongTappedPlaylistCommand = new Command(SongTappedPlaylist);
            SongTappedRequestCommand = new Command(SongTappedRequest);
            SongTappedRequestAuthorCommand = new Command(SongTappedRequestAuthor);
        }

        private void SongTappedVoted(object s)
        {
            MessagingCenter.Send<object, object>(this, "TappedSongVoted", s);
        }

        private void SongTappedPlaylist(object s)
        {
            MessagingCenter.Send<object, object>(this, "TappedSongPlaylist", s);
        }

        private void SongTappedRequest(object s)
        {
            MessagingCenter.Send<object, object>(this, "TappedSongRequest", s);
        }

        private void SongTappedRequestAuthor(object s)
        {
            MessagingCenter.Send<object, object>(this, "TappedSongRequestAuthor", s);
        }
    }
}
