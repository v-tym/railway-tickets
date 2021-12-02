using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class Train
    {
        public int Id { get; set; }        
        public int Number { get; set; }
        public int QttSeats { get; set; }
        public virtual ICollection<Flight> Flights { get; set; }
        public Train()
        {
            Flights = new List<Flight>();
        }
    }
}