using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongSelectedRequestPage : ContentPage
    {
        public SongSelectedRequestPage(SongSelectedRequestViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}