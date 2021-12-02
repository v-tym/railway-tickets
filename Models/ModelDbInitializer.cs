using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class ModelDbInitializer: DropCreateDatabaseAlways<ModelContext>
    {
        protected override void Seed(ModelContext db)
        {
            Station s1 = new Station { Name = "Odessa" };
            Station s2 = new Station { Name = "Kyiv" };
            Station s3 = new Station { Name = "Chernigiv" };
            db.Stations.Add(s1);
            db.Stations.Add(s2);
            db.Stations.Add(s3);

            Come come1 = new Come { Station = s1, Date = new DateTime(2019, 04, 10) };
            Come come2 = new Come { Station = s2, Date = new DateTime(2019, 08, 11) };
            Come come3 = new Come { Station = s3, Date = new DateTime(2019, 09, 10) };
            db.Comes.Add(come1);
            db.Comes.Add(come2);
            db.Comes.Add(come3);

            Out Out1 = new Out { Station = s1, Date = new DateTime(2019, 08, 10) };
            Out Out2 = new Out { Station = s2, Date = new DateTime(2019, 08, 12) };
            Out Out3 = new Out { Station = s3, Date = new DateTime(2019, 09, 12) };
            db.Outs.Add(Out1);
            db.Outs.Add(Out2);
            db.Outs.Add(Out3);

            Train train1 = new Train { Number = 423, QttSeats = 500 };
            Train train2 = new Train { Number = 424, QttSeats = 550 };
            Train train3 = new Train { Number = 425, QttSeats = 620 };
            db.Trains.Add(train1);
            db.Trains.Add(train2);
            db.Trains.Add(train3);

            Flight flight1 = new Flight { Come = come1, Out = Out1, FreeSeats = 50, Trains = new List<Train>() { train1, train2} };
            Flight flight2 = new Flight { Come = come2, Out = Out2, FreeSeats = 50, Trains = new List<Train>() { train2 } };
            Flight flight3 = new Flight { Come = come3, Out = Out3, FreeSeats = 50, Trains = new List<Train>() { train2, train3 } };
            db.Flights.Add(flight1);
            db.Flights.Add(flight2);
            db.Flights.Add(flight3);

            base.Seed(db);
        }
    }
}