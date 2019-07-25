using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Warsztat.DataContexts;
using Warsztat.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Warsztat.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ClientsController : Controller
    {
        private IdentityDb db = new IdentityDb();
        // GET: Clients
        public ActionResult Index(string search, int? page, string currentFilter)
        {
            var client = from d in db.Users
                         select d;

            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }
            ViewBag.CurrentFilter = search;


            if (!String.IsNullOrEmpty(search))
            {
                client = client.Where(s => s.LastName.Contains(search)
                                        || s.Name.Contains(search));
            }

            var clients = client.ToList()
                      .Select(u => new ClientViewModel

                      {
                          Id = u.Id,
                          Name = u.Name,
                          LastName = u.LastName,
                          Street = u.Street,
                          City = u.City,
                          Phone = u.Phone,
                          Email = u.Email
                      }).ToList();

          

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(clients.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Details(string id)
        {
           
            var detail = from d in db.Users
                         where d.Id == id
                         select new
                         {
                             Id = d.Id,
                             Name = d.Name,
                             LastName = d.LastName,
                             Street = d.Street,
                             City = d.City,
                             Phone = d.Phone,
                             Email = d.Email
                         };
            var details = detail.ToList()
                          .Select(d => new ClientViewModel

                          {
                              Id = d.Id,
                              Name = d.Name,
                              LastName = d.LastName,
                              Street = d.Street,
                              City = d.City,
                              Phone = d.Phone,
                              Email = d.Email
                          }).ToList();

            

            if (id == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }


        public ActionResult ClientCars(string id)
        {

            var cars = from c in db.Cars
                       where c.ApplicationUserID == id
                       select c;

            if (id == null)
            {
                return HttpNotFound();
            }
          
            return View(cars.ToList());
        }

        public ActionResult ClientReports(string id ,int? search, int? page, int? currentFilter)
        {
            var reports = from r in db.Reports
                          where r.UserId == id
                          select r;

            if (search != null)
            {
                reports = reports.Where(s => s.Id==(search));
            }
           

            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }
            ViewBag.CurrentFilter = search;

          

            if (id == null)
            {
                return HttpNotFound();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(reports.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);


            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,LastName,City, Street, Phone,Email,UserName, SecurityStamp, PasswordHash,EmailConfirmed")]ApplicationUser user )
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult PasswordReset(string id)
        {

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var user = userManager.FindById(id);

            string code = userManager.GeneratePasswordResetToken(user.Id);




            return RedirectToAction("ResetPassword", "Account", new { userId = user.Id, code = code });
        }
    }
}