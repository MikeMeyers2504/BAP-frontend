using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beeple_office.Utils
{
    public static class Constants
    {
        public static class MessagingCenter
        {
            public static class MenuPage
            {
                public static readonly string NavigateToCheckPage = "NavigateToCheckPage_MenuPage";
                public static readonly string NavigateToSandwichesPage = "NavigateToSandwichesPage_MenuPage";
                public static readonly string NavigateToRoomsPage = "NavigateToRoomsPage_MenuPage";
                public static readonly string NavigateToEventsPage = "NavigateToEventsPage_MenuPage";
            }

            public static class RegisterPage
            {
                public static readonly string NavigateToLoginPage = "NavigateToLoginPage_RegisterPage";
                public static readonly string PasswordDoesnotMatch = "PasswordDoesnotMatch_RegisterPage";
                public static readonly string EmptyFields = "EmptyFields_RegisterPage";
                public static readonly string ShowHide = "ShowHide_RegisterPage";
                public static readonly string GoneWrong = "GoneWrong_RegisterPage";
                public static readonly string EmailValidate = "EmailValidate_RegisterPage";
            }

            public static class LoginPage
            {
                public static readonly string NavigateToRegisterPage = "NavigateToRegisterPage_LoginPage";
                public static readonly string LoginFailed = "LoginFailed_LoginPage";
                public static readonly string NavigateToMenuPage = "NavigateToMenuPage_LoginPage";
                public static readonly string NoLoginData = "NoLoginData_LoginPage";
                public static readonly string NavigateToForgotPasswordPage = "NavigateToForgotPasswordPage_LoginPage";
                public static readonly string GoneWrong = "GoneWrong_LoginPage";
            }

            public static class HamburgerMenu
            {
                public static readonly string Logout = "Logout_HamburgerMenu";
                public static readonly string LanguageChanged = "LanguageChanged_HamburgerMenu";
            }

            public static class ForgotPasswordPage
            {
                public static readonly string NavigateToLoginPage = "NavigateToLoginPage_ForgotPasswordPage";
                public static readonly string PasswordDoesnotMatch = "PasswordDoesnotMatch_ForgotPasswordPage";
                public static readonly string EmptyFields = "EmptyFields_ForgotPasswordPage";
                public static readonly string ShowHide = "ShowHide_ForgotPasswordPage";
                public static readonly string Error = "Error_ForgotPasswordPage";
                public static readonly string GoneWrong = "GoneWrong_ForgotPasswordPage";
            }

            public static class CheckinCheckoutPage
            {
                public static readonly string Checkin = "Checkin_CheckinCheckoutPage";
                public static readonly string Checkout = "Checkout_CheckinCheckoutPage";
                public static readonly string GoneWrong = "GoneWrong_CheckinCheckoutPage";
            }

            public static class OrderSandwichesPage
            {
                public static readonly string OrderSandwiche = "OrderSandwiche_OrderSandwichesPage";
                public static readonly string GoneWrong = "GoneWrong_OrderSandwichesPage";
                public static readonly string EmptyFields = "EmptyFields_OrderSandwichesPage";
                public static readonly string NavigateToAllOrdersPage = "NavigateToAllOrdersPage_OrderSandwichesPage";

                public static readonly string NotPermitted = "NotPermitted_OrderSandwichesPage";
                public static readonly string Permitted = "Permitted_OrderSandwichesPage";
            }
            public static class AllOrdersOffSandwichesPage
            {
                public static readonly string GoneWrong = "GoneWrong_AllOrdersOffSandwichesPage";
                public static readonly string DeleteAll = "DeleteAll_AllOrdersOffSandwichesPage";
            }
            public static class RoomsPage
            {
                public static readonly string NavigateToNewRoom = "NavigateToNewRoom_RoomsPage";
                public static readonly string SelectedRow = "SelectedRow_RoomsPage";
                public static readonly string NavigateToFilterRooms = "NavigateToFilterRooms_RoomsPage";
                public static readonly string GoneWrong = "GoneWrong_RoomsPage";
            }
            public static class NewRoomReservationPage
            {
                public static readonly string RoomReserved = "RoomReserved_NewRoomReservationPage";
                public static readonly string TimeSmaller = "TimeSmaller_NewRoomReservationPage";
                public static readonly string ReservedError = "ReservedError_NewRoomReservationPage";
                public static readonly string AddingRoomError = "AddingRoomError_NewRoomReservationPage";
                public static readonly string RoomAdded = "RoomAdded_NewRoomReservationPage";
                public static readonly string GoneWrong = "GoneWrong_NewRoomReservationPage";
            }
            public static class FilterRoomsPage
            {
                public static readonly string NavigateToNewRoom = "NavigateToNewRoom_FilterRoomsPage";
                public static readonly string Error = "Error_FilterRoomsPage";
                public static readonly string UnselectRow = "UnselectRow_FilterRoomsPage";
                public static readonly string Nothing = "Nothing_FilterRoomsPage";
                public static readonly string GoneWrong = "GoneWrong_FilterRoomsPage";
            }
            public static class EventsPage
            {
                public static readonly string NavigateToNewEvent = "NavigateToNewEvent_EventsPage";
                public static readonly string SelectedRow = "SelectedRow_EventsPage";
                public static readonly string NavigateToFilterEvents = "NavigateToFilterEvents_EventsPage";
                public static readonly string NoPermission = "NoPermission_EventsPage";
                public static readonly string ChangedSuccess = "ChangedSuccess_EventsPage";
                public static readonly string ChangedFailed = "ChangedFailed_EventsPage";
                public static readonly string GoneWrong = "GoneWrong_EventsPage";
            }
            public static class NewEventPage
            {
                public static readonly string NewEvent = "NewEvent_NewEventPage";
                public static readonly string TimeSmaller = "TimeSmaller_NewEventPage";
                public static readonly string EventError = "EventError_NewEventPage";
                public static readonly string GoneWrong = "GoneWrong_NewEventPage";
            }
            public static class FilterEventsPage
            {
                public static readonly string NavigateToNewEvent = "NavigateToNewEvent_FilterEventsPage";
                public static readonly string Error = "Error_FilterEventsPage";
                public static readonly string Nothing = "Nothing_FilterEventsPage";
                public static readonly string GoneWrong = "GoneWrong_FilterEventsPage";
            }
        }
    }
}
