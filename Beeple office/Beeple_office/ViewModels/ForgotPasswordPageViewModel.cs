using Beeple_office.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;
using Xamarin.Forms;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class ForgotPasswordPageViewModel
    {
        #region Commands
        public ICommand ShowOrHideCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        #endregion

        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordCheck { get; set; }
        public string SecretOne { get; set; }
        public string SecretTwo { get; set; }

        public bool Changed;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }
        public ForgotPasswordPageViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            ShowOrHideCommand = new Command(() => { ShowHide(); });
            ChangePasswordCommand = new Command(() => { ChangePassword(); IsLoading = true; });
        }

        private void ShowHide()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.ForgotPasswordPage.ShowHide);
        }

        private async void ChangePassword()
        {
            if (Email != null && Password != null && SecretOne != null && SecretTwo != null)
            {
                if (Password == PasswordCheck)
                {
                    using (var c = new HttpClient())
                    {
                        try
                        {
                            var jsonRequest =
                            new { email = Email, password = Password, secretOne = SecretOne, secretTwo = SecretTwo };

                            var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                            HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");

                            var response = await c.PutAsync(ConnectionLinks.UsersAddress, content);
                            Debug.WriteLine(response);

                            var resultJson = await response.Content.ReadAsStringAsync();
                            var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                            Debug.WriteLine(message.Success);

                            if (message.Success == "true")
                            {
                                Changed = true;
                                IsLoading = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.ForgotPasswordPage.NavigateToLoginPage, Changed);
                            }
                            else
                            {
                                Changed = false;
                                IsLoading = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.ForgotPasswordPage.Error, Changed);
                            }
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            Changed = false;
                            Debug.WriteLine(e.Message);
                            MessagingCenter.Send(this, Constants.MessagingCenter.ForgotPasswordPage.GoneWrong);
                        }                        
                    }
                }
                else
                {
                    IsLoading = false;
                    MessagingCenter.Send(this, Constants.MessagingCenter.ForgotPasswordPage.PasswordDoesnotMatch);
                    Changed = false;
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.ForgotPasswordPage.EmptyFields);
                Changed = false;
            }
        }
    }
}
