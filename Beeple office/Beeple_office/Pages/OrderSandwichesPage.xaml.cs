using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beeple_office.ViewModels;
using Xamarin.Forms;
using System.Diagnostics;
using Beeple_office.Utils;
using Plugin.Connectivity;

namespace Beeple_office.Pages
{
    public partial class OrderSandwichesPage: ContentPage
    {
        public ToolbarItem SettingsIos { get; set; }
        public OrderSandwichesPage()
        {
            InitializeComponent();
            BindingContext = new OrderSandwichesPageViewModel();

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
            Title = AppResources.TitleSandwichesPage;
            LabelSortBread.Text = AppResources.StringSortBread;
            EntrySortBread.Placeholder = AppResources.StringWhiteBrown;
            LabelSandwiches.Text = AppResources.StringWhichSandwiche;
            LabelPriceSauce.Text = AppResources.StringSauceMoney;
            LabelPriceSmos.Text = AppResources.StringSmosMoney;
            LabelAmount.Text = AppResources.StringAmount;
            LabelTotalText.Text = AppResources.StringTotal;
            TotalPriceButton.Text = AppResources.StringCalculateTotalPrice;
            OrderButton.Text = AppResources.StringOrder;
            LabelResponsible.Text = AppResources.SentenceResponsible;
            ResponsibleButton.Text = AppResources.StringResponsible;
            base.OnAppearing();
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.OrderSandwichesAmountDisplays < 1)
            {
                AmountOfDisplaysShown.OrderSandwichesAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.OrderSandwichesAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<OrderSandwichesPageViewModel, bool>(this,
               Constants.MessagingCenter.OrderSandwichesPage.OrderSandwiche, (sender, ordered) => OrderSuccesFull(ordered));
            MessagingCenter.Subscribe<OrderSandwichesPageViewModel>(this,
               Constants.MessagingCenter.OrderSandwichesPage.GoneWrong, send => ShowAlertOrderSandwiche());
            MessagingCenter.Subscribe<OrderSandwichesPageViewModel>(this,
               Constants.MessagingCenter.OrderSandwichesPage.EmptyFields, send => ShowAlertEmptyFields());
            MessagingCenter.Subscribe<OrderSandwichesPageViewModel>(this,
               Constants.MessagingCenter.OrderSandwichesPage.NavigateToAllOrdersPage, send => NavigateToAllOrders());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<OrderSandwichesPageViewModel, bool>(this, Constants.MessagingCenter.OrderSandwichesPage.OrderSandwiche);
            MessagingCenter.Unsubscribe<OrderSandwichesPageViewModel>(this, Constants.MessagingCenter.OrderSandwichesPage.GoneWrong);
            MessagingCenter.Unsubscribe<OrderSandwichesPageViewModel>(this, Constants.MessagingCenter.OrderSandwichesPage.EmptyFields);
            MessagingCenter.Unsubscribe<OrderSandwichesPageViewModel>(this, Constants.MessagingCenter.OrderSandwichesPage.NavigateToAllOrdersPage);
        }

        private async void ShowAlertOrderSandwiche()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceOrderSandwichesPageWrong,
                 AppResources.AlertOk);
        }
        private async void ShowAlertEmptyFields()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceOrderSandwichesPageNotYet,
                 AppResources.AlertOk);
        }

        private async void NavigateToMenu()
        {
            await Navigation.PushAsync(new MenuPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
            Debug.WriteLine(UserLoggedIn.Token);
            Debug.WriteLine(UserLoggedIn.User);
        }

        private async void NavigateToAllOrders()
        {
            if (UserLoggedIn.Responsible)
            {
                await DisplayAlert(AppResources.AlertTitleMessage,
                 AppResources.AlertSentenceOrderSandwichesPageResponsible,
                 AppResources.AlertOk);
                await Navigation.PushAsync(new AllOrdersOffSandwichesPage());
            }
            else
            {
                await DisplayAlert(AppResources.AlertTitleMessage,
                 AppResources.AlertSentenceOrderSandwichesPageResponsibleNot,
                 AppResources.AlertOk);
            }
        }

        private async void OrderSuccesFull(bool successful)
        {
            string total = LabelTotal.Text;
            if (successful)
            {
                await DisplayAlert(AppResources.AlertTitleSuccess,
                 AppResources.AlertSentenceOrderSandwichesPagePartOneSuccessfully + total + AppResources.AlertSentenceOrderSandwichesPagePartTwoSuccessfully,
                 AppResources.AlertOk);
                NavigateToMenu();
            }
            else
            {
                ShowAlertOrderSandwiche();
            }
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}

