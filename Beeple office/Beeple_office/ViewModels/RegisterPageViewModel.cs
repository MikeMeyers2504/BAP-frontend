using Beeple_office.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using Xamarin.Forms;
using System.Net.Http;
using Android.Content.Res;
using Beeple_office.Pages;
using Newtonsoft.Json;
using PropertyChanged;
using System.Text.RegularExpressions;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class RegisterPageViewModel
    {
        #region Commands
        public ICommand ShowOrHideCommand { get; set; }
        public ICommand PostUserCommand { get; set; }
        #endregion

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PasswordCheck { get; set; }
        public string SecretOne { get; set; }
        public string SecretTwo { get; set; }

        public bool Added;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }

        public RegisterPageViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            ShowOrHideCommand = new Command(() => { ShowHide(); });
            PostUserCommand = new Command(async () => { IsLoading = true; await SendUser(); });
        }

        private void ShowHide()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.ShowHide);
        }
        public async Task SendUser()
        {
            if (FirstName != null && LastName != null && Email != null && Password != null && SecretOne != null && SecretTwo != null)
            {
                if (Password == PasswordCheck)
                {
                    if (Regex.Match(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
                    {
                        using (var c = new HttpClient())
                        {
                            try
                            {
                                var jsonRequest =
                                new { lastName = LastName, firstName = FirstName, password = Password, email = Email, secretOne = SecretOne, secretTwo = SecretTwo, responsible = false };

                                var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                                HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");

                                var response = await c.PostAsync(ConnectionLinks.UsersAddress, content);
                                Debug.WriteLine(response);
                                var resultJson = await response.Content.ReadAsStringAsync();
                                var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                                if (message.Success == "true")
                                {
                                    Added = true;
                                    IsLoading = false;
                                    MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.NavigateToLoginPage, Added);
                                }
                                else
                                {
                                    Added = false;
                                    IsLoading = false;
                                    MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.NavigateToLoginPage, Added);
                                }
                            }
                            catch (Exception e)
                            {
                                Added = false;
                                IsLoading = false;
                                Debug.WriteLine(e.Message);
                                MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.GoneWrong);
                            }
                        }
                    }
                    else
                    {
                        IsLoading = false;
                        MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.EmailValidate);
                    }
                }
                else
                {
                    IsLoading = false;
                    MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.PasswordDoesnotMatch);
                    Added = false;
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.RegisterPage.EmptyFields);
                Added = false;
            }
        }
    }
}
