using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestSongPage : ContentPage
    {
        public RequestSongPage()
        {
            InitializeComponent();

            BindingContext = new RequestSongViewModel(Navigation);
        }
    }
}