using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {

        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            // HttpContext.Session.Clear();
            if(HttpContext.Session.GetInt32("logID") != null)
            {
                return Redirect("/home");
            }
            return Redirect("/signin");
        }

        [HttpGet("/signin")]
        public IActionResult SignIn()
        {
            // HttpContext.Session.Clear();
            if(HttpContext.Session.GetInt32("logID") != null)
            {
                return Redirect("/home");
            }
            return View("Index");
        }

        public IActionResult RegisterProcess(User newUser)
        {   
            if(ModelState.IsValid)
            {   
                if(dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else 
                {
                    // bool hasUpperCaseLetter      = false ;
                    // bool hasLowerCaseLetter      = false ;
                    // bool hasDecimalDigit         = false ;
                    // // bool hasSymbol               = false;
                    // foreach (char c in newUser.Password )
                    //     {
                    //         if      ( char.IsUpper(c) ) hasUpperCaseLetter = true ;
                    //         else if ( char.IsLower(c) ) hasLowerCaseLetter = true ;
                    //         else if ( char.IsNumber(c) ) hasDecimalDigit    = true ;
                    //         // else if ( char.IsSymbol(c) ) hasSymbol    = true ;
                    //     }
                    // if(hasUpperCaseLetter == true && hasLowerCaseLetter == true && hasDecimalDigit == true)
                    if(Regex.IsMatch(newUser.Password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$"))
                    {
                        PasswordHasher<User> Hasher = new PasswordHasher<User>();
                        newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                        dbContext.Users.Add(newUser);
                        dbContext.SaveChanges();
                        return Redirect("/signin");
                    }
                
                    else
                    {
                        ModelState.AddModelError("Password", "Password Must Contain an Symbol, Number, and One Alphabetical Character");
                        return View("Index");
                    }
                    // else
                    // {
                    //     if(hasDecimalDigit == false)
                    //     {
                    //         ModelState.AddModelError("Password", "Password Must Contain a Number");
                    //     }
                    //     else if(hasLowerCaseLetter == false)
                    //     {
                    //         ModelState.AddModelError("Password", "Password Must Contain a Lowercase Letter");
                    //     }
                    //     else if(hasUpperCaseLetter == false)
                    //     {
                    //         ModelState.AddModelError("Password", "Password Must Contain an Uppercase Letter");
                    //     }
                        // else if(hasSymbol == false)
                        // {
                        //     ModelState.AddModelError("Password", "Password Must Contain an Symbol");
                        // }
                        
                    }
                }
            else
            {
                return View("Index");
            }
        }

        public IActionResult LoginProcess(Login logUser)
        {
            if(ModelState.IsValid)
            {   
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == logUser.Email);
                if(userInDb == null)
                {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
                }
                var hasher = new PasswordHasher<Login>();

                var result = hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.Password);
                System.Console.WriteLine(result);

                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }

                else 
                {
                    HttpContext.Session.SetInt32("logID", userInDb.UserId);
                    return Redirect("/home");
                }
            }
            else
            {   
                return View("Index");
            }
        }

        [HttpGet("/home")]
        public IActionResult Home()
        {
            if(HttpContext.Session.GetInt32("logID") == null)
            {
                return Redirect("/signin");
            }
            var now = DateTime.Now;
            ViewBag.Plans = dbContext.Plans
            .Where(plan => plan.Date > now)
            .Include(plan => plan.Goings)
            .ThenInclude(Goings => Goings.User).ToList();
            int id = HttpContext.Session.GetInt32("logID").GetValueOrDefault();
            ViewBag.Name = dbContext.Users
            .FirstOrDefault(user => user.UserId == id);
            ViewBag.CurrUser = id;
            ViewBag.goingList = dbContext.Goings.ToList();
            return View();
        }
        
        [HttpGet("/new")]
        public IActionResult New()
        {
            if(HttpContext.Session.GetInt32("logID") == null)
            {
                return Redirect("/");
            }
            ViewBag.CurrUser = HttpContext.Session.GetInt32("logID");
            return View();
        }

        public IActionResult AddPlan(Plan newPlan)
        {
            if(ModelState.IsValid)
            {
                if(newPlan.Date > DateTime.Now)
                {
                    int id = HttpContext.Session.GetInt32("logID").GetValueOrDefault();
                    User user2 = dbContext.Users
                    .FirstOrDefault(user => user.UserId == id);
                    newPlan.CreatedByName = user2.Name;
                    dbContext.Plans.Add(newPlan);
                    dbContext.SaveChanges();
                    return Redirect("/home");
                }
                else
                {
                    ModelState.AddModelError("Date", "Date must be in the future!");
                    return View("New");
                }
            }
            else
            {   
                return View("New");
            }
        }

        [HttpGet("/display/{id}")]
        public IActionResult Display(int id)
        {
            if(HttpContext.Session.GetInt32("logID") == null)
            {
                return Redirect("/");
            }
            ViewBag.Plan = dbContext.Plans
            .Include(plan => plan.Goings)
            .ThenInclude(goings => goings.User)
            .FirstOrDefault(p => p.PlanId == id);
            int id2 = HttpContext.Session.GetInt32("logID").GetValueOrDefault();
            ViewBag.Name = dbContext.Users
            .FirstOrDefault(user => user.UserId == id2);
            ViewBag.CurrUser = id2;
            ViewBag.goingList = dbContext.Goings.ToList();
            return View();
        }

        [HttpGet("/delete/{id}")]
        public IActionResult Delete(int id)
        {
            Plan RetrievedPlan = dbContext.Plans.SingleOrDefault(Plan => Plan.PlanId == id);
            dbContext.Plans.Remove(RetrievedPlan);
            dbContext.SaveChanges();
            return Redirect("/home");
        }

        [HttpGet("/join/{id}")]
        public IActionResult Join(int id)
        {
            int? cu = HttpContext.Session.GetInt32("logID");
            int CurrUser = cu ?? default(int);
            Plan RetrievedPlan = dbContext.Plans.SingleOrDefault(Plan => Plan.PlanId == id);
            if(CurrUser == RetrievedPlan.CreatedBy)
            {
                return Redirect("/home");
            }
            else
            {
            Going newGoing = new Going();
            newGoing.UserId = CurrUser;
            newGoing.PlanId = id;
            dbContext.Goings.Add(newGoing);
            dbContext.SaveChanges();
            return Redirect("/home");
            }
        }


        [HttpGet("/leave/{id}")]
        public IActionResult Leave(int id)
        {
            Going RetrievedGoing = dbContext.Goings.SingleOrDefault(Going => Going.GoingId == id);
            dbContext.Goings.Remove(RetrievedGoing);
            dbContext.SaveChanges();
            return Redirect("/home");
        }

        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
