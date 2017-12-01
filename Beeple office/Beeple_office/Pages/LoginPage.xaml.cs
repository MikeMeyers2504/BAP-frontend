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
    public partial class LoginPage
    {
        public ToolbarItem SettingsIos { get; set; }
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginPageViewModel();
            NavigationPage.SetHasBackButton(this, false);
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
            Title = AppResources.TitleLoginPage;
            WelcomeLabel.Text = AppResources.LoginPageWelcome;
            Username.Placeholder = AppResources.StringEmail;
            Password.Placeholder = AppResources.StringPassword;
            LoginButton.Text = AppResources.StringLogin;
            ForgotButton.Text = AppResources.LoginPageForgotPassword;
            RegisterButton.Text = AppResources.StringRegister;
            base.OnAppearing();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.LoginAmountDisplays < 1)
            {
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
                AmountOfDisplaysShown.LoginAmountDisplays++;
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.LoginAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<LoginPageViewModel, bool>(this,
               Constants.MessagingCenter.LoginPage.LoginFailed, (sender, loggedIn) => FailedToLogin(loggedIn));
            MessagingCenter.Subscribe<LoginPageViewModel>(this,
               Constants.MessagingCenter.LoginPage.NavigateToRegisterPage, send => NavigateToRegister());
            MessagingCenter.Subscribe<LoginPageViewModel>(this,
               Constants.MessagingCenter.LoginPage.NavigateToMenuPage, send => NavigateToMenu());
            MessagingCenter.Subscribe<LoginPageViewModel>(this,
               Constants.MessagingCenter.LoginPage.NoLoginData, send => ShowAlertNoData());
            MessagingCenter.Subscribe<LoginPageViewModel>(this,
               Constants.MessagingCenter.LoginPage.NavigateToForgotPasswordPage, send => NavigateToForgotPassword());
            MessagingCenter.Subscribe<LoginPageViewModel>(this,
               Constants.MessagingCenter.LoginPage.GoneWrong, send => ShowAlertLogin());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<LoginPageViewModel>(this, Constants.MessagingCenter.LoginPage.NavigateToRegisterPage);
            MessagingCenter.Unsubscribe<LoginPageViewModel, bool>(this, Constants.MessagingCenter.LoginPage.LoginFailed);
            MessagingCenter.Unsubscribe<LoginPageViewModel>(this, Constants.MessagingCenter.LoginPage.NavigateToMenuPage);
            MessagingCenter.Unsubscribe<LoginPageViewModel>(this, Constants.MessagingCenter.LoginPage.NoLoginData);
            MessagingCenter.Unsubscribe<LoginPageViewModel>(this, Constants.MessagingCenter.LoginPage.NavigateToForgotPasswordPage);
            MessagingCenter.Unsubscribe<LoginPageViewModel>(this, Constants.MessagingCenter.LoginPage.GoneWrong);
        }

        private async void NavigateToRegister()
        {
            Username.Text = null;
            Password.Text = null;
            await Navigation.PushAsync(new RegisterPage());
        }

        private async void NavigateToForgotPassword()
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
        }

        private async void FailedToLogin(bool loggedIn)
        {
            if (!loggedIn)
            {
                Username.Text = null;
                Password.Text = null;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.AlertSentenceLoginPageRight,
                    AppResources.AlertOk);
            }
        }
        private async void ShowAlertLogin()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceLoginPageWrong,
                 AppResources.AlertOk);
        }
        private async void NavigateToMenu()
        {
            await Navigation.PushAsync(new MenuPage());
            Navigation.RemovePage(this);
        }

        private async void ShowAlertNoData()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceLoginPageFill,
                 AppResources.AlertOk);
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
