using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Warsztat.Models
{
    
    public class Report
    {
        [Display(Name = "Nr. zgłoszenia")]
        public int Id { get; set; }
        [Display(Name = "Typ zgłoszenia")]
        public ReportType ReportType { get; set; }
        [Required(ErrorMessage = "Przebieg jest wymagany")]
        [Display(Name = "Przebieg [km]")]
        public int Mileage { get; set; }
        [Required(ErrorMessage = "Proszę wpisać tytuł")]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }
        [Display(Name = "Opis zgłoszenia")]
        public string Description { get; set; }
        [Display(Name = "Data utworzenia")]
        public string Created { get; set; }
        [Display(Name = "Przypisane do:")]
        public string PersonAssigned { get; set; }
        [Display(Name = "Termin przyjazdu na warsztat")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? WorkshopCome { get; set; }
        [Display(Name = "Informacja od warsztatu")]
        public string InfoFromWorkshop { get; set; }
        [Display(Name = "Rozwiązanie")]
        public string Resolution { get; set; }
        [Display(Name = "Zgłaszający")]
        public string ReportedBy { get; set; }
        [Display(Name = "Status zgłoszenia")]
        public State State { get; set; }
        [Display(Name ="Termin przyjazdu do warsztatu")]

        [ForeignKey("ReportId")]
        public ICollection<CustomerMessages> Messages { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "Wybierz samochód!")]
        [ForeignKey("Car")]
        [Display(Name = "Samochód")]
        public int CarID { get; set; }
        public virtual Car Car { get; set; }
        
    }
    public enum ReportType: int
    {
        Inspekcja =0,
        Wymiana =1,
        Naprawa =2
    }
    public enum State: int
    {
        Nowe = 0,
        Obsługiwane = 1,
        Zakończone = 2
    }
}