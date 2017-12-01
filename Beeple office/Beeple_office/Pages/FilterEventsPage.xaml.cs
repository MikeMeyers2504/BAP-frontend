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
    public partial class FilterEventsPage : ContentPage
    {
        public FilterEventsPage()
        {
            InitializeComponent();
            BindingContext = new FilterEventsPageViewModel();
            DatePickerEvent.MinimumDate = DateTime.Now;
            TimePickerEnd.Time = default(TimeSpan);
            TimePickerStart.Time = default(TimeSpan);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Subscribe();
            Title = AppResources.TitleFilterEventsPage;
            LabelPickTime.Text = AppResources.SentenceRoomsFilterPage;
            ButtonFilterDate.Text = AppResources.StringFilterDate;
            ButtonFilterTime.Text = AppResources.StringFilterTime;
            ButtonNewEvent.Text = AppResources.StringNewEvent;
            if (!CrossConnectivity.Current.IsConnected && AmountOfDisplaysShown.EventsAmountDisplays < 1)
            {
                AmountOfDisplaysShown.EventsAmountDisplays++;
                await DisplayAlert(AppResources.AlertTitleError,
                    AppResources.InternetConnectionLoss,
                    AppResources.AlertOk);
            }
        }

        protected override void OnDisappearing()
        {
            AmountOfDisplaysShown.EventsAmountDisplays = 0;
            base.OnDisappearing();
            UnSubscribe();
        }
        private void Subscribe()
        {
            MessagingCenter.Subscribe<FilterEventsPageViewModel>(this,
               Constants.MessagingCenter.FilterEventsPage.NavigateToNewEvent, send => NavigateToNewEvent());
            MessagingCenter.Subscribe<FilterEventsPageViewModel>(this,
               Constants.MessagingCenter.FilterEventsPage.Error, send => Error());
            MessagingCenter.Subscribe<FilterEventsPageViewModel>(this,
               Constants.MessagingCenter.FilterEventsPage.Nothing, send => MessageNothing());
            MessagingCenter.Subscribe<FilterEventsPageViewModel>(this,
               Constants.MessagingCenter.FilterEventsPage.GoneWrong, send => ShowAlertFilterEvents());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<FilterEventsPageViewModel>(this, Constants.MessagingCenter.FilterEventsPage.NavigateToNewEvent);
            MessagingCenter.Unsubscribe<FilterEventsPageViewModel>(this, Constants.MessagingCenter.FilterEventsPage.Error);
            MessagingCenter.Unsubscribe<FilterEventsPageViewModel>(this, Constants.MessagingCenter.FilterEventsPage.Nothing);
            MessagingCenter.Unsubscribe<FilterEventsPageViewModel>(this, Constants.MessagingCenter.FilterEventsPage.GoneWrong);
        }
        private async void ShowAlertFilterEvents()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceFilterEventsWrong,
                 AppResources.AlertOk);
        }
        private async void NavigateToNewEvent()
        {
            await Navigation.PushAsync(new NewEventPage());
        }

        private async void Error()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceFilterRoomsDefault +
                 AppResources.AlertSentenceFilterRoomsLater,
                 AppResources.AlertOk);
        }

        private async void MessageNothing()
        {
            await DisplayAlert(AppResources.AlertTitleMessage,
                 AppResources.AlertSentenceFilterEventsNoEvents,
                 AppResources.AlertOk);
        }

        private void ListFilteredEvents_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListFilteredEventsDate.SelectedItem = null;
        }
        private void ListFilteredTime_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListFilteredEventsTime.SelectedItem = null;
        }
    }
}
