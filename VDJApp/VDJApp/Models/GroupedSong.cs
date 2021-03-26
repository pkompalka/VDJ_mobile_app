using System.Collections.ObjectModel;

namespace VDJApp.Models
{
    public class GroupedSong : ObservableCollection<Song>
    {
        public string NameOfGroup { get; set; }
    }
}
