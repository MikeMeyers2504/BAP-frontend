using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Views;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Beeple_office.Utils;
using Java.Util;
using PropertyChanged;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class CheckinCheckoutPageViewModel
    {
        #region Commands
        public ICommand CheckinCommand { get; set; }
        public ICommand CheckoutCommand { get; set; }

        #endregion

        public string User { get; set; }
        public string CurrentTime { get; set; }
        public string CurrentTimeFull { get; set; }
        public string CurrentDate { get; set; }
        public string CurrentDateFull { get; set; }

        public bool CheckedOut;
        public bool CheckedIn;

        public int Hours;
        public int Minutes;
        public int Day;
        public int Month;
        public int Year;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }

        public CheckinCheckoutPageViewModel()
        {
            InitCommands();
            User = AppResources.StringEmail + ": " + UserLoggedIn.User;
            Hours = DateTime.Now.Hour;
            Minutes = DateTime.Now.Minute;
            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
            CurrentTime = string.Format("{0:00}:{1:00}", Hours, Minutes);
            CurrentDate = string.Format("{0:00}/{1:00}/{2:0000}", Day, Month, Year);
            CurrentTimeFull = AppResources.StringTime + CurrentTime;
            CurrentDateFull = AppResources.StringDate + CurrentDate;
            Debug.WriteLine(DateTime.Now);
            Debug.WriteLine(DateTime.UtcNow);           
        }

        private void InitCommands()
        {
            CheckinCommand = new Command(async () => { await Checkin(CurrentDate, CurrentTime); });
            CheckoutCommand = new Command(async () => { await Checkout(CurrentDate, CurrentTime); });
        }

        public async Task Checkin(string currentDate, string currentTime)
        {
            IsLoading = true;
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        var jsonRequest = new { email = UserLoggedIn.User, date = currentDate, time = currentTime };

                        var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        content.Headers.Add("x-access-token", UserLoggedIn.Token);
                        Debug.WriteLine(content);

                        var response = await c.PostAsync(ConnectionLinks.CheckinsAddress, content);
                        Debug.WriteLine(response);

                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Success);

                        if (message.Success == "true")
                        {
                            CheckedIn = true;
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.Checkin, CheckedIn);
                        }
                        else
                        {
                            CheckedIn = false;
                            if (!CheckedIn)
                            {
                                MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.Checkin, CheckedIn);
                            }
                            IsLoading = false;
                        }
                    }
                    catch (Exception e)
                    {
                        CheckedIn = false;
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.GoneWrong);
                    }                   
                }
            }
            else
            {
                CheckedIn = false;
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.GoneWrong); 
            }
        }

        public async Task Checkout(string currentDate, string currentTime)
        {
            IsLoading = true;
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        var jsonRequest = new { email = UserLoggedIn.User, date = currentDate, time = currentTime };

                        var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        content.Headers.Add("x-access-token", UserLoggedIn.Token);
                        Debug.WriteLine(content);

                        var response = await c.PostAsync(ConnectionLinks.CheckoutsAddress, content);
                        Debug.WriteLine(response);

                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Success);

                        if (message.Success == "true")
                        {
                            CheckedOut = true;
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.Checkout, CheckedOut);
                        }
                        else
                        {
                            CheckedOut = false;
                            if (!CheckedIn)
                            {
                                MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.Checkout, CheckedOut);
                            }
                            IsLoading = false;
                        }
                    }
                    catch (Exception e)
                    {
                        CheckedOut = false;
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.GoneWrong);
                    }                    
                }
            }
            else
            {
                CheckedOut = false;
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.CheckinCheckoutPage.GoneWrong);
            }
        }
    }
}
