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
    public class NewEventPageViewModel
    {
        #region Commands
        public ICommand SubmitEventCommand { get; set; }

        #endregion

        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime Date { get; set; }
        public string SortEvent { get; set; }
        public string Where { get; set; }
        public string Requirements { get; set; }
        public string Name { get; set; }
        public bool Transport { get; set; }
        public bool FoodAndDrinks { get; set; }

        private ObservableCollection<User> _userList;
        public ObservableCollection<User> UserList
        {
            get { return _userList; }
            set
            {
                _userList = value;
            }
        }

        private ObservableCollection<Invites> _selectedNamesList;
        public ObservableCollection<Invites> SelectedNamesList
        {
            get { return _selectedNamesList; }
            set
            {
                _selectedNamesList = value;
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

        public NewEventPageViewModel()
        {
            InitCommands();
            ExecuteList();
            SelectedNamesList = new ObservableCollection<Invites>();
        }

        private void InitCommands()
        {
            SubmitEventCommand = new Command(() => { Show(); });
        }

        private async void ExecuteList()
        {
            IsLoading = true;
            await GetNames();
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
                        UserList = JsonConvert.DeserializeObject<ObservableCollection<User>>(content);
                        UserList = new ObservableCollection<User>(UserList.OrderBy(i => i.LastName));
                        IsLoading = false;
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.GoneWrong);
                    }
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.GoneWrong);
            }
        }

        private async void Show()
        {
            IsLoading = true;
            int day = Date.Day;
            int month = Date.Month;
            int year = Date.Year;
            string reservedDate = day + "/" + month + "/" + year;

            Debug.WriteLine(Name);
            Debug.WriteLine(Where);
            Debug.WriteLine(Requirements);
            Debug.WriteLine(Transport);
            Debug.WriteLine(FoodAndDrinks);
            Debug.WriteLine(SortEvent);
            Debug.WriteLine(TimeStart);
            Debug.WriteLine(TimeEnd);
            Debug.WriteLine(Date.Day);
            Debug.WriteLine(Date.Month);
            Debug.WriteLine(Date.Year);
            Debug.WriteLine(reservedDate);

            foreach (var name in UserList)
            {
                if (name.IsSelected)
                {
                    Debug.WriteLine(name.Email);
                    Debug.WriteLine(name.IsSelected);
                    SelectedNamesList.Add(new Invites() { Email = name.Email, Present = false });
                }
            }

            if (TimeStart < TimeEnd && Where != null && SortEvent != null && Name != null)
            {
                await NewEvent(SelectedNamesList, reservedDate, TimeStart, TimeEnd, Name, Where, Requirements, Transport, FoodAndDrinks, SortEvent);
            }
            else
            {
                IsLoading = false;
                SelectedNamesList.Clear();
                MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.TimeSmaller);
            }
        }

        public async Task NewEvent(ObservableCollection<Invites> namesInvites, string exactDate, TimeSpan timeStart, TimeSpan timeEnd, string eventName, string place, string requirementsNeeded, bool transportEvent, bool foodDrinks, string whichEvent)
        {
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        var jsonRequest = new { email = UserLoggedIn.User, date = exactDate, start = timeStart, end = timeEnd, sortEvent = whichEvent, where = place, name = eventName, foodEnDrinks = foodDrinks, requirements = requirementsNeeded, transport = transportEvent, invites = namesInvites };

                        var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        content.Headers.Add("x-access-token", UserLoggedIn.Token);
                        Debug.WriteLine(content);

                        var response = await c.PostAsync(ConnectionLinks.EventsAddress, content);
                        Debug.WriteLine(response);

                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Success);

                        if (message.Success == "true")
                        {
                            SelectedNamesList.Clear();
                            namesInvites.Clear();
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.NewEvent);
                        }
                        else
                        {
                            SelectedNamesList.Clear();
                            namesInvites.Clear();
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.EventError);
                        }
                    }
                    catch (Exception e)
                    {
                        SelectedNamesList.Clear();
                        namesInvites.Clear();
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.GoneWrong);
                    }                   
                }
            }
            else
            {
                SelectedNamesList.Clear();
                namesInvites.Clear();
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.NewEventPage.GoneWrong);
            }
        }
    }
}
