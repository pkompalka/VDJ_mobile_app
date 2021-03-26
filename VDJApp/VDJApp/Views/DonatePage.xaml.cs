using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DonatePage : ContentPage
    {
        public DonatePage()
        {
            InitializeComponent();

            BindingContext = new DonateViewModel();
        }
    }
}