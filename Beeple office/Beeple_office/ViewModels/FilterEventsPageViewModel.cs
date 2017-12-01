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
using Xamarin.Forms;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class FilterEventsPageViewModel
    {
        #region Commands
        public ICommand GoToNewEventPageCommand { get; set; }
        public ICommand FilterEventDateCommand { get; set; }
        public ICommand FilterEventTimeCommand { get; set; }
        #endregion

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }

        public bool FilterTime { get; set; }
        public bool FilterDate { get; set; }
        public bool VisibleDate { get; set; }
        public bool VisibleTime { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime Date { get; set; }
        public string LabelStart { get; set; }
        public string LabelEnd { get; set; }

        private ObservableCollection<Events> _filteredEventsListDate;
        public ObservableCollection<Events> FilteredEventsListDate
        {
            get { return _filteredEventsListDate; }
            set
            {
                _filteredEventsListDate = value;
            }
        }

        private ObservableCollection<Events> _filteredEventsListTime;
        public ObservableCollection<Events> FilteredEventsListTime
        {
            get { return _filteredEventsListTime; }
            set
            {
                _filteredEventsListTime = value;
            }
        }

        public FilterEventsPageViewModel()
        {
            InitCommands();
            FilteredEventsListTime = new ObservableCollection<Events>();
            LabelStart = AppResources.StringStart;
            LabelEnd = AppResources.StringEnd;
        }

        private void InitCommands()
        {
            GoToNewEventPageCommand = new Command(() => { GoToNewEvent(); });
            FilterEventDateCommand = new Command(() => { GoToFilterEventsDate(); });
            FilterEventTimeCommand = new Command(() => { GoToFilterEventsTime(); });
        }

        private void GoToNewEvent()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.NavigateToNewEvent);
        }

        private async void GoToFilterEventsDate()
        {
            IsLoading = true;
            FilterDate = true;
            FilterTime = false;
            await GetFilteredEvents();
        }

        private async void GoToFilterEventsTime()
        {
            IsLoading = true;
            FilterDate = false;
            FilterTime = true;
            await GetFilteredEvents();
        }
        public async Task GetFilteredEvents()
        {
            int day = Date.Day;
            int month = Date.Month;
            int year = Date.Year;
            string reservedDate = day + "/" + month + "/" + year;

            if (UserLoggedIn.User != null)
            {
                if (FilterDate)
                {
                    using (var c = new HttpClient())
                    {
                        try
                        {
                            c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                            var response = await c.GetAsync(ConnectionLinks.EventsAddress + "?date=" + reservedDate);
                            Debug.WriteLine(response);

                            var content = await response.Content.ReadAsStringAsync();
                            FilteredEventsListDate = JsonConvert.DeserializeObject<ObservableCollection<Events>>(content);
                            FilteredEventsListDate = new ObservableCollection<Events>(FilteredEventsListDate.OrderBy(i => i.Start));
                            if (FilteredEventsListDate.Count != 0)
                            {
                                IsLoading = false;
                                VisibleDate = true;
                                VisibleTime = false;
                            }
                            else
                            {
                                IsLoading = false;
                                VisibleDate = false;
                                VisibleTime = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.Nothing);
                            }
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            VisibleDate = false;
                            VisibleTime = false;
                            Debug.WriteLine(e.Message);
                            MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.GoneWrong);
                        }                        
                    }
                }
                else if (FilterTime && TimeStart > new TimeSpan(0, 0, 0) && TimeEnd > new TimeSpan(0, 0, 0) && TimeEnd > TimeStart)
                {
                    FilteredEventsListTime.Clear();
                    using (var c = new HttpClient())
                    {
                        try
                        {
                            c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                            var response = await c.GetAsync(ConnectionLinks.EventsAddress);
                            Debug.WriteLine(response);

                            var content = await response.Content.ReadAsStringAsync();
                            FilteredEventsListDate = JsonConvert.DeserializeObject<ObservableCollection<Events>>(content);
                            FilteredEventsListDate = new ObservableCollection<Events>(FilteredEventsListDate.OrderBy(i => i.Start));
                            foreach (var item in FilteredEventsListDate)
                            {
                                if (/*item.Date == reservedDate && */item.Start >= TimeStart && item.End <= TimeEnd)
                                {
                                    FilteredEventsListTime.Add(item);
                                }
                            }
                            FilteredEventsListTime = new ObservableCollection<Events>(FilteredEventsListTime.OrderBy(i => i.Start));
                            if (FilteredEventsListTime.Count != 0)
                            {
                                IsLoading = false;
                                VisibleTime = true;
                                VisibleDate = false;
                            }
                            else
                            {
                                IsLoading = false;
                                VisibleTime = false;
                                VisibleDate = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.Nothing);
                            }
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            VisibleTime = false;
                            VisibleDate = false;
                            Debug.WriteLine(e.Message);
                            MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.GoneWrong);
                        }                        
                    }
                }
                else
                {
                    IsLoading = false;
                    VisibleDate = false;
                    VisibleTime = false;
                    MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.Error);
                }
            }
            else
            {
                VisibleDate = false;
                VisibleTime = false;
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.FilterEventsPage.GoneWrong);
            }
        }
    }
}
