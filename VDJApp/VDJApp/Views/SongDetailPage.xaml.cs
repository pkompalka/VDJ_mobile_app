using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongDetailPage : ContentPage
    {
        public SongDetailPage(SongDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}