using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beeple_office.Pages;
using Beeple_office.ViewModels;
using Xamarin.Forms;
using Beeple_office.Utils;
using Plugin.Connectivity;
using PropertyChanged;

namespace Beeple_office.Pages
{
    [ImplementPropertyChanged]
    public partial class HamburgerMenu: ContentPage
    {
        public bool LoggedOut;
        public HamburgerMenu()
        {
            BindingContext = new HamburgerViewModel();
            Title = AppResources.TitleSettingsPage;
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            LogoutButton.Text = AppResources.SettingsPageLogout;
            EngButton.Text = AppResources.English;
            DuButton.Text = AppResources.Dutch;
            FrButton.Text = AppResources.French;
            base.OnAppearing();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.HambugerMenuAmountDisplays < 1)
            {
                AmountOfDisplaysShown.HambugerMenuAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }
        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.HambugerMenuAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }
        private void Subscribe()
        {
            MessagingCenter.Subscribe<HamburgerViewModel>(this,
               Constants.MessagingCenter.HamburgerMenu.Logout, send => Logout());
            MessagingCenter.Subscribe<HamburgerViewModel>(this,
               Constants.MessagingCenter.HamburgerMenu.LanguageChanged, send => Refresh());
        }
        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<HamburgerViewModel>(this, Constants.MessagingCenter.HamburgerMenu.Logout);
            MessagingCenter.Unsubscribe<HamburgerViewModel>(this, Constants.MessagingCenter.HamburgerMenu.LanguageChanged);
        }

        private async void Logout()
        {
            LoggedOut = true;
            UserLoggedIn.Token = null;
            UserLoggedIn.User = null; 
            Debug.WriteLine(UserLoggedIn.Token);
            Debug.WriteLine(UserLoggedIn.User);
            await Navigation.PushAsync(new LoginPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
        }
        private void Refresh()
        {
            if (UserLoggedIn.User != null)
            {
                Navigation.PushAsync(new MenuPage());
                foreach (var page in Navigation.NavigationStack.ToList())
                {
                    Navigation.RemovePage(page);
                }
            }
            else
            {
                Navigation.PushAsync(new LoginPage());
                foreach (var page in Navigation.NavigationStack.ToList())
                {
                    Navigation.RemovePage(page);
                }
            }
        }
    }
}
