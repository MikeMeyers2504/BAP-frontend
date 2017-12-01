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
    public partial class FilterRoomsPage : ContentPage
    {
        public FilterRoomsPage()
        {
            InitializeComponent();
            BindingContext = new FilterRoomsPageViewModel();
            DatePickerRoom.MinimumDate = DateTime.Now;
            TimePickerEnd.Time = default(TimeSpan);
            TimePickerStart.Time = default(TimeSpan);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Title = AppResources.TitleFilterRoomsPage;
            LabelRoom.Text = AppResources.StringRoom;
            PickTime.Text = AppResources.SentenceRoomsFilterPage;
            FilterRoomButton.Text = AppResources.StringFilterRoomDate;
            FilterTimeButton.Text = AppResources.StringFilterTimeDate;
            NewReservationButton.Text = AppResources.TitleNewRoomPage;
            Subscribe();
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.RoomsAmountDisplays < 1)
            {
                AmountOfDisplaysShown.RoomsAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.RoomsAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<FilterRoomsPageViewModel>(this,
               Constants.MessagingCenter.FilterRoomsPage.NavigateToNewRoom, send => NavigateToNewRoom());
            MessagingCenter.Subscribe<FilterRoomsPageViewModel>(this,
               Constants.MessagingCenter.FilterRoomsPage.Error, send => Error());
            MessagingCenter.Subscribe<FilterRoomsPageViewModel>(this,
               Constants.MessagingCenter.FilterRoomsPage.UnselectRow, send => Unselect());
            MessagingCenter.Subscribe<FilterRoomsPageViewModel>(this,
               Constants.MessagingCenter.FilterRoomsPage.Nothing, send => MessageNothing());
            MessagingCenter.Subscribe<FilterRoomsPageViewModel>(this,
               Constants.MessagingCenter.FilterRoomsPage.GoneWrong, send => ShowAlertRooms());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<FilterRoomsPageViewModel>(this, Constants.MessagingCenter.FilterRoomsPage.NavigateToNewRoom);
            MessagingCenter.Unsubscribe<FilterRoomsPageViewModel>(this, Constants.MessagingCenter.FilterRoomsPage.Error);
            MessagingCenter.Unsubscribe<FilterRoomsPageViewModel>(this, Constants.MessagingCenter.FilterRoomsPage.UnselectRow);
            MessagingCenter.Unsubscribe<FilterRoomsPageViewModel>(this, Constants.MessagingCenter.FilterRoomsPage.Nothing);
            MessagingCenter.Unsubscribe<FilterRoomsPageViewModel>(this, Constants.MessagingCenter.FilterRoomsPage.GoneWrong);
        }
        private async void ShowAlertRooms()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceFilterRoomsWrong,
                 AppResources.AlertOk);
        }
        private async void NavigateToNewRoom()
        {
            await Navigation.PushAsync(new NewRoomReservationPage());
        }

        private void Unselect()
        {
            ListNames.SelectedItem = null;
        }

        private async void Error()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceFilterRoomsCheck +
                 AppResources.AlertSentenceFilterRoomsDefault +
                 AppResources.AlertSentenceFilterRoomsLater +
                 AppResources.AlertSentenceFilterRoomsAboveZero,
                 AppResources.AlertOk);
        }

        private async void MessageNothing()
        {
            await DisplayAlert(AppResources.AlertTitleMessage,
                 AppResources.AlertSentenceFilterRoomsNoReservations,
                 AppResources.AlertOk);
        }

        private void ListFilteredRooms_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListFilteredRooms.SelectedItem = null;
        }
        private void ListFilteredTime_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListFilteredTime.SelectedItem = null;
        }
    }
}
