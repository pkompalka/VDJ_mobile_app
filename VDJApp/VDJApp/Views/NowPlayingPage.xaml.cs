﻿using VDJApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VDJApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NowPlayingPage : ContentPage
    {
        public NowPlayingPage()
        {
            InitializeComponent();

            BindingContext = new NowPlayingViewModel();
        }
    }
}