using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class ModelContext: DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<Come> Comes { get; set; }
        public DbSet<Out> Outs { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Flight> Flights { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>().HasMany(c => c.Trains)
           .WithMany(s => s.Flights)
           .Map(t => t.MapLeftKey("FlightId")
           .MapRightKey("TrainId")
           .ToTable("FlightTrain"));
        }
    }

   
}