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
    public partial class NewEventPage : ContentPage
    {
        public NewEventPage()
        {
            InitializeComponent();
            BindingContext = new NewEventPageViewModel();
            DatePickerRoom.MinimumDate = DateTime.Now;
            TimePickerEnd.Time = default(TimeSpan);
            TimePickerStart.Time = default(TimeSpan);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Subscribe();
            Title = AppResources.StringNewEvent;
            LabelSort.Text = AppResources.StringSortEvent;
            LabelWhere.Text = AppResources.StringWhere;
            LabelReq.Text = AppResources.StringRequirements;
            LabelNameEvent.Text = AppResources.StringNameEvent;
            LabelTProvided.Text = AppResources.StringTProvided;
            LabelFdProvided.Text = AppResources.StringFDProvided;
            LabelWhithWho.Text = AppResources.StringChooseWho;
            SubmitButton.Text = AppResources.StringSubmit;
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
            MessagingCenter.Subscribe<NewEventPageViewModel>(this,
               Constants.MessagingCenter.NewEventPage.NewEvent, send => ShowReport());
            MessagingCenter.Subscribe<NewEventPageViewModel>(this,
               Constants.MessagingCenter.NewEventPage.TimeSmaller, send => ShowError());
            MessagingCenter.Subscribe<NewEventPageViewModel>(this,
               Constants.MessagingCenter.NewEventPage.EventError, send => ErrorOrder());
            MessagingCenter.Subscribe<NewEventPageViewModel>(this,
               Constants.MessagingCenter.NewEventPage.GoneWrong, send => ShowAlertNewEvent());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<NewEventPageViewModel>(this, Constants.MessagingCenter.NewEventPage.NewEvent);
            MessagingCenter.Unsubscribe<NewEventPageViewModel>(this, Constants.MessagingCenter.NewEventPage.TimeSmaller);
            MessagingCenter.Unsubscribe<NewEventPageViewModel>(this, Constants.MessagingCenter.NewEventPage.EventError);
            MessagingCenter.Unsubscribe<NewEventPageViewModel>(this, Constants.MessagingCenter.NewEventPage.GoneWrong);
        }
        private async void ShowAlertNewEvent()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceNewEventWrong,
                 AppResources.AlertOk);
        }
        private void Names_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Names.SelectedItem = null;
        }
        private async void ShowReport()
        {
            await DisplayAlert(AppResources.AlertTitleSuccess,
                 AppResources.AlertSentenceNewEventSuccessfully,
                 AppResources.AlertOk);
            await Navigation.PushAsync(new MenuPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
        }

        private async void ShowError()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceNewRoomReservationStartTimeBigger +
                 AppResources.AlertSentenceNewEventFilled,
                 AppResources.AlertOk);
        }

        private async void ErrorOrder()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceNewEventAlreadyEvent,
                 AppResources.AlertOk);
        }
    }
}
