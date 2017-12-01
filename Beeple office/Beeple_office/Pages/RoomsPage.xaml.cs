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
    public partial class RoomsPage : ContentPage
    {
        public ToolbarItem SettingsIos { get; set; }
        public RoomsPage()
        {
            InitializeComponent();
            BindingContext = new RoomsPageViewModel();

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
            base.OnAppearing();
            Subscribe();
            Title = AppResources.TitleRoomsPage;
            FilterButton.Text = AppResources.StringFilterList;
            NewRoomReservationButton.Text = AppResources.TitleNewRoomPage;
            LabelBring.IsVisible = false;
            LabelPersons.IsVisible = false;
            ListRooms.SelectedItem = null;
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
            MessagingCenter.Subscribe<RoomsPageViewModel>(this,
               Constants.MessagingCenter.RoomsPage.NavigateToNewRoom, send => NavigateToNewRoom());
            MessagingCenter.Subscribe<RoomsPageViewModel, Rooms>(this,
               Constants.MessagingCenter.RoomsPage.SelectedRow, (sender, room) => SelectRow(room));
            MessagingCenter.Subscribe<RoomsPageViewModel>(this,
               Constants.MessagingCenter.RoomsPage.NavigateToFilterRooms, send => NavigateToFilterRooms());
            MessagingCenter.Subscribe<RoomsPageViewModel>(this,
               Constants.MessagingCenter.RoomsPage.GoneWrong, send => ShowAlertRooms());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<RoomsPageViewModel>(this, Constants.MessagingCenter.RoomsPage.NavigateToNewRoom);
            MessagingCenter.Unsubscribe<RoomsPageViewModel, Rooms>(this, Constants.MessagingCenter.RoomsPage.SelectedRow);
            MessagingCenter.Unsubscribe<RoomsPageViewModel>(this, Constants.MessagingCenter.RoomsPage.NavigateToFilterRooms);
            MessagingCenter.Unsubscribe<RoomsPageViewModel>(this, Constants.MessagingCenter.RoomsPage.GoneWrong);
        }
        private async void ShowAlertRooms()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceRoomsPage,
                 AppResources.AlertOk);
        }
        private async void NavigateToNewRoom()
        {
            await Navigation.PushAsync(new NewRoomReservationPage());
        }

        private async void NavigateToFilterRooms()
        {
            await Navigation.PushAsync(new FilterRoomsPage());
        }

        private void SelectRow(Rooms room)
        {
            if (room.Extras != null)
            {
                LabelBring.Text = AppResources.RoomsPageBringThis;
                LabelBring.IsVisible = true;
            }
            else
            {
                LabelBring.IsVisible = false;
            }

            if (room.Persons.Count > 0)
            {
                LabelPersons.Text = AppResources.RoomsPageAddedPersons;
                LabelPersons.IsVisible = true;
            }
            else
            {
                LabelPersons.IsVisible = false;
            }
            ListRooms.SelectedItem = room;
        }
        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
