using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Warsztat.DataContexts;
using Warsztat.Gmail;
using Warsztat.Models;

namespace Warsztat.Controllers
{   [Authorize]
    public class ReportsController : Controller
    {
        private IdentityDb db = new IdentityDb();

        public ReportsController()
        {
            db = new IdentityDb();
            db.ConfigureUsername(() => User.Identity.Name);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult History(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;




            var adminReports = from r in db.Reports
                               select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                adminReports = adminReports.Where(s => s.Car.Model.Contains(searchString) || s.Car.Vin.Contains(searchString)|| s.Car.PlateNumber.Contains(searchString) || s.ApplicationUser.LastName.Contains(searchString) || s.ApplicationUser.UserName.Contains(searchString));
            }

          
           

            int pageSize = 8;
            int pageNumber = (page ?? 1);
           
            
                return View(adminReports.ToList().ToPagedList(pageNumber, pageSize));
           
        }


        // GET: Reports
        public ActionResult Index(int? search, int? page, string stat)
        {
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            var reports = user.Reports.ToList();

            if (User.IsInRole("Admin"))
            {
                reports = db.Reports.ToList();
            }


            switch (stat)
            {
                case "Nowe":
                reports = reports.Where(r => r.State.ToString().Contains("Nowe")).ToList();
                break;
                case "Obsługiwane":
                    reports = reports.Where(r => r.State.ToString().Contains("Obsługiwane")).ToList();
                break;
                case "Zakończone":
                    reports = reports.Where(r => r.State.ToString().Contains("Zakończone")).ToList();
                    break;

            }
            ViewBag.CurrentSort = stat;
          

            if (search != null)
                {
                    reports = reports.Where(s => s.Id == (search)).ToList();
                }

             
            if (search != null)
            {
                page = 1;
                    
            }
              
            int pageSize = 5;
            int pageNumber = (page ?? 1);
                
            return View(reports.ToList().ToPagedList(pageNumber, pageSize));
            
        }

        // GET: Reports/Details/5
        public ActionResult Details(int? id)
        {
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var userReports = user.Reports.ToList();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //CustomerMessages = db.CustomerMessages.Where(t => t.ReportId == id);
            Report report = userReports.Where(r => r.Id == id).FirstOrDefault();
            if (User.IsInRole("Admin"))
            {
                report = db.Reports.Find(id);
            }
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report );
        }


        [HttpPost]
        public ActionResult CreateMessage([Bind(Include = "Id,Message,ReportId")] CustomerMessages model)
        {


            if (ModelState.IsValid)
            {

                model.Date = DateTime.Now;
                model.User = User.Identity.Name;
                db.CustomerMessages.Add(model);
                db.SaveChanges();
            }

            //var mailAdmin = (from r in db.Reports
            //                where (r.Id == model.ReportId)
            //                select r.PersonAssigned).FirstOrDefault();
            var mailUser = (from r in db.Reports
                           where (r.Id == model.ReportId)
                           select r.ReportedBy).Single();

           
           
              var  mailAdmin = "dawidbrusilo@hotmail.com";
                
           
            if (mailUser.Any())
            {
                mailUser.ToString();
            }





            SendEmail mail = new SendEmail();

            if (User.IsInRole("Admin"))
            {
                mail.ToEmail = mailUser;
            }
            else
            {
                mail.ToEmail = mailAdmin;
            }

            mail.Subject = "Wiadomość czatu dotycząca zgłoszenia nr." + model.ReportId + "!";
            mail.Body = "Użytkownik " + model.User + " napisał: <br>" +
                        model.Message;

            mail.IsHtml = true;
            mail.Send();

            return RedirectToAction("Details","Reports", new { id = model.ReportId });
        }


        public ActionResult ShowMessages(int? ReportId)
        {
            var messages = from m in db.CustomerMessages
                           where m.ReportId == ReportId
                           select m;
            

            return PartialView(messages);
        }

        public ActionResult Changes(string id)

