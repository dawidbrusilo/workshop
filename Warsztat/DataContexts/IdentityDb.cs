using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Warsztat.Models;
using TrackerEnabledDbContext.Identity;
namespace Warsztat.DataContexts
{
    public class IdentityDb :  TrackerIdentityContext<ApplicationUser>
    {
        public IdentityDb()
            : base("DefaultConnection")
        {
        }

        public static IdentityDb Create()
        {
            return new IdentityDb();
        }

        public System.Data.Entity.DbSet<Warsztat.Models.Car> Cars { get; set; }

        public System.Data.Entity.DbSet<Warsztat.Models.Report> Reports { get; set; }

        public System.Data.Entity.DbSet<Warsztat.Models.ClientViewModel> ClientViewModels { get; set; }

        public System.Data.Entity.DbSet<Warsztat.Models.CustomerMessages> CustomerMessages { get; set; }
        //public System.Data.Entity.DbSet<Warsztat.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Warsztat.Models.ApplicationUser> ApplicationUsers { get; set; }



        //public System.Data.Entity.DbSet<Warsztat.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}