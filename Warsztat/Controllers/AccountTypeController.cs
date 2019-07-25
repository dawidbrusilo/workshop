using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Warsztat.DataContexts;
using Warsztat.Models;

namespace Warsztat.Controllers
{
    [Authorize(Roles="SuperAdmin")]
    public class AccountTypeController : Controller
    {
        IdentityDb identityDb = new IdentityDb();

        public ActionResult Index()
        {
            var role = identityDb.Roles.ToList();
            return View(role);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                identityDb.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["TypKonta"]
                });
                identityDb.SaveChanges();
                ViewBag.Message = "Stworzono nowy typ konta!";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Edit(string Name)
        {
            var Role = identityDb.Roles.Where(r => r.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(Role);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                identityDb.Entry(role).State = System.Data.Entity.EntityState.Modified;
                identityDb.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Delete(string Role)
        {
            var role = identityDb.Roles.Where(r => r.Name.Equals(Role, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            identityDb.Roles.Remove(role);
            identityDb.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Manage()
        {
            var list = identityDb.Roles.OrderBy(r => r.Name).ToList().Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
            ViewBag.Role = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(string Username, string role)
        {
            ApplicationUser user = identityDb.Users.Where(u => u.UserName.Equals(Username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var assign = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new IdentityDb()));
            var result = assign.AddToRole(user.Id, role);

            ViewBag.Message = "Użytkownik został przypisany z sukcesem!";


            var list = identityDb.Roles.OrderBy(r => r.Name).ToList().Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
            ViewBag.Role = list;

            return View("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string Username)
        {
            if (!string.IsNullOrWhiteSpace(Username))
            {
                ApplicationUser user = identityDb.Users.Where(u => u.UserName.Equals(Username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var assign = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new IdentityDb()));

                ViewBag.UserRoles = assign.GetRoles(user.Id);


                var list = identityDb.Roles.OrderBy(r => r.Name).ToList().Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
                ViewBag.Role = list;
            }

            return View("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string Username, string role)
        {
            var assign = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new IdentityDb()));
            ApplicationUser user = identityDb.Users.Where(u => u.UserName.Equals(Username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (assign.IsInRole(user.Id, role))
            {
                assign.RemoveFromRole(user.Id, role);
                ViewBag.Message = "Rola usunięta z sukcesem!";
            }
            else
            {
                ViewBag.Message = "Użytkownik nie posiada wybranej roli.";
            }

            var list = identityDb.Roles.OrderBy(r => r.Name).ToList().Select(s => new SelectListItem { Value = s.Name.ToString(), Text = s.Name }).ToList();
            ViewBag.Role = list;

            return View("Manage");
        }
    }
}
