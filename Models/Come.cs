using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class Come
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? StationId { get; set; }
        public Station Station { get; set; }
        public ICollection<Flight> Flights { get; set; }
        public Come()
        {
            Flights = new List<Flight>();
        }
    }
}