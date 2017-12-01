using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beeple_office.ViewModels;
using Xamarin.Forms;
using Beeple_office.Utils;
using Plugin.Connectivity;
using System.Diagnostics;

namespace Beeple_office.Pages
{
    public partial class ForgotPasswordPage
    {
        public bool Show;
        public ToolbarItem SettingsIos { get; set; }
        public ForgotPasswordPage()
        {
            InitializeComponent();
            BindingContext = new ForgotPasswordPageViewModel();

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
            Title = AppResources.TitlePasswordPage;
            Email.Placeholder = AppResources.StringEmail;
            Password.Placeholder = AppResources.StringPassword;
            PasswordCheck.Placeholder = AppResources.StringConfirmPassword;
            QuestionOne.Text = "1. " + AppResources.SentenceFavoriteColorAnimal;
            QuestionTwo.Text = "2. " + AppResources.SentenceToy;
            SecretOne.Placeholder = AppResources.StringAnswerQuestionOne;
            SecretTwo.Placeholder = AppResources.StringAnswerQuestionTwo;
            ChangeButton.Text = AppResources.TitlePasswordPage;
            ShowHideButton.Text = AppResources.StringShowHide;
            base.OnAppearing();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.ForgotPasswordAmountDisplays < 1)
            {
                AmountOfDisplaysShown.ForgotPasswordAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }
        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.ForgotPasswordAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }
        private void Subscribe()
        {
            MessagingCenter.Subscribe<ForgotPasswordPageViewModel, bool>(this,
               Constants.MessagingCenter.ForgotPasswordPage.NavigateToLoginPage, (sender, changed) => NavigateToLogin(changed));
            MessagingCenter.Subscribe<ForgotPasswordPageViewModel>(this,
               Constants.MessagingCenter.ForgotPasswordPage.PasswordDoesnotMatch, send => ClearPasswords());
            MessagingCenter.Subscribe<ForgotPasswordPageViewModel>(this,
               Constants.MessagingCenter.ForgotPasswordPage.EmptyFields, send => EmptyFields());
            MessagingCenter.Subscribe<ForgotPasswordPageViewModel>(this,
               Constants.MessagingCenter.ForgotPasswordPage.ShowHide, send => ShowHideText());
            MessagingCenter.Subscribe<ForgotPasswordPageViewModel, bool>(this,
               Constants.MessagingCenter.ForgotPasswordPage.Error, (sender, changed) => ShowError(changed));
            MessagingCenter.Subscribe<ForgotPasswordPageViewModel>(this,
               Constants.MessagingCenter.ForgotPasswordPage.GoneWrong, send => ShowAlertPassword());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<ForgotPasswordPageViewModel, bool>(this, Constants.MessagingCenter.ForgotPasswordPage.NavigateToLoginPage);
            MessagingCenter.Unsubscribe<ForgotPasswordPageViewModel, bool>(this, Constants.MessagingCenter.ForgotPasswordPage.Error);
            MessagingCenter.Unsubscribe<ForgotPasswordPageViewModel>(this, Constants.MessagingCenter.ForgotPasswordPage.PasswordDoesnotMatch);
            MessagingCenter.Unsubscribe<ForgotPasswordPageViewModel>(this, Constants.MessagingCenter.ForgotPasswordPage.EmptyFields);
            MessagingCenter.Unsubscribe<ForgotPasswordPageViewModel>(this, Constants.MessagingCenter.ForgotPasswordPage.ShowHide);
            MessagingCenter.Unsubscribe<ForgotPasswordPageViewModel>(this, Constants.MessagingCenter.ForgotPasswordPage.GoneWrong);
        }
        private void ShowHideText()
        {
            if (!Show)
            {
                Password.IsPassword = false;
                PasswordCheck.IsPassword = false;
                SecretOne.IsPassword = false;
                SecretTwo.IsPassword = false;
                Show = true;
            }
            else
            {
                Password.IsPassword = true;
                PasswordCheck.IsPassword = true;
                SecretOne.IsPassword = true;
                SecretTwo.IsPassword = true;
                Show = false;
            }
        }
        private async void ShowAlertPassword()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentencePasswordPageWrong,
                 AppResources.AlertOk);
        }
        private async void NavigateToLogin(bool changed)
        {
            if (changed)
            {
                Email.Text = null;
                Password.Text = null;
                PasswordCheck.Text = null;
                SecretOne.Text = null;
                SecretTwo.Text = null;
                await DisplayAlert(AppResources.AlertTitleAccount,
                    AppResources.AlertSentencePasswordPageChanged,
                    AppResources.AlertOk);
                await Navigation.PushAsync(new LoginPage());
                Navigation.RemovePage(this);
            }
        }
        private async void ShowError(bool changed)
        {
            if (!changed)
            {
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.AlertSentencePasswordPageWrong,
                    AppResources.AlertOk);
            }
        }
        private async void ClearPasswords()
        {
            await DisplayAlert(AppResources.AlertTitlePasswordProblem,
                AppResources.AlertSentenceRegisterPageNotMatch,
                AppResources.AlertOk);
            Password.Text = null;
            PasswordCheck.Text = null;
        }

        private async void EmptyFields()
        {
            await DisplayAlert(AppResources.AlertTitleEmpty,
                AppResources.AlertSentencePasswordPageEmpty,
                AppResources.AlertOk);
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
