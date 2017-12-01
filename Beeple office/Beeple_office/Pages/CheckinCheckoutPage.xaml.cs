using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beeple_office.ViewModels;
using Xamarin.Forms;
using Beeple_office.Utils;
using System.Diagnostics;
using Plugin.Connectivity;

namespace Beeple_office.Pages
{
    public partial class CheckinCheckoutPage : ContentPage
    {
        public ToolbarItem SettingsIos { get; set; }
        public CheckinCheckoutPage()
        {
            InitializeComponent();
            BindingContext = new CheckinCheckoutPageViewModel();
            Debug.WriteLine(UserLoggedIn.Token);
            Debug.WriteLine(UserLoggedIn.User);

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
        }

        protected override async void OnAppearing()
        {
            Title = AppResources.TitleCheckinCheckoutPage;
            ChInButton.Text = AppResources.ChInButton;
            ChUiButton.Text = AppResources.ChOuButton;
            base.OnAppearing();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.CheckinCheckoutAmountDisplays < 1)
            {
                AmountOfDisplaysShown.CheckinCheckoutAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.CheckinCheckoutAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<CheckinCheckoutPageViewModel, bool>(this,
               Constants.MessagingCenter.CheckinCheckoutPage.Checkin, (sender, checkedIn) => CheckinSucces(checkedIn));
            MessagingCenter.Subscribe<CheckinCheckoutPageViewModel, bool>(this,
               Constants.MessagingCenter.CheckinCheckoutPage.Checkout, (sender, checkedOut) => CheckoutSucces(checkedOut));
            MessagingCenter.Subscribe<CheckinCheckoutPageViewModel>(this,
               Constants.MessagingCenter.CheckinCheckoutPage.GoneWrong, send => ShowAlertCheckinCheckout());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<CheckinCheckoutPageViewModel, bool>(this, Constants.MessagingCenter.CheckinCheckoutPage.Checkin);
            MessagingCenter.Unsubscribe<CheckinCheckoutPageViewModel, bool>(this, Constants.MessagingCenter.CheckinCheckoutPage.Checkout);
            MessagingCenter.Unsubscribe<CheckinCheckoutPageViewModel>(this, Constants.MessagingCenter.CheckinCheckoutPage.GoneWrong);
        }

        private async void ShowAlertCheckinCheckout()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceCheckinCheckoutPage,
                 AppResources.AlertOk);
        }

        private async void NavigateToMenu()
        {
            await Navigation.PushAsync(new MenuPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
            //Navigation.RemovePage(this);
            Debug.WriteLine(UserLoggedIn.Token);
            Debug.WriteLine(UserLoggedIn.User);
        }

        private async void CheckinSucces(bool successful)
        {
            if (successful)
            {
                await DisplayAlert(AppResources.AlertTitleSuccess, 
                 AppResources.SuccessfullCheckin,
                 AppResources.AlertOk);
                NavigateToMenu();
            }
        }

        private async void CheckoutSucces(bool successful)
        {
            if (successful)
            {
                await DisplayAlert(AppResources.AlertTitleSuccess,
                 AppResources.SuccessfullCheckout,
                 AppResources.AlertOk);
                NavigateToMenu();
            }          
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
