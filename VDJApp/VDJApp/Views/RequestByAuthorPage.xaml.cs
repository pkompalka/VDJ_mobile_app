using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequestByAuthorPage : ContentPage
    {
        public RequestByAuthorPage()
        {
            InitializeComponent();

            BindingContext = new RequestByAuthorViewModel(Navigation);
        }
    }
}