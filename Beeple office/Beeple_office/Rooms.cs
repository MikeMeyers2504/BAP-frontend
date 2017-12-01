using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beeple_office
{
    public class Rooms
    {
        public String Email { get; set; }
        public String Date { get; set; }
        public TimeSpan Start { get; set ; }
        public TimeSpan End { get; set; }
        public String Extras { get; set; }
        public String Room { get; set; }         
        //public List<NameAndEmail> Persons { get; set; }
        public List<User> Persons { get; set; }
    }
}