        {
            var currentUserId = User.Identity.GetUserId();
           
           

            //var r = (from s in db.LogDetails
            //         join l in db.AuditLog
            //         on s.AuditLogId equals l.AuditLogId
            //         //join z in db.Reports
            //         //on s.AuditLogId equals z.Id
            //         where (l.RecordId == id) && (s.PropertyName != "CarId") /*&& (z.UserId == currentUserId) */       /* && (s.PropertyName != "UserId") && (s.OriginalValue != null)*/
            //         select s);

            var admin = (from s in db.LogDetails
                         join l in db.AuditLog
                         on s.AuditLogId equals l.AuditLogId
                         where (l.RecordId == id) && (s.PropertyName != "CarId")
                         select s);

            var normalUser = from a in admin
                             join z in db.Reports on a.AuditLogId equals z.Id
                             where (z.UserId ==currentUserId)
                             select a;





            if (User.IsInRole("Admin"))
            {
                return View(admin);
            }

            return View(normalUser);
        }

        //[HttpPost]
        //public JsonResult AutoComplete(string prefix)
        //{
        //var currentUserId = User.Identity.GetUserId();
        //ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
        //var myCars = (from c in user.Cars
        //              where c.Make.StartsWith(prefix)
        //              select new
        //              {
        //                  label = c.Make + " " + c.Model + " " + c.PlateNumber,
        //                  val = c.Id
        //              }).ToList();

        //    var cars = (from c in db.Cars
        //                where (c.Make.StartsWith(prefix) || c.Model.StartsWith(prefix)) || c.PlateNumber.StartsWith(prefix)
        //                select new
        //                {
        //                    label = c.Make + " " + c.Model + " " + c.PlateNumber,
        //                    val = c.Id
        //                     }).ToList();

        //    //if (User.IsInRole("Admin"))

        //        return Json(cars, JsonRequestBehavior.AllowGet);

        ////    return Json(myCars, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            //var myCars = (from c in user.Cars
            //              where c.Make.StartsWith(prefix)
            //              select new
            //              {
            //                  label = c.Make + " " + c.Model + " " + c.PlateNumber,
            //                  val = c.Id
            //              }).ToList();

            var cars = (from c in db.Cars
                        where (c.Make.StartsWith(prefix) || c.Model.StartsWith(prefix)) || c.PlateNumber.StartsWith(prefix)
                        select new
                        {
                            label = c.Make + " " + c.Model + " " + c.PlateNumber,
                            val = c.Id
                        }).ToList();

