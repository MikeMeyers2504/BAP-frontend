using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Beeple_office.ViewModels;
using Beeple_office.Utils;
using Xamarin.Forms;
using Beeple_office.Pages;
using Plugin.Connectivity;

namespace Beeple_office.Pages
{
    public partial class RegisterPage
    {
        public bool Show;
        public ToolbarItem SettingsIos { get; set; }
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterPageViewModel();

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
            Title = AppResources.StringRegister;
            LastName.Placeholder = AppResources.RegisterPageLast;
            FirstName.Placeholder = AppResources.RegisterPageFirst;
            Email.Placeholder = AppResources.StringEmail;
            Password.Placeholder = AppResources.StringPassword;
            PasswordCheck.Placeholder = AppResources.StringConfirmPassword;
            QuestionOne.Text = "1. " + AppResources.SentenceFavoriteColorAnimal;
            QuestionTwo.Text = "2. " + AppResources.SentenceToy;
            SecretOne.Placeholder = AppResources.StringAnswerQuestionOne;
            SecretTwo.Placeholder = AppResources.StringAnswerQuestionTwo;
            RegisterButton.Text = AppResources.StringRegister;
            ShowOrHideButton.Text = AppResources.StringShowHide;
            base.OnAppearing();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.RegisterAmountDisplays < 1)
            {
                AmountOfDisplaysShown.RegisterAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.RegisterAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<RegisterPageViewModel, bool>(this,
               Constants.MessagingCenter.RegisterPage.NavigateToLoginPage, (sender, added) => NavigateToLogin(added));
            MessagingCenter.Subscribe<RegisterPageViewModel>(this,
               Constants.MessagingCenter.RegisterPage.PasswordDoesnotMatch, send => ClearPasswords());
            MessagingCenter.Subscribe<RegisterPageViewModel>(this,
               Constants.MessagingCenter.RegisterPage.EmptyFields, send => EmptyFields());
            MessagingCenter.Subscribe<RegisterPageViewModel>(this,
               Constants.MessagingCenter.RegisterPage.ShowHide, send => ShowHideText());
            MessagingCenter.Subscribe<RegisterPageViewModel>(this,
               Constants.MessagingCenter.RegisterPage.GoneWrong, send => ShowAlertRegister());
            MessagingCenter.Subscribe<RegisterPageViewModel>(this,
               Constants.MessagingCenter.RegisterPage.EmailValidate, send => ShowAlertValidEmail());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<RegisterPageViewModel, bool>(this, Constants.MessagingCenter.RegisterPage.NavigateToLoginPage);
            MessagingCenter.Unsubscribe<RegisterPageViewModel>(this, Constants.MessagingCenter.RegisterPage.PasswordDoesnotMatch);
            MessagingCenter.Unsubscribe<RegisterPageViewModel>(this, Constants.MessagingCenter.RegisterPage.EmptyFields);
            MessagingCenter.Unsubscribe<RegisterPageViewModel>(this, Constants.MessagingCenter.RegisterPage.ShowHide);
            MessagingCenter.Unsubscribe<RegisterPageViewModel>(this, Constants.MessagingCenter.RegisterPage.GoneWrong);
            MessagingCenter.Unsubscribe<RegisterPageViewModel>(this, Constants.MessagingCenter.RegisterPage.EmailValidate);
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

        private async void NavigateToLogin(bool added)
        {
            if (added)
            {
                LastName.Text = null;
                FirstName.Text = null;
                Email.Text = null;
                Password.Text = null;
                PasswordCheck.Text = null;
                SecretOne.Text = null;
                SecretTwo.Text = null;
                await DisplayAlert(AppResources.AlertTitleAccount,
                    AppResources.AlertSentenceRegisterPageCreated,
                    AppResources.AlertOk);
                await Navigation.PushAsync(new LoginPage());
                Navigation.RemovePage(this);
            }
            else
            {
                Email.Text = null;
                await DisplayAlert(AppResources.AlertTitleAccountDuplicate,
                    AppResources.AlertSentenceRegisterPageTaken,
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
        private async void ShowAlertRegister()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceRegisterPageWrong,
                 AppResources.AlertOk);
        }
        private async void EmptyFields()
        {
            await DisplayAlert(AppResources.AlertTitleEmpty,
                AppResources.AlertSentenceRegisterPageEmpty,
                AppResources.AlertOk);
        }
        private async void ShowAlertValidEmail()
        {
            Email.Text = null;
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceRegisterPageValidate,
                 AppResources.AlertOk);
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
