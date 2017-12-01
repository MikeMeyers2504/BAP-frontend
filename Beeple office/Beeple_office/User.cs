using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beeple_office
{
    public class User
    {
        public String Email { get; set; }
        public String Avatar { get; set; }

        public String Password { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String SecretOne { get; set; }
        public String SecretTwo { get; set; }
        public Boolean IsSelected { get; set; }
    }
}
