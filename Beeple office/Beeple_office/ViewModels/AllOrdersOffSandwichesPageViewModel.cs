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
    public class AllOrdersOffSandwichesPageViewModel
    {
        #region Commands
        public ICommand DeleteAllCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand NormalCommand { get; set; }
        #endregion

        public string SearchBarText { get; set; }

        private ObservableCollection<AllSandwiches> _items;
        public ObservableCollection<AllSandwiches> Items
        {
            get { return _items; }
            set
            {
                _items = value;
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

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await GetAllOrders();

                    IsRefreshing = false;
                });
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

        public AllOrdersOffSandwichesPageViewModel()
        {
            ExecuteList();
            InitCommands();
        }

        private void InitCommands()
        {
            FilterCommand = new Command(async () => { await GetFilteredAllOrders(); });
            NormalCommand = new Command(async () => { await GetAllOrders(); });
            DeleteAllCommand = new Command(async () => { await DeleteAllOrders(); });
        }

        private async void ExecuteList()
        {
            IsLoading = true;
            await GetAllOrders();
        }

        public async Task GetAllOrders()
        {
            IsLoading = true;
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);
                        var response = await c.GetAsync(ConnectionLinks.SandwichesAddress);
                        Debug.WriteLine(response);
                        var content = await response.Content.ReadAsStringAsync();
                        Items = JsonConvert.DeserializeObject<ObservableCollection<AllSandwiches>>(content);
                        Items = new ObservableCollection<AllSandwiches>(Items.OrderBy(i => i.Email));
                        IsLoading = false;
                        foreach (var item in Items)
                        {
                            Debug.WriteLine(item.TotalPrice);
                            Debug.WriteLine(item.Vegetables);
                        }
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
                    }
                }
            }else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
            }
        }

        public async Task GetFilteredAllOrders()
        {
            IsLoading = true;
            if (UserLoggedIn.User != null && SearchBarText != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.GetAsync(ConnectionLinks.SandwichesAddress + "?email=" + SearchBarText);
                        Debug.WriteLine(response);

                        var content = await response.Content.ReadAsStringAsync();
                        Items = JsonConvert.DeserializeObject<ObservableCollection<AllSandwiches>>(content);
                        Items = new ObservableCollection<AllSandwiches>(Items.OrderBy(i => i.Email));
                        IsLoading = false;
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
                    }
                    
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
            }
        }
    
        public async Task DeleteAllOrders()
        {
            IsLoading = true;
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", UserLoggedIn.Token);

                        var response = await c.DeleteAsync(ConnectionLinks.SandwichesAddress);
                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(response);
                        Debug.WriteLine(resultJson);
                        Debug.WriteLine(message);

                        if (message.Success == "true")
                        {
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.DeleteAll);
                        }
                        else
                        {
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
                        }
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
                    }              
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.AllOrdersOffSandwichesPage.GoneWrong);
            }
        }
    }
}
