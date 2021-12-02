using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using System.Data.Entity;

namespace Test.Controllers
{
    public class FlightController : Controller
    {
        ModelContext db = new ModelContext();

        public ActionResult Index()
        {
            var flights = db.Flights
                .Include(c => c.Come.Station)
                .Include(c => c.Out.Station)
                .Include(c => c.Trains);           
            ViewBag.flights = flights;
            return View();
        }

        public ActionResult Index2(int? stationCome, DateTime? dateOut)
        {
            IQueryable<Flight> flights = db.Flights
                .Include(c => c.Come.Station)
                .Include(c => c.Out.Station)
                .Include(c => c.Trains);

            if (stationCome != null && stationCome != 0)
            {
                if (stationCome == 1)
                {
                    flights = flights.Where(f => f.Come.StationId == null);
                }
                else
                {
                    flights = flights.Where(f => f.Come.StationId == stationCome);
                }                
            }           
            if (dateOut.HasValue)
            {
                DateTime dateTime = new DateTime();
                dateTime = (DateTime)dateOut;
                flights = flights.Where(f => f.Come.Date.Day == dateTime.Day && f.Come.Date.Month == dateTime.Month && f.Come.Date.Year == dateTime.Year);
            }

            List<Station> stations = db.Stations.ToList();
            stations.Insert(0, new Station { Name = "Все", Id = 0 });
            stations.Insert(1, new Station { Name = "Без станции", Id = 1 });
            FlightListViewModel flvm = new FlightListViewModel 
            {
                Flights = flights.ToList(),
                StationCome = new SelectList(stations, "Id", "Name")                
            };

            return View(flvm);
        }

        public ActionResult Detail(int id)
        {
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        [HttpGet]
        public ActionResult Delete (int id)
        {
            Flight flight = db.Flights.Find(id);
            if (flight != null)
            {
                db.Flights.Remove(flight);
                db.SaveChanges();
                return RedirectToAction("Index2");
            }
            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Create()
        {
            IEnumerable<Train> trains = db.Trains;
            ViewBag.Trains = trains;
            IEnumerable<Station> stations = db.Stations;
            ViewBag.Station = stations;
            return View();
        }

        [HttpPost]
        public ActionResult Create( int? FreeSeats, 
                                    int? ComeStation, 
                                    DateTime? dateCome, 
                                    int? OutStation, 
                                    DateTime? dateOut,
                                    int[] selectedTrains)
        {
            
            IEnumerable<Train> trains = db.Trains;
            ViewBag.Trains = trains;
            IEnumerable<Station> stations = db.Stations;
            ViewBag.Station = stations;
            if (FreeSeats == null || ComeStation == null || dateCome == null || OutStation == null || dateOut == null || selectedTrains == null)
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены");
            }
            if (dateCome>dateOut || dateCome == dateOut)
            {
                ModelState.AddModelError("", "Дата отправления должна быть больше даты прибытия");
            }
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Валидация пройдена";
                Station comeStation = db.Stations.Find(ComeStation);
                Station outStation = db.Stations.Find(OutStation);
                Come come = new Come();
                come.Station = comeStation;
                come.Date = (DateTime)dateCome;
                db.Comes.Add(come);

                Out _out = new Out();
                _out.Station = outStation;
                _out.Date = (DateTime)dateOut;
                db.Outs.Add(_out);

                Flight flight = new Flight();
                flight.Out = _out;
                flight.Come = come;
                flight.FreeSeats = (int)FreeSeats;

                List<Train> trainList = new List<Train>() { };
                List<Train> errorMessages = new List<Train>() { };

                foreach (Train i in db.Trains.Where(t => selectedTrains.Contains(t.Id)).Include(c => c.Flights))
                {
                    trainList.Add(i);
                }
                
                foreach (Train train in trainList)
                {                   
                    if (!findTimeIntersection(flight, train))
                    {
                        flight.Trains.Add(train);
                    }
                    else { ModelState.AddModelError("", $"{train.Number}" + " поезд не добавлен, пересечение во времени"); }
                }

                if (ModelState.IsValid)
                {
                    db.Flights.Add(flight);
                    db.SaveChanges();
                    return RedirectToAction("Index2");
                }

                ViewBag.Message = "Запрос не прошел валидацию";
                return View();
            }     

            ViewBag.Message = "Запрос не прошел валидацию";
            return View();
        }

