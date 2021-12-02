using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class Station
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Come> Comes { get; set; }
        public ICollection<Out> Outs { get; set; }
        public Station()
        {
            Comes = new List<Come>();
            Outs = new List<Out>();
        }
    }
}