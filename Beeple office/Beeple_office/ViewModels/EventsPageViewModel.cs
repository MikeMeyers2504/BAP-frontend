using Beeple_office.Utils;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Foundation;
using Xamarin.Forms;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class EventsPageViewModel
    {
        #region Commands
        public ICommand GoToNewEventPageCommand { get; set; }
        public ICommand GoToSmallViewCommand { get; set; }
        public ICommand ChangePresentCommand { get; set; }
        public ICommand GoToFilterEventsPageCommand { get; set; }

        #endregion
        public String Email { get; set; }
        public String SortEvent { get; set; }
        public String Where { get; set; }
        public String Requirements { get; set; }
        public bool Transport { get; set; }
        public bool FoodEnDrinks { get; set; }
        public string LabelStart { get; set; }
        public string LabelEnd { get; set; }
        public string ButtonDetails { get; set; }
        public string ButtonPresent { get; set; }
        public string LabelPresent { get; set; }
        public DateTime TodayDate { get; set; }
        public DateTime TodayDateOnly { get; set; }
        public DateTime EventDate { get; set; }

        public static String Date { get; set; }
        public static String Name { get; set; }
        public static ObservableCollection<Invites> PersonsEventPresent { get; set; }

        public ObservableCollection<Invites> ShowPersonsOffEvent { get; set; }

        private ObservableCollection<Events> _eventsList;
        public ObservableCollection<Events> EventsList
        {
            get { return _eventsList; }
            set
            {
                _eventsList = value;
            }
        }

        private ObservableCollection<Events> _eventsListAfterDate;
        public ObservableCollection<Events> EventsListAfterDate
        {
            get { return _eventsListAfterDate; }
            set
            {
                _eventsListAfterDate = value;
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }
        public EventsPageViewModel()
        {
            InitCommands();
            ExecuteList();
            EventsListAfterDate = new ObservableCollection<Events>();
            ShowPersonsOffEvent = new ObservableCollection<Invites>();
            PersonsEventPresent = new ObservableCollection<Invites>();
            LabelStart = AppResources.StringStart;
            LabelEnd = AppResources.StringEnd;
            LabelPresent = AppResources.StringPresent;
            ButtonDetails = AppResources.StringDetails;
            ButtonPresent = AppResources.StringPresentButton;
            TodayDate = DateTime.Now;
            TodayDateOnly = TodayDate.Date;
        }

        private async void ExecuteList()
        {
            IsLoading = true;
            await GetEvents();
        }

        private void InitCommands()
        {
            GoToNewEventPageCommand = new Command(() => { GoToNewEvent(); });
            GoToSmallViewCommand = new Command<Events>(clickedEvent => { GoToDetail(clickedEvent); });
            ChangePresentCommand = new Command<Invites>(clickedPerson => { ChangePresent(clickedPerson); });
            GoToFilterEventsPageCommand = new Command(() => { GoToFilterEvents(); });
        }

        private void GoToNewEvent()
        {
            Email = null;
            SortEvent = null;
            Where = null;
            Requirements = null;
            ShowPersonsOffEvent.Clear();
            MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.NavigateToNewEvent);
        }
        private void GoToFilterEvents()
        {
            Email = null;
            SortEvent = null;
            Where = null;
            Requirements = null;
            ShowPersonsOffEvent.Clear();
            MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.NavigateToFilterEvents);
        }

        public async Task GetEvents()
        {
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.GetAsync(ConnectionLinks.EventsAddress);
                        Debug.WriteLine(response);

                        var content = await response.Content.ReadAsStringAsync();
                        EventsList = JsonConvert.DeserializeObject<ObservableCollection<Events>>(content);
                        EventsList = new ObservableCollection<Events>(EventsList.OrderBy(i => i.Name));
                        IsLoading = false;
                        Debug.WriteLine(EventsList);
                        foreach (var item in EventsList)
                        {
                            //EventDate = Convert.ToDateTime(item.Date);
                            EventDate = DateTime.ParseExact(item.Date, @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            if (EventDate >= TodayDateOnly)
                            {
                                EventsListAfterDate.Add(item);
                            }
                        }
                        EventsListAfterDate = new ObservableCollection<Events>(EventsListAfterDate.OrderBy(i => i.Name));
                        Debug.WriteLine(EventsList.Count);
                        Debug.WriteLine(EventsListAfterDate.Count);
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.GoneWrong);
                    }
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.GoneWrong);
            }
        }

        private void GoToDetail(Events clickedEvent)
        {
            IsLoading = true;
            ShowPersonsOffEvent.Clear();
            PersonsEventPresent.Clear();
            Email = null;
            SortEvent = null;
            Where = null;
            Requirements = null;

            MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.SelectedRow, clickedEvent);
            Debug.WriteLine(clickedEvent);
            Debug.WriteLine(clickedEvent.Date);
            Debug.WriteLine(clickedEvent.Email);
            Debug.WriteLine(clickedEvent.Start);
            Debug.WriteLine(clickedEvent.End);
            Debug.WriteLine(clickedEvent.SortEvent);
            Debug.WriteLine(clickedEvent.Where);
            Debug.WriteLine(clickedEvent.Invites);
            Debug.WriteLine(clickedEvent.Requirements);
            Debug.WriteLine(clickedEvent.Name);
            Debug.WriteLine(clickedEvent.Transport);
            Debug.WriteLine(clickedEvent.FoodEnDrinks);

            Email = clickedEvent.Email;
            SortEvent = clickedEvent.SortEvent;
            Where = clickedEvent.Where;
            Requirements = clickedEvent.Requirements;
            Transport = clickedEvent.Transport;
            FoodEnDrinks = clickedEvent.FoodEnDrinks;

            Date = clickedEvent.Date;
            Name = clickedEvent.Name;

            if (clickedEvent.Invites.Count > 0)
            {
                for (int i = 0; i < clickedEvent.Invites.Count; i++)
                {
                    ShowPersonsOffEvent.Add(clickedEvent.Invites[i]);
                    PersonsEventPresent.Add(clickedEvent.Invites[i]);
                }
                ShowPersonsOffEvent = new ObservableCollection<Invites>(ShowPersonsOffEvent.OrderBy(i => i.Email));
            }
            IsLoading = false;
        }

        public async void ChangePresent(Invites clickedPerson)
        {
            IsLoading = true;
            if (clickedPerson.Email == UserLoggedIn.User && clickedPerson.Present)
            {
                for (int i = 0; i < PersonsEventPresent.Count; i++)
                {
                    if (PersonsEventPresent[i].Email == UserLoggedIn.User)
                    {
                        PersonsEventPresent[i].Present = false;
                    }
                }
                await ChangePresentStatus();
            }
            else if (clickedPerson.Email == UserLoggedIn.User && !clickedPerson.Present)
            {
                for (int i = 0; i < PersonsEventPresent.Count; i++)
                {
                    if (PersonsEventPresent[i].Email == UserLoggedIn.User)
                    {
                        PersonsEventPresent[i].Present = true;
                    }
                }
                await ChangePresentStatus();
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.NoPermission);
            }                       
        }
                
        public async Task ChangePresentStatus()
        {
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        Debug.WriteLine(Date);
                        Debug.WriteLine(Name);

                        var jsonRequest =
                            new { date = Date, name = Name, invites = PersonsEventPresent };

                        var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        content.Headers.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.PutAsync(ConnectionLinks.EventsAddress, content);
                        Debug.WriteLine(response);

                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Success);

                        if (message.Success == "true")
                        {
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.ChangedSuccess);
                        }
                        else
                        {
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.ChangedFailed);
                        }
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.GoneWrong);
                    }                    
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.EventsPage.GoneWrong);
            }
        }
    }
}
