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
    public class RoomsPageViewModel
    {
        #region Commands
        public ICommand GoToNewRoomPageCommand { get; set; }
        public ICommand GoToSmallViewCommand { get; set; }
        public ICommand GoToFilterPageCommand { get; set; }

        #endregion

        public DateTime TodayDate { get; set; }

        public string Extra { get; set; }
        public string NameButton { get; set; }
        public string LabelStart { get; set; }
        public string LabelEnd { get; set; }

        public ObservableCollection<User> ShowPersonsOffReservedRoom { get; set; }

        public Rooms ReservedRoom { get; set; }

        private ObservableCollection<Rooms> _reservedRoomsList;
        public ObservableCollection<Rooms> ReservedRoomsList
        {
            get { return _reservedRoomsList; }
            set
            {
                _reservedRoomsList = value;
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

        //public ObservableCollection<NameAndEmail> AddedPersons { get; set; }

        public RoomsPageViewModel()
        {
            InitCommands();
            ExecuteList();
            ShowPersonsOffReservedRoom = new ObservableCollection<User>();
            NameButton = AppResources.StringDetails;
            LabelStart = AppResources.StringStart;
            LabelEnd = AppResources.StringEnd;
        }

        private async void ExecuteList()
        {
            IsLoading = true;
            TodayDate = DateTime.Now;
            await GetOrderedRooms();
        }

        private void InitCommands()
        {
            GoToNewRoomPageCommand = new Command(() => { GoToNewRoom(); });
            GoToSmallViewCommand = new Command<Rooms>(room => { GoToDetail(room); });
            GoToFilterPageCommand = new Command(() => { GoToFilterRooms(); });
        }

        private void GoToNewRoom()
        {
            ShowPersonsOffReservedRoom.Clear();
            Extra = null;
            MessagingCenter.Send(this, Constants.MessagingCenter.RoomsPage.NavigateToNewRoom);
        }

        private void GoToFilterRooms()
        {
            ShowPersonsOffReservedRoom.Clear();
            Extra = null;
            MessagingCenter.Send(this, Constants.MessagingCenter.RoomsPage.NavigateToFilterRooms);
        }

        public async Task GetOrderedRooms()
        {
            int day = TodayDate.Day;
            int month = TodayDate.Month;
            int year = TodayDate.Year;
            string fulldate = day + "/" + month + "/" + year; 

            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.GetAsync(ConnectionLinks.RoomsAddress + "?date=" + fulldate);
                        Debug.WriteLine(response);

                        var content = await response.Content.ReadAsStringAsync();
                        ReservedRoomsList = JsonConvert.DeserializeObject<ObservableCollection<Rooms>>(content);
                        ReservedRoomsList = new ObservableCollection<Rooms>(ReservedRoomsList.OrderBy(i => i.Start));
                        Debug.WriteLine(ReservedRoomsList);
                        IsLoading = false;

                        foreach (var item in ReservedRoomsList)
                        {
                            Debug.WriteLine(item.Email);
                            Debug.WriteLine(item.Date);
                            Debug.WriteLine(item.End);
                            Debug.WriteLine(item.Extras);
                            Debug.WriteLine(item.Room);
                            Debug.WriteLine(item.Start);
                            Debug.WriteLine(item.Persons);
                            Debug.WriteLine("New reservation: ");
                        }
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.RoomsPage.GoneWrong);
                    }
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.RoomsPage.GoneWrong);
            }
        }

        private void GoToDetail(Rooms room)
        {
            ShowPersonsOffReservedRoom.Clear();
            Extra = null;
            MessagingCenter.Send(this, Constants.MessagingCenter.RoomsPage.SelectedRow, room);
            //ShowPersonsOffReservedRoom = new ObservableCollection<User>();
            Debug.WriteLine(room);
            Debug.WriteLine(room.Date);
            Debug.WriteLine(room.Email);
            Debug.WriteLine(room.End);
            Debug.WriteLine(room.Extras);
            Debug.WriteLine(room.Room);
            Debug.WriteLine(room.Start);
            Debug.WriteLine(room.Persons);
            Extra = room.Extras;

            if (room.Persons.Count > 0)
            {
                for (int i = 0; i < room.Persons.Count; i++)
                {
                    ShowPersonsOffReservedRoom.Add(room.Persons[i]);
                }
                ShowPersonsOffReservedRoom = new ObservableCollection<User>(ShowPersonsOffReservedRoom.OrderBy(i => i.LastName));
            }
        }
    }
}
