using System.ComponentModel;
using Xamarin.Forms;
using VDJApp.ViewModels;

namespace VDJApp.Views
{
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            BindingContext = new MenuViewModel(Navigation);
        }
    }
}