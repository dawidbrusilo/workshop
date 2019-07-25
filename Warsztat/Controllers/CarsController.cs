using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Warsztat.DataContexts;
using Warsztat.Models;

namespace Warsztat.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private IdentityDb db = new IdentityDb();

        public ActionResult History(int? id)
        {
            var currentUserId = User.Identity.GetUserId();
           
            var carAdminReports = from r in db.Reports
                                  where r.CarID == (id)
                                  select r;
            var carReports = from r in db.Reports
                             join c in db.Cars on r.CarID equals c.Id into cr
                             from d in cr
                             where r.CarID == (id) && d.ApplicationUserID ==(currentUserId)
                             select r;
            if (id == null)
            {
                return HttpNotFound();
            }
            

            if (User.IsInRole("Admin"))
            {
                return View(carAdminReports.ToList());
            }
            if (!carReports.Any())
            {
                return View("Error");
            }
            return View(carReports.ToList());
        }

        // GET: Cars
        public ActionResult Index()
        {

            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);


            var userCars = user.Cars
                .ToList();


            if (User.IsInRole("Admin"))
            {
                return View(db.Cars.ToList());
            }

            return View(userCars);
        }

        // GET: Cars/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            var userCars = user.Cars.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = userCars.Where(c => c.Id == id).FirstOrDefault();
            if (User.IsInRole("Admin"))
            {
                 car = db.Cars.Find(id);
            }
           
             
           if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {

            var customers = (from c in db.Users
                             where (c.Name.StartsWith(prefix)|| c.LastName.StartsWith(prefix))
                             select new
                             {
                                 label = c.Name + " " + c.LastName,
                                 val = c.Id
                             }).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        
       

    // GET: Cars/Create
    public ActionResult Create()
        {
           
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            ViewBag.Value= user.Id;
           // ViewBag.Owner = user.UserName;
           
            return View();
        }

        // POST: Cars/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string CustomerName, string CustomerId,[Bind(Include = "Id,Make,Model,Engine,DateOfProduction,PlateNumber,Vin,ApplicationUserID")] Car car)
        {
            ViewBag.Message = "CustomerName: " + CustomerName + " CustomerId: " + CustomerId;
            if (ModelState.IsValid)
            {
                
               
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            var userCars = user.Cars.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = userCars.Where(c => c.Id == id).FirstOrDefault();
            if (User.IsInRole("Admin"))
            {
                 car = db.Cars.Find(id);
            }
           
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Make,Model,Engine,DateOfProduction,PlateNumber,Vin")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        [Authorize(Roles ="Admin")]
        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }
        [Authorize(Roles = "Admin")]
        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
