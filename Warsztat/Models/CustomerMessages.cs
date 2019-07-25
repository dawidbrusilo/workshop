using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Warsztat.Models
{
    public class CustomerMessages
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public int ReportId { get; set; }
        public DateTime Date {get; set;}
        public virtual Report Report { get; set; }

    }
}