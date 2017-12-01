using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Text;
using Beeple_office.Pages;
using Xamarin.Forms;
using Beeple_office.Utils;
using System.Globalization;
using PropertyChanged;

namespace Beeple_office.ViewModels
{
    public class MenuPageViewModel
    {

        #region Commands
        public ICommand ToChecksPageCommand { get; set; }
        public ICommand ToSandwichesPageCommand { get; set; }
        public ICommand ToEventsPageCommand { get; set; }
        public ICommand ToRoomsPageCommand { get; set; }

        #endregion

        public MenuPageViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            ToChecksPageCommand = new Command(() => { SendC(); });
            ToSandwichesPageCommand = new Command(() => { SendS(); });
            ToEventsPageCommand = new Command(() => { SendE(); });
            ToRoomsPageCommand = new Command(() => { SendR(); });
        }

        private void SendC()
        {
            Debug.WriteLine("page Check-ins/outs");
            MessagingCenter.Send(this, Constants.MessagingCenter.MenuPage.NavigateToCheckPage);
        }
        private void SendS()
        {
            Debug.WriteLine("page sandwiches");
            MessagingCenter.Send(this, Constants.MessagingCenter.MenuPage.NavigateToSandwichesPage);
        }
        private void SendR()
        {
            Debug.WriteLine("page rooms");
            MessagingCenter.Send(this, Constants.MessagingCenter.MenuPage.NavigateToRoomsPage);
        }
        private void SendE()
        {
            Debug.WriteLine("page events");
            MessagingCenter.Send(this, Constants.MessagingCenter.MenuPage.NavigateToEventsPage);
        }       
    }
}
