using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beeple_office.ViewModels;
using Xamarin.Forms;
using Beeple_office.Utils;
using Plugin.Connectivity;

namespace Beeple_office.Pages
{
    public partial class AllOrdersOffSandwichesPage : ContentPage
    {
        public AllOrdersOffSandwichesPage()
        {
            InitializeComponent();
            BindingContext = new AllOrdersOffSandwichesPageViewModel();
        }

        protected override async void OnAppearing()
        {
            Title = AppResources.TitleSandwichesOrdersPage;
            EntryFilter.Placeholder = AppResources.StringFilterEmail;
            LabelRefreshList.Text = AppResources.SentenceRefreshListToNormal;
            FilterButton.Text = AppResources.StringFilterOnEmail;
            NormalButton.Text = AppResources.StringNormalList;
            DeleteButton.Text = AppResources.StringDeleteAllSandwiches;
            LabelSmosInfo.Text = AppResources.StringSentenceSmos;
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
            MessagingCenter.Subscribe<AllOrdersOffSandwichesPageViewModel>(this,
               Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong, send => ShowAlert());
            MessagingCenter.Subscribe<AllOrdersOffSandwichesPageViewModel>(this,
               Constants.MessagingCenter.AllOrdersOffSandwichesPage.DeleteAll, send => ShowDeletedOk());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<AllOrdersOffSandwichesPageViewModel>(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
            MessagingCenter.Unsubscribe<AllOrdersOffSandwichesPageViewModel>(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.DeleteAll);
        }

        private async void ShowAlert()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceAllSandwichesPageWrong,
                 AppResources.AlertOk);
        }
        private async void ShowDeletedOk()
        {
            await DisplayAlert(AppResources.AlertTitleMessage,
                 AppResources.AlertSentenceAllSandwichesPageSuccess,
                 AppResources.AlertOk);
            await Navigation.PushAsync(new MenuPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
        }
    }
}
