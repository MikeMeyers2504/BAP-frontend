using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Net.Http;
using Android.Content.Res;
using Beeple_office.Pages;
using Newtonsoft.Json;
using System.Diagnostics;
using Beeple_office.Utils;
using PropertyChanged;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class LoginPageViewModel
    {
        #region Commands
        public ICommand CheckLoginCommand { get; set; }
        public ICommand GoToRegisterCommand { get; set; }
        public ICommand GoToForgotPasswordCommand { get; set; }
        #endregion

        public string UserName { get; set; }
        public string Password { get; set; }

        public static bool HasLoggedIn;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }

        public LoginPageViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            CheckLoginCommand = new Command(async () => { IsLoading = true; await Login(UserName, Password); });
            GoToRegisterCommand = new Command(() => { GoToRegister(); });
            GoToForgotPasswordCommand = new Command(() => { GoToForgotPassword(); });
        }

        private void GoToRegister()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.LoginPage.NavigateToRegisterPage);
        }

        private void GoToForgotPassword()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.LoginPage.NavigateToForgotPasswordPage);
        }

        public async Task Login(string username, string passwordUser)
        {
            if (username != null || passwordUser != null)
            {
                using (var c = new HttpClient())
                {
                    var jsonRequest = new { email = username, password = passwordUser };
                    var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                    try
                    {
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        var response = await c.PostAsync(ConnectionLinks.AuthenticateAddress, content);
                        Debug.WriteLine(response);
                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Token);
                        Debug.WriteLine(message.Success);
                        if (message.Success == "true")
                        {
                            HasLoggedIn = true;
                            UserLoggedIn.User = username;
                            UserLoggedIn.Token = message.Token;
                            UserLoggedIn.Responsible = message.Responsible;
                            Debug.WriteLine(UserLoggedIn.Token);
                            Debug.WriteLine(UserLoggedIn.User);
                            Debug.WriteLine(UserLoggedIn.Responsible);
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.LoginPage.NavigateToMenuPage);
                        }
                        else
                        {
                            HasLoggedIn = false;
                            if (!HasLoggedIn)
                            {
                                IsLoading = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.LoginPage.LoginFailed, HasLoggedIn);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        HasLoggedIn = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.LoginPage.GoneWrong);
                    }                                                                               
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.LoginPage.NoLoginData);
            }
        }
    }
}
