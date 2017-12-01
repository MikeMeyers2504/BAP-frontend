using Beeple_office.Utils;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Beeple_office.ViewModels
{
    [ImplementPropertyChanged]
    public class HamburgerViewModel
    {
        public ICommand GoHomeCommand { get; set; }
        public ICommand ChangeToEnglishCommand { get; set; }
        public ICommand ChangeToFrenchCommand { get; set; }
        public ICommand ChangeToDutchCommand { get; set; }
        public bool French { get; set; }
        public bool Dutch { get; set; }
        public bool English { get; set; }
        public bool LoggedIn { get; set; }

        public HamburgerViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            ChangeToEnglishCommand = new Command(() => { EnglishBool(); });
            ChangeToFrenchCommand = new Command(() => { FrenchBool(); });
            ChangeToDutchCommand = new Command(() => { DutchBool(); });
            if (UserLoggedIn.User != null)
            {
                LoggedIn = true;
            }
            else
            {
                LoggedIn = false;
            }
        }
        private void FrenchBool()
        {
            French = true;
            English = false;
            Dutch = false;
            Change();
        }
        private void DutchBool()
        {
            Dutch = true;
            English = false;
            French = false;
            Change();
        }
        private void EnglishBool()
        {
            English = true;
            French = false;
            Dutch = false;
            Change();
        }
        private void Change()
        {
            if (French)
            {
                Debug.WriteLine("FRENCH: " + French);
                CultureInfo ci = new CultureInfo("fr");
                AppResources.Culture = ci;
                French = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.HamburgerMenu.LanguageChanged);
            }
            else if (Dutch)
            {
                Debug.WriteLine("DUTCH: " + Dutch);
                CultureInfo ci = new CultureInfo("nl");
                AppResources.Culture = ci;
                Dutch = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.HamburgerMenu.LanguageChanged);
            }
            else
            {
                Debug.WriteLine("ENGELS:" + English);
                CultureInfo ci = new CultureInfo("en");
                AppResources.Culture = ci;
                English = false;
                MessagingCenter.Send(this, Constants.MessagingCenter.HamburgerMenu.LanguageChanged);
            }
        }

        public void GoHome()
        {
            MessagingCenter.Send(this, Constants.MessagingCenter.HamburgerMenu.Logout);
        }
    }
}
