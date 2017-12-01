using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beeple_office
{
    public static class ConnectionLinks
    {
        public static Uri BaseAddress { get; set; }
        public static Uri AuthenticateAddress { get; set; }
        public static Uri UsersAddress { get; set; }
        public static Uri CheckinsAddress { get; set; }
        public static Uri CheckoutsAddress { get; set; }
        public static Uri EventsAddress { get; set; }
        public static Uri RoomsAddress { get; set; }
        public static Uri RoomNamesAddress { get; set; }
        public static Uri SandwichesAddress { get; set; }

        static ConnectionLinks()
        {
            BaseAddress = new Uri("https://beeple-backend-app.herokuapp.com/api");
            AuthenticateAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/authenticate");
            UsersAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/users");
            CheckinsAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/checkins");
            CheckoutsAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/checkouts");
            EventsAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/events");
            RoomsAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/rooms");
            RoomNamesAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/roomNames");
            SandwichesAddress = new Uri("https://beeple-backend-app.herokuapp.com/api/sandwiches");
        }
    }
}
