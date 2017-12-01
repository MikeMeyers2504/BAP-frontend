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
using Java.Lang.Reflect;
using ModernHttpClient;
using Xamarin.Forms;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class NewRoomsReservationPageViewModel
    {
        #region Commands
        public ICommand SubmitReservationCommand { get; set; }
        public ICommand AddRoomCommand { get; set; }

        #endregion
        public RoomNames Room { get; set; }
        public string NewRoomName { get; set; }
        public string Material { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime Date { get; set; }
        public double Diff { get; set; }

        private ObservableCollection<RoomNames> _roomNames;
        public ObservableCollection<RoomNames> RoomNames
        {
            get { return _roomNames; }
            set
            {
                _roomNames = value;
            }
        }

        private ObservableCollection<User> _namesList;
        public ObservableCollection<User> NamesList
        {
            get { return _namesList; }
            set
            {
                _namesList = value;
            }
        }

        private ObservableCollection<User> _selectedNamesList;
        public ObservableCollection<User> SelectedNamesList
        {
            get { return _selectedNamesList; }
            set
            {
                _selectedNamesList = value;
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
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

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await GetRoomNames();

                    IsRefreshing = false;
                });
            }
        }
        public NewRoomsReservationPageViewModel()
        {
            InitCommands();
            ExecuteList();
            SelectedNamesList = new ObservableCollection<User>();
            RoomNames = new ObservableCollection<RoomNames>();
        }

        private void InitCommands()
        {
            SubmitReservationCommand = new Command(() => { Show(); });
            AddRoomCommand = new Command(() => { AddRoom(); });
        }

        private async void ExecuteList()
        {
            IsLoading = true;
            await GetNames();
            await GetRoomNames();
        }

        private async void AddRoom()
        {
            IsLoading = true;
            if (NewRoomName != null)
            {
                if (UserLoggedIn.User != null)
                {
                    using (var c = new HttpClient())
                    {
                        try
                        {
                            var jsonRequest = new { roomName = NewRoomName };

                            var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                            HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                            content.Headers.Add("x-access-token", UserLoggedIn.Token);
                            Debug.WriteLine(content);

                            var response = await c.PostAsync(ConnectionLinks.RoomNamesAddress, content);
                            Debug.WriteLine(response);

                            var resultJson = await response.Content.ReadAsStringAsync();
                            var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                            Debug.WriteLine(message.Success);
                            if (message.Success == "true")
                            {
                                NewRoomName = null;
                                await GetRoomNames();
                                IsLoading = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.RoomAdded);
                            }
                            else
                            {
                                IsLoading = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.AddingRoomError);
                            }
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            Debug.WriteLine(e.Message);
                            MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
                        }                        
                    }
                }
                else
                {
                    NewRoomName = null;
                    IsLoading = false;
                    MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.AddingRoomError);
            }
            
        }
        public async Task GetNames()
        {
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.GetAsync(ConnectionLinks.UsersAddress);
                        Debug.WriteLine(response);

                        var content = await response.Content.ReadAsStringAsync();
                        NamesList = JsonConvert.DeserializeObject<ObservableCollection<User>>(content);
                        NamesList = new ObservableCollection<User>(NamesList.OrderBy(i => i.LastName));
                        Debug.WriteLine(NamesList);
                        //IsLoading = false;
                    }
                    catch (Exception e)
                    {
                        //IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);

                    }
                }
            }
            else
            {
                //IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
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
                        MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
                    }
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
            }
        }

        private async void Show()
        {
            IsLoading = true;
            int day = Date.Day;
            int month = Date.Month;
            int year = Date.Year;
            string reservedDate = day + "/" + month + "/" + year;

            Debug.WriteLine(Room);
            Debug.WriteLine(Material);
            Debug.WriteLine(TimeStart);
            Debug.WriteLine(TimeEnd);
            Debug.WriteLine(Date.Day);
            Debug.WriteLine(Date.Month);
            Debug.WriteLine(Date.Year);
            Debug.WriteLine(reservedDate);

            foreach(var name in NamesList)
            {
                if (name.IsSelected)
                {
                    Debug.WriteLine(name.FirstName);
                    Debug.WriteLine(name.LastName);
                    Debug.WriteLine(name.IsSelected);
                    SelectedNamesList.Add(new User(){FirstName = name.FirstName, LastName = name.LastName, Email = name.Email, IsSelected = name.IsSelected });
                }
            }

            for (int i = 0; i < SelectedNamesList.Count; i++)
            {
                Debug.WriteLine("added name: " + SelectedNamesList[i].FirstName);
            }
            
            if (TimeStart < TimeEnd)
            {
                await OrderRoom(SelectedNamesList, reservedDate, TimeStart, TimeEnd, Material, Room.RoomName);
            }
            else
            {
                IsLoading = false;
                SelectedNamesList.Clear();
                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.TimeSmaller);
            }
        }

        public async Task OrderRoom(ObservableCollection<User> namesInvites, string exactDate, TimeSpan timeStart, TimeSpan timeEnd, string material, string whichRoom)
        {          
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        var jsonRequest = new { email = UserLoggedIn.User, date = exactDate, start = timeStart, end = timeEnd, extras = Material, room = whichRoom, persons = namesInvites };

                        var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        content.Headers.Add("x-access-token", UserLoggedIn.Token);
                        Debug.WriteLine(content);

                        var response = await c.PostAsync(ConnectionLinks.RoomsAddress, content);
                        Debug.WriteLine(response);

                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Success);

                        if (message.Success == "true")
                        {
                            SelectedNamesList.Clear();
                            namesInvites.Clear();
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.RoomReserved);
                        }
                        else
                        {
                            SelectedNamesList.Clear();
                            namesInvites.Clear();
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.ReservedError);
                        }
                    }
                    catch (Exception e)
                    {
                        SelectedNamesList.Clear();
                        namesInvites.Clear();
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
                    }
                }
            }
            else
            {
                SelectedNamesList.Clear();
                namesInvites.Clear();
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.NewRoomReservationPage.GoneWrong);
            }
        }
    }
}