            return Json(cars, JsonRequestBehavior.AllowGet);
        }


        // GET: Reports/Create
        public ActionResult Create()
        {

            
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            
            //ViewBag.cars = new SelectList(db.Cars, " Model", "Model");

            var userCars = user.Cars
                .ToList()
                .Select(x => new SelectListItem()
                {
                    Text = x.Make +" "+ x.Model + " " + x.PlateNumber,
                    Value = x.Id.ToString()
                })
                  .ToList();

            ViewBag.Id = user.Id;
            ViewBag.Name = user.UserName;
            ViewBag.cars = userCars;

            return View();

        }

        // POST: Reports/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ReportType,GetCar,Mileage,Title,Description,ReportedBy,UserId, Created, State,CarId")] Report report)
        {
            var currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            var userCars = user.Cars
               .ToList()
               .Select(x => new SelectListItem()
               {
                   Text = x.Make + " " + x.Model + " " + x.PlateNumber,
                   Value = x.Id.ToString()
               })
                 .ToList();
            ViewBag.cars = userCars;
            if (ModelState.IsValid)
            {
                db.Reports.Add(report);
                db.SaveChanges();
                                                                    

               return RedirectToAction("SendCreatedReportNotification", new { id = report.Id });

            }

            return View(report);
        }
        [Authorize(Roles = "Admin")]
        // GET: Reports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            ViewBag.cars = new SelectList(db.Cars, " Model", "Model");
            var list = db.Users.OrderBy(r => r.UserName).ToList().Select(s => new SelectListItem { Value = s.UserName.ToString(), Text = s.UserName }).ToList();
            //ViewBag.Role = list;
            var allUsers = db.Users.ToList();
            var adminUsers = allUsers
                .Where(u => u.Roles.Select(r => r.RoleId)
                .Contains("1"))
                .ToList()
                .Select(x => new SelectListItem()
                 {
                     Text = x.UserName,
                     Value = x.UserName
                 })
                 .ToList();
                ViewBag.Role = adminUsers;




            return View(report);
        }

        // POST: Reports/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ReportType,GetCar,Mileage,Title,Description,PersonAssigned,WorkshopCome,InfoFromWorkshop,Resolution,ReportedBy,Created,UserId, State,CarID")] Report report)
        {
            if (ModelState.IsValid)
            {
                db.Entry(report).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(report);
        }
        
        public ActionResult SendCreatedReportNotification(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }


            var callbackUrl = Url.Action("Details", "Reports", new { id = report.Id }, protocol: Request.Url.Scheme);

            SendEmail mail = new SendEmail();
            mail.ToEmail = report.ReportedBy;
            mail.Subject = "Witaj, zgłoszenie nr." + report.Id + " zostało utworzone!";
            mail.Body = "<b> < a href =\"" + callbackUrl + "\">Zgłoszenie nr." + report.Id + "</a "+" zostało utworzone!"+"</b><br>" +
                            "<b>" + "Status zgłoszenia: " + "</b>" + report.State + "<br>" +
                            "<b>" + "Typ zgłoszenia: " + "</b>" + report.ReportType + "<br>" +
                           "<b>" + "Samochód: " + "</b>" + report.Car.Make + " " + report.Car.Model + "<br>" +
                           "<b>" + "Przebieg: " + "</b>" + report.Mileage + "<br>" +
                           "<b>" + "Tytuł: " + "</b>" + report.Title + "<br >" +
                           "<b>" + "Opis: " + "</b>" + report.Description + "<br>";
           
            mail.IsHtml = true;
            mail.Send();

            return RedirectToAction("Index", "Reports");
        }



        [Authorize(Roles ="Admin")]
        public ActionResult SentNotifications(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }


            var callbackUrl = Url.Action("Details", "Reports", new { id=report.Id }, protocol: Request.Url.Scheme);
           
            SendEmail mail = new SendEmail();
            mail.ToEmail = report.ReportedBy;
            mail.Subject = "Witaj, twoje zgłoszenie nr." + report.Id + " zostało zaktualizowane";
            mail.Body =    
                           "<b>" + "<a href =\"" + callbackUrl + "\">Zgłoszenie nr." +report.Id+"</a> zostało zaktualizowane</b>"+ "<br>"+
                           "<b>" + "Status zgłoszenia: " + "</b>" + report.State + "<br>" +
                           "<b>" + "Typ zgłoszenia: " + "</b>" + report.ReportType + "<br>" +
                           "<b>" + "Samochód: " + "</b>" + report.Car.Make + " "+report.Car.Model + "<br>" +
                           "<b>" + "Przebieg: " + "</b>" + report.Mileage + "<br>" +
                           "<b>" + "Tytuł: " + "</b>" + report.Title + "<br >" +
                           "<b>" + "Opis: " + "</b>" + report.Description + "<br>";
                           if (report.PersonAssigned != null)
                            {
                            mail.Body = mail.Body + "<b>" + "Przypisana osoba: " + "</b>" + report.PersonAssigned + "<br>";
                            }
                            if (report.WorkshopCome != null)
                             { 
                                mail.Body = mail.Body + "<b>" + "Data przyjazdu na warsztat: " + "</b>" + report.WorkshopCome + "<br>" ;
                             }
                             if (report.InfoFromWorkshop != null)
                              {
                              mail.Body= mail.Body + "<b>" + "Informacja od warsztatu: " + "</b>" + report.InfoFromWorkshop + "<br>";
                              }
                             if (report.Resolution != null)
                              {
                               mail.Body = mail.Body + "<b>" + "Rozwiązanie: " + "</b>" + report.Resolution + "<br>";
                              }
                            ;
            mail.IsHtml = true;
            mail.Send();



            return RedirectToAction("Index");
        }
       


        // GET: Reports/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        // POST: Reports/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Report report = db.Reports.Find(id);
            db.Reports.Remove(report);
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
