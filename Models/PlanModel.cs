using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
namespace BeltExam.Models
{
    public class Plan
    {
        [Key]
        public int PlanId {get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Title must be 2 characters or longer!")]
        [Display(Name = "Title")]
        public string Title {get;set;}

        [Required]
        public DateTime Date {get;set;}

        [Required]
        public DateTime Time {get;set;}

        [Required]
        [Range(0,999999, ErrorMessage="Duration must be above zero")]
        [Display(Name = "Duration")]
        public int Duration {get;set;}

        [Required]
        public string DType {get;set;}

        [Required]
        public string Description {get;set;}

        public int CreatedBy {get;set;}

        public string CreatedByName {get;set;}

        public List<Going> Goings {get;set;}
    }
}