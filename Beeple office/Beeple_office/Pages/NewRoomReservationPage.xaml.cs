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
    public partial class NewRoomReservationPage : ContentPage
    {
        public NewRoomReservationPage()
        {
            InitializeComponent();
            BindingContext = new NewRoomsReservationPageViewModel();
            Title = AppResources.TitleNewRoomPage;
            LabelRoom.Text = AppResources.StringRoom;
            EntryRoomName.Placeholder = AppResources.NewRoomPageExampleName;
            AddButton.Text = AppResources.StringAdd;
            LabelMaterial.Text = AppResources.StringMaterial;
            EntryMaterial.Placeholder = AppResources.StringLaptops;
            LabelWhithWho.Text = AppResources.StringChooseWho;
            SubmitButton.Text = AppResources.StringSubmit;
            LabelPick.Text = AppResources.SentenceRoomsFilterPage;
            DatePickerRoom.MinimumDate = DateTime.Now;
            TimePickerEnd.Time = default(TimeSpan);
            TimePickerStart.Time = default(TimeSpan);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
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
            MessagingCenter.Subscribe<NewRoomsReservationPageViewModel>(this,
               Constants.MessagingCenter.NewRoomReservationPage.RoomReserved, send => ShowReport());
            MessagingCenter.Subscribe<NewRoomsReservationPageViewModel>(this,
               Constants.MessagingCenter.NewRoomReservationPage.TimeSmaller, send => ShowError());
            MessagingCenter.Subscribe<NewRoomsReservationPageViewModel>(this,
               Constants.MessagingCenter.NewRoomReservationPage.ReservedError, send => ErrorOrder());
            MessagingCenter.Subscribe<NewRoomsReservationPageViewModel>(this,
               Constants.MessagingCenter.NewRoomReservationPage.AddingRoomError, send => AddingError());
            MessagingCenter.Subscribe<NewRoomsReservationPageViewModel>(this,
               Constants.MessagingCenter.NewRoomReservationPage.RoomAdded, send => AddedRoomName());
            MessagingCenter.Subscribe<NewRoomsReservationPageViewModel>(this,
               Constants.MessagingCenter.NewRoomReservationPage.GoneWrong, send => ShowAlertNewRoom());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<NewRoomsReservationPageViewModel>(this, Constants.MessagingCenter.NewRoomReservationPage.RoomReserved);
            MessagingCenter.Unsubscribe<NewRoomsReservationPageViewModel>(this, Constants.MessagingCenter.NewRoomReservationPage.TimeSmaller);
            MessagingCenter.Unsubscribe<NewRoomsReservationPageViewModel>(this, Constants.MessagingCenter.NewRoomReservationPage.ReservedError);
            MessagingCenter.Unsubscribe<NewRoomsReservationPageViewModel>(this, Constants.MessagingCenter.NewRoomReservationPage.AddingRoomError);
            MessagingCenter.Unsubscribe<NewRoomsReservationPageViewModel>(this, Constants.MessagingCenter.NewRoomReservationPage.RoomAdded);
            MessagingCenter.Unsubscribe<NewRoomsReservationPageViewModel>(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
        }
        
        private void Names_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Names.SelectedItem = null;
        }
        private async void ShowAlertNewRoom()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceNewRoomReservationWrong,
                 AppResources.AlertOk);
        }

        private async void ShowReport()
        {         
            await DisplayAlert(AppResources.AlertTitleSuccess,
                 AppResources.AlertSentenceNewRoomReservationSuccessfully,
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
                 AppResources.AlertSentenceNewRoomReservationStartTimeBigger,
                 AppResources.AlertOk);
        }

        private async void AddedRoomName()
        {
            await DisplayAlert(AppResources.AlertTitleSuccess,
                 AppResources.AlertSentenceNewRoomReservationRoomNameAdded,
                 AppResources.AlertOk);
        }

        private async void AddingError()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceNewRoomReservationAlreadyUsed,
                 AppResources.AlertOk);
        }

        private async void ErrorOrder()
        {
            
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceNewRoomReservationAlreadyReservation,
                 AppResources.AlertOk);
        }
    }
}
