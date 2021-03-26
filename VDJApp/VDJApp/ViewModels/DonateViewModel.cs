using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class DonateViewModel : BaseViewModel
    {
        public ICommand OpenWebCommand { get; }

        public string PayPalUrl = "https://www.sandbox.paypal.com";

        public DonateViewModel()
        {
            Title = "Donate";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync(PayPalUrl));
        }
    }
}