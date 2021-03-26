using VDJApp.Models;
using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DedicationPage : ContentPage
    {
        public DedicationPage(Song song)
        {
            InitializeComponent();

            BindingContext = new DedicationViewModel(song, Navigation);
        }
    }
}