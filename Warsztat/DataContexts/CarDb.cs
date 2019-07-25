using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Warsztat.Models;

namespace Warsztat.DataContexts
{
    public class CarDb : DbContext
    {
        public CarDb()
            : base("DefaultConnection")
        {

        }

    }
}