using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaylistPage : ContentPage
    {
        public PlaylistPage()
        {
            InitializeComponent();

            BindingContext = new PlaylistViewModel(Navigation);
        }
    }
}