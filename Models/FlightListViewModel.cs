using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test.Models
{
    public class FlightListViewModel
    {
        public IEnumerable<Flight> Flights { get; set; }
        public SelectList StationCome { get; set; }
       // public DateTime DateOut { get; set; }
    }
}