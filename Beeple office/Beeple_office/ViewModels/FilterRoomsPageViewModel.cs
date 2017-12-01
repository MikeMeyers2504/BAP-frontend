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
    public class FilterRoomsPageViewModel
    {
        #region Commands
        public ICommand GoToNewRoomPageCommand { get; set; }
        public ICommand FilterRoomCommand { get; set; }
        public ICommand FilterTimeCommand { get; set; }
        #endregion

        public RoomNames Room { get; set; }
        public bool FilterTime { get; set; }
        public bool FilterRoom { get; set; }
        public bool VisibleRoom { get; set; }
        public bool VisibleTime { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime Date { get; set; }
        public string LabelStart { get; set; }
        public string LabelEnd { get; set; }

        private ObservableCollection<Rooms> _filteredReservedRoomsList;
        public ObservableCollection<Rooms> FilteredReservedRoomsList
        {
            get { return _filteredReservedRoomsList; }
            set
            {
                _filteredReservedRoomsList = value;
            }
        }

        private ObservableCollection<Rooms> _filteredReservedRoomsListTime;
        public ObservableCollection<Rooms> FilteredReservedRoomsListTime
        {
            get { return _filteredReservedRoomsListTime; }
            set
            {
                _filteredReservedRoomsListTime = value;
            }
        }

        private ObservableCollection<RoomNames> _roomNames;
        public ObservableCollection<RoomNames> RoomNames
        {
            get { return _roomNames; }
            set
            {
                _roomNames = value;
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

        public FilterRoomsPageViewModel()
        {
            InitCommands();
            ExecuteList();
            RoomNames = new ObservableCollection<RoomNames>();
            FilteredReservedRoomsListTime = new ObservableCollection<Rooms>();
            LabelStart = AppResources.StringStart;
            LabelEnd = AppResources.StringEnd;
        }

        private async void ExecuteList()
        {
            IsLoading = true;
            await GetRoomNames();
        }

        private void InitCommands()
        {
            GoToNewRoomPageCommand = new Command(() => { GoToNewRoom(); });
            FilterRoomCommand = new Command(() => { GoToFilterRooms(); });
            FilterTimeCommand = new Command(() => { GoToFilterRoomsTime(); });
        }

        private void GoToNewRoom()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.NavigateToNewRoom);
        }

        private async void GoToFilterRooms()
        {
            IsLoading = true;
            FilterRoom = true;
            FilterTime = false;
            await GetOrderedRooms();
        }

        private async void GoToFilterRoomsTime()
        {
            IsLoading = true;
            MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.UnselectRow);
            FilterRoom = false;
            FilterTime = true;
            await GetOrderedRooms();
        }

        public async Task GetOrderedRooms()
        {
            int day = Date.Day;
            int month = Date.Month;
            int year = Date.Year;
            string reservedDate = day + "/" + month + "/" + year;

            if (UserLoggedIn.User != null)
            {
                if (FilterRoom && Room != null)
                {
                    using (var c = new HttpClient())
                    {
                        try
                        {
                            c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                            var response = await c.GetAsync(ConnectionLinks.RoomsAddress + "?date=" + reservedDate + "&room=" + Room.RoomName);
                            Debug.WriteLine(response);

                            var content = await response.Content.ReadAsStringAsync();
                            FilteredReservedRoomsList = JsonConvert.DeserializeObject<ObservableCollection<Rooms>>(content);
                            FilteredReservedRoomsList = new ObservableCollection<Rooms>(FilteredReservedRoomsList.OrderBy(i => i.Start));
                            if (FilteredReservedRoomsList.Count != 0)
                            {
                                IsLoading = false;
                                VisibleRoom = true;
                                VisibleTime = false;
                            }
                            else
                            {
                                IsLoading = false;
                                VisibleRoom = false;
                                VisibleTime = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.Nothing);
                            }
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            VisibleRoom = false;
                            VisibleTime = false;
                            Debug.WriteLine(e.Message);
                            MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.GoneWrong);
                        }                        
                    }
                } else if (FilterTime && TimeStart > new TimeSpan(0, 0, 0) && TimeEnd > new TimeSpan(0, 0, 0) && TimeEnd > TimeStart)
                {
                    FilteredReservedRoomsListTime.Clear();
                    using (var c = new HttpClient())
                    {
                        try
                        {
                            c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                            var response = await c.GetAsync(ConnectionLinks.RoomsAddress);
                            Debug.WriteLine(response);

                            var content = await response.Content.ReadAsStringAsync();
                            FilteredReservedRoomsList = JsonConvert.DeserializeObject<ObservableCollection<Rooms>>(content);
                            FilteredReservedRoomsList = new ObservableCollection<Rooms>(FilteredReservedRoomsList.OrderBy(i => i.Start));
                            foreach (var item in FilteredReservedRoomsList)
                            {
                                if (item.Date == reservedDate && item.Start >= TimeStart && item.End <= TimeEnd)
                                {
                                    FilteredReservedRoomsListTime.Add(item);
                                }
                            }
                            FilteredReservedRoomsListTime = new ObservableCollection<Rooms>(FilteredReservedRoomsListTime.OrderBy(i => i.Start));
                            if (FilteredReservedRoomsListTime.Count != 0)
                            {
                                IsLoading = false;
                                VisibleTime = true;
                                VisibleRoom = false;
                            }
                            else
                            {
                                IsLoading = false;
                                VisibleTime = false;
                                VisibleRoom = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.Nothing);
                            }
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            VisibleTime = false;
                            VisibleRoom = false;
                            Debug.WriteLine(e.Message);
                            MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.GoneWrong);
                        }                        
                    }
                }
                else
                {
                    IsLoading = false;
                    VisibleRoom = false;
                    VisibleTime = false;
                    MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.Error);
                }                
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.GoneWrong);
            }
        }

        public async Task GetRoomNames()
        {
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.GetAsync(ConnectionLinks.RoomNamesAddress);
                        Debug.WriteLine(response);

                        var content = await response.Content.ReadAsStringAsync();
                        RoomNames = JsonConvert.DeserializeObject<ObservableCollection<RoomNames>>(content);
                        Debug.WriteLine(RoomNames);
                        RoomNames = new ObservableCollection<RoomNames>(RoomNames.OrderBy(i => i.RoomName));
                        IsLoading = false;
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.GoneWrong);
                    }
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.FilterRoomsPage.GoneWrong);
            }
        }
    }
}
