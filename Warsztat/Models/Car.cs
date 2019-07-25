using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Warsztat.Models
{
    public class Car
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Marka")]
        public string Make { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Silnik")]
        public string Engine { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Rok produkcji")]
        [Range(1900, 2020)]
        public int DateOfProduction { get; set; }
        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Numer rejstracyjny")]
        public string PlateNumber { get; set; }
        [Display(Name = "Numer VIN")]
        public string Vin { get; set; }

        [Display(Name = "Właściciel")]
        [Required(ErrorMessage = "Proszę wybrać właściciela!")]
        public string ApplicationUserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

    }
}