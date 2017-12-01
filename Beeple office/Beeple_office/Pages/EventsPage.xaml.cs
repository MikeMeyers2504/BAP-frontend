using Beeple_office.Utils;
using Beeple_office.ViewModels;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Beeple_office.Pages
{
    public partial class EventsPage : ContentPage
    {
        public ToolbarItem SettingsIos { get; set; }
        public EventsPage()
        {
            InitializeComponent();
            BindingContext = new EventsPageViewModel();

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
            LabelPersons.IsVisible = false;
            LabelEmail.IsVisible = false;
            LabelSort.IsVisible = false;
            LabelWhere.IsVisible = false;
            LabelReq.IsVisible = false;
            LabelTransport.IsVisible = false;
            LabelFeD.IsVisible = false;
            ListEvents.SelectedItem = null;

            Title = AppResources.TitleEventsPage;
            LabelPersons.Text = AppResources.StringInvitedPersons;
            LabelEmail.Text = AppResources.StringOrganiser;
            LabelSort.Text = AppResources.StringSortEvent;
            LabelWhere.Text = AppResources.StringWhere;
            LabelReq.Text = AppResources.StringRequirements;
            LabelTransport.Text = AppResources.StringTransport;
            LabelFeD.Text = AppResources.StringFoodDrinks;
            NewEButton.Text = AppResources.StringNewEvent;
            FilterButton.Text = AppResources.StringFilterList;

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
            MessagingCenter.Subscribe<EventsPageViewModel>(this,
               Constants.MessagingCenter.EventsPage.NavigateToNewEvent, send => NavigateToNewEvent());
            MessagingCenter.Subscribe<EventsPageViewModel, Events>(this,
               Constants.MessagingCenter.EventsPage.SelectedRow, (sender, thisEvent) => SelectRow(thisEvent));
            MessagingCenter.Subscribe<EventsPageViewModel>(this,
               Constants.MessagingCenter.EventsPage.NavigateToFilterEvents, send => NavigateToFilter());
            MessagingCenter.Subscribe<EventsPageViewModel>(this,
               Constants.MessagingCenter.EventsPage.ChangedFailed, send => ChangeFailed());
            MessagingCenter.Subscribe<EventsPageViewModel>(this,
               Constants.MessagingCenter.EventsPage.ChangedSuccess, send => ChangeSuccess());
            MessagingCenter.Subscribe<EventsPageViewModel>(this,
               Constants.MessagingCenter.EventsPage.NoPermission, send => NoPermission());
            MessagingCenter.Subscribe<EventsPageViewModel>(this,
               Constants.MessagingCenter.EventsPage.GoneWrong, send => ShowAlertEvents());
        }

        private void UnSubscribe()
        {
            MessagingCenter.Unsubscribe<EventsPageViewModel>(this, Constants.MessagingCenter.EventsPage.NavigateToNewEvent);
            MessagingCenter.Unsubscribe<EventsPageViewModel, Events>(this, Constants.MessagingCenter.EventsPage.SelectedRow);
            MessagingCenter.Unsubscribe<EventsPageViewModel>(this, Constants.MessagingCenter.EventsPage.NavigateToFilterEvents);
            MessagingCenter.Unsubscribe<EventsPageViewModel>(this, Constants.MessagingCenter.EventsPage.ChangedFailed);
            MessagingCenter.Unsubscribe<EventsPageViewModel>(this, Constants.MessagingCenter.EventsPage.ChangedSuccess);
            MessagingCenter.Unsubscribe<EventsPageViewModel>(this, Constants.MessagingCenter.EventsPage.NoPermission);
            MessagingCenter.Unsubscribe<EventsPageViewModel>(this, Constants.MessagingCenter.EventsPage.GoneWrong);
        }
        private async void ShowAlertEvents()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceEventsWrong,
                 AppResources.AlertOk);
            await Navigation.PushAsync(new MenuPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
        }
        private async void ChangeFailed()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceEventsNoEvent,
                 AppResources.AlertOk);
        }
        private async void ChangeSuccess()
        {
            await DisplayAlert(AppResources.AlertTitleSuccess,
                 AppResources.AlertSentenceEventsSuccessfully,
                 AppResources.AlertOk);
            await Navigation.PushAsync(new MenuPage());
            foreach (var page in Navigation.NavigationStack.ToList())
            {
                Navigation.RemovePage(page);
            }
        }
        private async void NoPermission()
        {
            await DisplayAlert(AppResources.AlertTitleError,
                 AppResources.AlertSentenceEventsPermission,
                 AppResources.AlertOk);
        }
        private async void NavigateToNewEvent()
        {
            LabelTYesNo.IsVisible = false;
            LabelFeDYesNo.IsVisible = false;
            await Navigation.PushAsync(new NewEventPage());
        }

        private async void NavigateToFilter()
        {
            LabelTYesNo.IsVisible = false;
            LabelFeDYesNo.IsVisible = false;
            await Navigation.PushAsync(new FilterEventsPage());
        }

        private void SelectRow(Events thisEvent)
        {
            if (thisEvent != null)
            {
                LabelPersons.IsVisible = true;
                LabelEmail.IsVisible = true;
                LabelSort.IsVisible = true;
                LabelWhere.IsVisible = true;
                LabelReq.IsVisible = true;
                LabelTransport.IsVisible = true;
                LabelFeD.IsVisible = true;
                if (thisEvent.FoodEnDrinks)
                {
                    LabelFeDYesNo.Text = AppResources.StringYes;
                }
                else
                {
                    LabelFeDYesNo.Text = AppResources.StringNo;
                }
                if (thisEvent.Transport)
                {
                    LabelTYesNo.Text = AppResources.StringYes;
                }
                else
                {
                    LabelTYesNo.Text = AppResources.StringNo;
                }
            }
            else
            {
                LabelPersons.IsVisible = false;
                LabelEmail.IsVisible = false;
                LabelSort.IsVisible = false;
                LabelWhere.IsVisible = false;
                LabelReq.IsVisible = false;
                LabelTransport.IsVisible = false;
                LabelFeD.IsVisible = false;
            }
            ListEvents.SelectedItem = thisEvent;
        }

        private async void NavigateToSettings()
        {
            await Navigation.PushAsync(new HamburgerMenu());
        }
    }
}
