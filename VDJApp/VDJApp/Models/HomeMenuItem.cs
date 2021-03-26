namespace VDJApp.Models
{
    public enum MenuItemType
    {
        NowPlaying,
        Playlist,
        TopSong,
        RequestSong,
        Chat,
        Donate,
        Info
    }

    public class HomeMenuItem 
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }

    }
}
