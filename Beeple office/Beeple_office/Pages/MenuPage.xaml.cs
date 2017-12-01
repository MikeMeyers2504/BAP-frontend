using Beeple_office.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beeple_office.Pages;
using Beeple_office.ViewModels;
using Xamarin.Forms;
using System.Diagnostics;
using System.Globalization;
using Plugin.Connectivity;
using PropertyChanged;

namespace Beeple_office.Pages
{
    [ImplementPropertyChanged]
    public partial class MenuPage
    {
        public ToolbarItem SettingsIos { get; set; }
        public MenuPage()
        {
            BindingContext = new MenuPageViewModel();
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            SettingsIos = new ToolbarItem();
            if (Device.OS == TargetPlatform.Android)
            {
                ToolbarItems.Add(new ToolbarItem("hamburger", "Resources/drawable/settings.png", async () =>
                {
                    await Navigation.PushAsync(new HamburgerMenu());
                }));
            }
            else if (Device.OS == TargetPlatform.iOS)
            {
                SettingsIos.Text = "+";
                SettingsIos.Command = new Command(() => { NavigateToSettings(); });
                ToolbarItems.Add(SettingsIos);
            }
            else
            {
                Debug.WriteLine("There is no device found");
            }

            Debug.WriteLine(UserLoggedIn.Token);
            Debug.WriteLine(UserLoggedIn.User);
        }
        protected override async void OnAppearing()
        {
            Title = AppResources.TitleMenuPage;
            McButton.Text = AppResources.McButton;
            MeButton.Text = AppResources.MeButton;
            MsButton.Text = AppResources.MsButton;
            MrButton.Text = AppResources.MrButton;
            base.OnAppearing();
            UnSubscribe();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.MenuAmountDisplays < 1)
            {
                AmountOfDisplaysShown.MenuAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.MenuAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<MenuPageViewModel>(this,
               Constants.MessagingCenter.MenuPage.NavigateToCheckPage, send => NavigateToC());
            MessagingCenter.Subscribe<MenuPageViewModel>(this,
               Constants.MessagingCenter.MenuPage.NavigateToSandwichesPage, send => NavigateToS());
            MessagingCenter.Subscribe<MenuPageViewModel>(this,
               Constants.MessagingCenter.MenuPage.NavigateToEventsPage, send => NavigateToE());
            MessagingCenter.Subscribe<MenuPageViewModel>(this,
               Constants.MessagingCenter.MenuPage.NavigateToRoomsPage, send => NavigateToR());
        }
        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<MenuPageViewModel>(this, Constants.MessagingCenter.MenuPage.NavigateToCheckPage);
            MessagingCenter.Unsubscribe<MenuPageViewModel>(this, Constants.MessagingCenter.MenuPage.NavigateToEventsPage);
            MessagingCenter.Unsubscribe<MenuPageViewModel>(this, Constants.MessagingCenter.MenuPage.NavigateToRoomsPage);
            MessagingCenter.Unsubscribe<MenuPageViewModel>(this, Constants.MessagingCenter.MenuPage.NavigateToSandwichesPage);
        }

        private async void NavigateToC()
        {
            await Navigation.PushAsync(new CheckinCheckoutPage());
        }
        private async void NavigateToE()
        {
            await Navigation.PushAsync(new EventsPage());
        }
        private async void NavigateToR()
        {
            await Navigation.PushAsync(new RoomsPage());
        }
        private async void NavigateToS()
        {
            await Navigation.PushAsync(new OrderSandwichesPage());
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
