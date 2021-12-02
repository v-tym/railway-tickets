using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;

namespace Test.Controllers
{
    public class StationController : Controller
    {
        ModelContext db = new ModelContext();

        public ActionResult Index() {

            IEnumerable<Station> stations = db.Stations;

            ViewBag.Station = stations;

            return View();
        }

        [HttpGet]
        public ActionResult Change(int? Id) {

            if (Id == null)
            {
                return HttpNotFound();
            }
            Station station = db.Stations.Find(Id);
            if (station != null) 
            {                
                return View(station);
            }            
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Change(Station station) {

            Station trainDb = db.Stations.FirstOrDefault(s => s.Name == station.Name && s.Id != station.Id);
            if (station == null)
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены");
            }
            if (trainDb != null)
            {
                ModelState.AddModelError("", "Такая станция уже есть");
            }
            if (ModelState.IsValid)
            {
                db.Entry(station).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(station);
        }

        [HttpGet]
        public ActionResult GreateStation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GreateStation(Station station)
        {
            Station trainDb = db.Stations.FirstOrDefault(s => s.Name == station.Name);
            if (station == null)
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены");
            }
            if (trainDb != null)
            {
                ModelState.AddModelError("", "Такая станция уже есть");
            }
            if (ModelState.IsValid)
            {
                db.Stations.Add(station);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(station);
                       
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            // Station s = db.Stations.Find(id);
            Station s = db.Stations
                .Include(st => st.Comes)
                .Include(st => st.Outs)
                .FirstOrDefault(st => st.Id == id);
            if (s == null)
            {
                return HttpNotFound();
            }           
            db.Stations.Remove(s);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}