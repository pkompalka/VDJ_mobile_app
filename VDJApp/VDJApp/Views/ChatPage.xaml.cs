using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();

            BindingContext = new ChatViewModel();

            MessagingCenter.Subscribe<object, object>(this, "ScollList", (sender, args) =>
            {
                SongsListView.ScrollTo(args, ScrollToPosition.End, true);
            });
        }
    }
}