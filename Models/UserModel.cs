using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
namespace BeltExam.Models
{
    public class User
    {
    [Key]
    public int UserId {get;set;}
    
    [MinLength(2)][Display(Name = "Name")]
    public string Name {get;set;}
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email {get;set;}
    
    [DataType(DataType.Password)]
    
    [Required]
    [Display(Name = "Password")]
    // [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]

    public string Password {get;set;}
    
    public List<Going> Goings {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    // Will not be mapped to your users table!
    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm {get;set;}
    }
}