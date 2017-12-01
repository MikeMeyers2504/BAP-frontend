using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beeple_office
{
    public class Events
    {
        public String Email { get; set; }
        public string Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public String SortEvent { get; set; }
        public String Where { get; set; }
        public List<Invites> Invites { get; set; }
        public String Requirements { get; set; }
        public String Name { get; set; }
        public bool Transport { get; set; }
        public bool FoodEnDrinks { get; set; }
    }
}