        [HttpGet]
        public ActionResult Change(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Flight flight = db.Flights.Find(id);
            
            if (flight != null)
            {
                ViewBag.flightId = flight.Id;
                IEnumerable<Station> stations = db.Stations;
                ViewBag.Station = stations;
                Come come = db.Comes.Find(flight.ComeId);
                ViewBag.ComeId = come.Id;
                ViewBag.SelectedCome = come.StationId;
                ViewBag.DateCome = come.Date.ToString("yyyy-MM-ddThh:mm");
                Out _out = db.Outs.Find(flight.OutId);
                ViewBag.OutId = _out.Id;
                ViewBag.SelectedOut = _out.StationId;
                ViewBag.DateOut = _out.Date.ToString("yyyy-MM-ddThh:mm");
                ViewBag.FreeSeats = flight.FreeSeats;
                IEnumerable<Train> trains = db.Trains;
                ViewBag.Trains = trains;
                ViewBag.SelectedTrains = flight.Trains;

                return View();
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult Change (int? flightId,
                                    int? FreeSeats,
                                    int? ComeId,
                                    int? ComeStation,
                                    DateTime? dateCome,
                                    int? OutId,
                                    int? OutStation,
                                    DateTime? dateOut,
                                    int[] selectedTrains)
        {
            Flight flight12 = db.Flights.Find(flightId);
            ViewBag.flightId = flight12.Id;
            IEnumerable<Station> stations = db.Stations;
            ViewBag.Station = stations;
            Come come11 = db.Comes.Find(flight12.ComeId);
            ViewBag.ComeId = come11.Id;
            ViewBag.SelectedCome = come11.StationId;
            ViewBag.DateCome = come11.Date.ToString("yyyy-MM-ddThh:mm");
            Out _out1 = db.Outs.Find(flight12.OutId);
            ViewBag.OutId = _out1.Id;
            ViewBag.SelectedOut = _out1.StationId;
            ViewBag.DateOut = _out1.Date.ToString("yyyy-MM-ddThh:mm");
            ViewBag.FreeSeats = flight12.FreeSeats;
            IEnumerable<Train> trains = db.Trains;
            ViewBag.Trains = trains;
            ViewBag.SelectedTrains = flight12.Trains;

            if (flightId == null || FreeSeats == null || ComeId == null ||
                ComeStation == null || dateCome == null || OutId == null ||
                OutStation == null || dateOut == null || selectedTrains == null)
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены");
            }
            if (dateCome > dateOut || dateCome == dateOut)
            {
                ModelState.AddModelError("", "Дата отправления должна быть больше даты прибытия");
            }
            if (ModelState.IsValid)
            {
                Come come = db.Comes.Find(ComeId);
                come.Station = db.Stations.Find(ComeStation);
                come.Date = (DateTime)dateCome;
                db.Entry(come).State = EntityState.Modified;

                Out _out = db.Outs.Find(OutId);
                _out.Station = db.Stations.Find(OutStation);
                _out.Date = (DateTime)dateOut;
                db.Entry(_out).State = EntityState.Modified;

                Flight flight = db.Flights.Find(flightId);
                flight.FreeSeats = (int)FreeSeats;
                flight.Come = come;
                flight.Out = _out;
                flight.Trains.Clear();

                List<Train> trainList = new List<Train>() { };
                List<Train> errorMessages = new List<Train>() { };               
               
                foreach (Train i in db.Trains.Where(t => selectedTrains.Contains(t.Id)).Include(c => c.Flights))
                {
                    trainList.Add(i);
                }

                foreach (Train train in trainList)
                {
                    if (!findTimeIntersection(flight, train))
                    {
                        flight.Trains.Add(train);
                    }
                    else { ModelState.AddModelError("", $"{train.Number}" + " поезд не добавлен, пересечение во времени"); }
                }
                if (ModelState.IsValid)
                {
                    db.Entry(flight).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index2");
                }

                ViewBag.Message = "Запрос не прошел валидацию";
                return View();
            }
            ViewBag.Message = "Запрос не прошел валидацию";
            return View();
        }

        private bool findTimeIntersection(Flight flight, Train selectedTrain)
        {            
            IEnumerable<Flight> ft = selectedTrain.Flights.Where(sft => sft.Id != flight.Id);
            if (ft.Count() != 0)
            {
                foreach (Flight f in ft)
                {                                        
                    Come come = db.Comes.Find(f.ComeId);
                    Out _out = db.Outs.Find(f.OutId);
                    bool OutIntesection = come.Date <= flight.Out.Date && flight.Out.Date <= _out.Date;
                    bool ComeIntesection = come.Date <= flight.Come.Date && flight.Come.Date <= _out.Date;
                    bool OutIntesection2 = flight.Come.Date <= _out.Date && _out.Date <= flight.Out.Date;
                    bool ComeIntesection2 = flight.Come.Date <= come.Date && come.Date <= flight.Out.Date;
                    if ((OutIntesection || ComeIntesection) ||(OutIntesection2 || ComeIntesection2))
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}