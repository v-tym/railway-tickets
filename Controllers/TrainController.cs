using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Test.Models;

namespace Test.Controllers
{
    public class TrainController: Controller
    {
        ModelContext db = new ModelContext();
        public ActionResult Index ()
        {
            IEnumerable<Train> trains = db.Trains;
            ViewBag.trains = trains;           
            return View();
        }

        public ActionResult Delete(int Id)
        {
            Train train = db.Trains.Find(Id);
            if (train == null)
            {
                return HttpNotFound();
            }
            db.Trains.Remove(train);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Train train)
        {
            Train trainDb = db.Trains.FirstOrDefault(t => t.Number == train.Number);
            if (train == null)
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены");
            }
            if (trainDb != null)
            {
                ModelState.AddModelError("", "Такой поезд уже есть");
            }
            if (train.Number < 0)
            {
                ModelState.AddModelError("", "Отрицательные числа не допускаются");
            }
            if (ModelState.IsValid)
            {
                db.Trains.Add(train);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(train);
           
        }
        [HttpGet]
        public ActionResult Change(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Train train = db.Trains.Find(id);
            if (train != null)
            {
                return View(train);
            }
            return HttpNotFound();           
        }
        [HttpPost]
        public ActionResult Change(Train train)
        {                       
            Train trainDb = db.Trains.FirstOrDefault(t => t.Number == train.Number && t.Id != train.Id);
            if (train == null)
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены");
            }            
            if (trainDb != null)
            {
                ModelState.AddModelError("", "Такой поезд уже есть");
            }
            if (train.Number < 0)
            {
                ModelState.AddModelError("", "Отрицательные числа не допускаются");
            }
            if (ModelState.IsValid)
            {
                db.Entry(train).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(train);
        }
    }
}