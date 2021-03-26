using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopSongsPage : ContentPage
    {
        public TopSongsPage()
        {
            InitializeComponent();

            BindingContext = new TopSongsViewModel(Navigation);
        }
    }
}