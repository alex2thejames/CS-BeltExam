using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
namespace BeltExam.Models
{
    public class Going
    {
        [Key]
        public int GoingId {get;set;}
        public int UserId {get;set;}
        public int PlanId {get;set;}
        public User User {get;set;}
        public Plan Plan {get;set;}
    }
}