using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Util.Jar;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PropertyChanged;
using Xamarin.Forms;
using System.Net.Http;
using Android.Views;
using Newtonsoft.Json;
using Beeple_office.Utils;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class OrderSandwichesPageViewModel : INotifyPropertyChanged
    {
        #region Commands
        public ICommand AddOneCommand { get; set; }
        public ICommand GoToResponsiblePageCommand { get; set; }
        public ICommand AddMinOneCommand { get; set; }
        public ICommand OrderCommand { get; set; }
        public ICommand ResponsibleCommand { get; set; }
        public ICommand CalculateTotalCommand { get; set; }

        #endregion
        [DoNotNotify]
        public string SortBread { get; set; }

        public bool Ordered;
        public int Number { get; set; }
        public bool SmosYn { get; set; }
        [DoNotNotify]
        public double Sauce { get; set; }
        [DoNotNotify]
        public double Smos { get; set; }
        public string SauceOne { get; set; }
        public string SauceTwo { get; set; }
        public double TotalPrice { get; set; }
        [DoNotNotify]
        //public string CurrentDate { get; set; }
        public Sandwiches OrderedSandwiches { get; set; }

        private ObservableCollection<Sandwiches> _sandwichesList;
        public ObservableCollection<Sandwiches> SandwichesList
        {
            get { return _sandwichesList; }
            set
            {
                _sandwichesList = value;
                OnPropertyChanged("SandwichesList");
            }
        }

        public List<string> SauceListOne { get; set; }
        public List<string> SauceListTwo { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
            }
        }

        public OrderSandwichesPageViewModel()
        {
            IsLoading = true;
            Sauce = 0.20;
            Smos = 0.50;
            TotalPrice = 0.00;
            InitCommands();
            SauceListOne = new List<string>
            { "Mayonaise", "Cocktail", "Andalouse", "Ketchup", "Tartar", "Mustard", "Curry", "Tabasco", "Mammout"};
            SauceListOne = new List<string>(SauceListOne.OrderBy(i => i));
            SauceListTwo = new List<string>
            { "Mayonaise", "Cocktail", "Andalouse", "Ketchup", "Tartar", "Mustard", "Curry", "Tabasco", "Mammout"};
            SauceListTwo = new List<string>(SauceListTwo.OrderBy(i => i));

            SandwichesList = new ObservableCollection<Sandwiches>() {
                new Sandwiches()
                {
                    Name = "Kaas",
                    Price = 2.30
                },
                  new Sandwiches()
                {
                    Name = "Kip curry",
                    Price = 2.70
                },
                  new Sandwiches()
                {
                    Name = "Haringsalade",
                    Price = 2.50
                },
                  new Sandwiches()
                {
                    Name = "Kaas en hesp",
                    Price = 2.60
                },
                  new Sandwiches()
                {
                    Name = "Zalmsalade",
                    Price = 2.90
                },
                  new Sandwiches()
                {
                    Name = "Martino",
                    Price = 3.20
                },
                  new Sandwiches()
                {
                    Name = "Hawaï",
                    Price = 3.20
                },
                  new Sandwiches()
                {
                    Name = "Eiersalade",
                    Price = 2.50
                },
                  new Sandwiches()
                {
                    Name = "Préparé",
                    Price = 2.70
                }
            };
            SandwichesList = new ObservableCollection<Sandwiches>(SandwichesList.OrderBy(i => i.Name));
            IsLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void InitCommands()
        {
            GoToResponsiblePageCommand = new Command(() => { GoToAllOrders(); });
            AddOneCommand = new Command(() => { AddOne(); });
            AddMinOneCommand = new Command(() => { AddMinOne(); });
            OrderCommand = new Command(() => { ShowAll(); });
            CalculateTotalCommand = new Command(() =>
            {
                if (Number != 0 && SortBread != null && OrderedSandwiches.Name != "")
                {
                    CalculateTotalPrice();
                }
                else
                {
                    IsLoading = false;
                    MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.EmptyFields);
                }
            });
        }

        private void GoToAllOrders()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.NavigateToAllOrdersPage);
        }

        private void AddOne()
        {
            Number += 1;
            Debug.WriteLine("plus: "+ Number);
        }
        
        private void AddMinOne()
        {
            if (Number != 0)
            {
                Number -= 1;
                Debug.WriteLine("min: " + Number);
            }
        }

        private async void ShowAll()
        {
            IsLoading = true;
            if (Number != 0 && SortBread != null && OrderedSandwiches.Name != "")
            {
                CalculateTotalPrice();
                await Order();
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.EmptyFields);
            }
        }

        private void CalculateTotalPrice()
        {
            if (SmosYn)
            {
                if (SauceOne != null)
                {
                    if (SauceTwo != null)
                    {
                        TotalPrice = (OrderedSandwiches.Price + Smos + Sauce * 2) * Number;
                        Debug.WriteLine("1:");
                        Debug.WriteLine(TotalPrice);
                    }
                    else
                    {
                        TotalPrice = (OrderedSandwiches.Price + Smos + Sauce) * Number;
                        Debug.WriteLine("2:");
                        Debug.WriteLine(TotalPrice);
                    }
                }
                else
                {
                    if (SauceTwo != null)
                    {
                        TotalPrice = (OrderedSandwiches.Price + Smos + Sauce) * Number;
                        Debug.WriteLine("3:");
                        Debug.WriteLine(TotalPrice);
                    }
                    else
                    {
                        TotalPrice = (OrderedSandwiches.Price + Smos) * Number;
                        Debug.WriteLine("4:");
                        Debug.WriteLine(TotalPrice);
                    }
                }
            }
            else
            {
                if (SauceOne != null)
                {
                    if (SauceTwo != null)
                    {
                        TotalPrice = (OrderedSandwiches.Price + Sauce * 2) * Number;
                        Debug.WriteLine("5:");
                        Debug.WriteLine(TotalPrice);
                    }
                    else
                    {
                        TotalPrice = (OrderedSandwiches.Price + Sauce) * Number;
                        Debug.WriteLine("6:");
                        Debug.WriteLine(TotalPrice);
                    }
                }
                else
                {
                    if (SauceTwo != null)
                    {
                        TotalPrice = (OrderedSandwiches.Price + Sauce) * Number;
                        Debug.WriteLine("7:");
                        Debug.WriteLine(TotalPrice);
                    }
                    else
                    {
                        TotalPrice = OrderedSandwiches.Price * Number;
                        Debug.WriteLine("8:");
                        Debug.WriteLine(TotalPrice);
                    }
                }
            }
            Debug.WriteLine(TotalPrice);
        }

        public async Task Order()
        {
            if (UserLoggedIn.User != null)
            {
                using (var c = new HttpClient())
                {
                    try
                    {
                        var jsonRequest = new { email = UserLoggedIn.User, bread = OrderedSandwiches.Name, amount = Number, totalPrice = TotalPrice, sauceOne = SauceOne, sauceTwo = SauceTwo, vegetables = SmosYn, sortBread = SortBread };

                        var serializedJsonRequest = JsonConvert.SerializeObject(jsonRequest);
                        HttpContent content = new StringContent(serializedJsonRequest, Encoding.UTF8, "application/json");
                        content.Headers.Add("x-access-token", UserLoggedIn.Token);
                        Debug.WriteLine(content);

                        var response = await c.PostAsync(ConnectionLinks.SandwichesAddress, content);
                        Debug.WriteLine(response);

                        var resultJson = await response.Content.ReadAsStringAsync();
                        var message = JsonConvert.DeserializeObject<ResponseLoginMessage>(resultJson);
                        Debug.WriteLine(message.Success);

                        if (message.Success == "true")
                        {
                            Ordered = true;
                            IsLoading = false;
                            MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.OrderSandwiche, Ordered);
                        }
                        else
                        {
                            Ordered = false;
                            if (!Ordered)
                            {
                                IsLoading = false;
                                MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.OrderSandwiche, Ordered);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Ordered = false;
                        IsLoading = false;
                        Debug.WriteLine(e.Message);
                        MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.GoneWrong);
                    }
                }
            }
            else
            {
                Ordered = false;
                IsLoading = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.OrderSandwichesPage.GoneWrong);
            }
        }
    }
}
