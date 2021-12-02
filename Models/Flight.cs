using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public int FreeSeats { get; set; }
        public int? ComeId { get; set; }
        public Come Come { get; set; }
        public int? OutId { get; set; }
        public Out Out { get; set; }
        public virtual ICollection<Train> Trains { get; set; }
        public Flight()
        {
            Trains = new List<Train>();
        }
    }

}