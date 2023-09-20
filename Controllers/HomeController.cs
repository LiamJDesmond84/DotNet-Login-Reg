using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotNet_Login_Reg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace DotNet_Login_Reg.Controllers;

public class HomeController : Controller
{

    private AppDbContext _context;

    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {

        Debug.WriteLine(HttpContext.Session.GetObjectFromJson<User>("SessionUser"));

        if (HttpContext.Session.GetObjectFromJson<User>("SessionUser") == null)
        {
            Debug.WriteLine("NULL");
            List<User> users = _context.Users.ToList();

            ViewData["Users"] = users;

            return View("Index");
        }
        else
        {
            Debug.WriteLine("NOT NULL");
            User user = HttpContext.Session.GetObjectFromJson<User>("SessionUser");
            return View("Index", user);
        }
        
    }


    [HttpGet]
    public IActionResult RegisterPage ()
    {
        return View ("Register");
    }

    [HttpGet]
    public IActionResult LoginPage()
    {
        return View("Login");
    }


    //____/\\\\\\\\\______/\\\\\\\\\\\\\\\_____/\\\\\\\\\\\\__/\\\\\\\\\\\_____/\\\\\\\\\\\____/\\\\\\\\\\\\\\\__/\\\\\\\\\\\\\\\____/\\\\\\\\\_____
    // __/\\\///////\\\___\/\\\///////////____/\\\//////////__\/////\\\///____/\\\/////////\\\_\///////\\\/////__\/\\\///////////___/\\\///////\\\___       
    //  _\/\\\_____\/\\\___\/\\\______________/\\\_________________\/\\\______\//\\\______\///________\/\\\_______\/\\\_____________\/\\\_____\/\\\___      
    //   _\/\\\\\\\\\\\/____\/\\\\\\\\\\\_____\/\\\____/\\\\\\\_____\/\\\_______\////\\\_______________\/\\\_______\/\\\\\\\\\\\_____\/\\\\\\\\\\\/____     
    //    _\/\\\//////\\\____\/\\\///////______\/\\\___\/////\\\_____\/\\\__________\////\\\____________\/\\\_______\/\\\///////______\/\\\//////\\\____    
    //     _\/\\\____\//\\\___\/\\\_____________\/\\\_______\/\\\_____\/\\\_____________\////\\\_________\/\\\_______\/\\\_____________\/\\\____\//\\\___   
    //      _\/\\\_____\//\\\__\/\\\_____________\/\\\_______\/\\\_____\/\\\______/\\\______\//\\\________\/\\\_______\/\\\_____________\/\\\_____\//\\\__  
    //       _\/\\\______\//\\\_\/\\\\\\\\\\\\\\\_\//\\\\\\\\\\\\/___/\\\\\\\\\\\_\///\\\\\\\\\\\/_________\/\\\_______\/\\\\\\\\\\\\\\\_\/\\\______\//\\\_ 
    //        _\///________\///__\///////////////___\////////////____\///////////____\///////////___________\///________\///////////////__\///________\///__




    public IActionResult Register(User user)
    {
        Debug.WriteLine("Register Method");
        Debug.WriteLine(user.FirstName);
        // Check initial ModelState
        if (ModelState.IsValid)
        {
            Debug.WriteLine("ITS VALID");
            // If a User exists with provided email
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Email", "Email already in use!");

                // You may consider returning to the View at this point

                return View("Register");
            }

            else
            {
                // Initializing a PasswordHasher object, providing our User class as its type
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                //Save your user object to the database

                _context.Add(user);
                _context.SaveChanges();

                HttpContext.Session.SetObjectAsJson("SessionUser", user);


                Debug.WriteLine(HttpContext.Session.GetObjectFromJson<User>("SessionUser"));

                return RedirectToAction("Index", user);
            }
            // other code
        }
        else
        {

            return View("Index");
        }

    }


    //__/\\\___________________/\\\\\__________/\\\\\\\\\\\\__/\\\\\\\\\\\__/\\\\\_____/\\\_
    //  _\/\\\_________________/\\\///\\\______/\\\//////////__\/////\\\///__\/\\\\\\___\/\\\_       
    //   _\/\\\_______________/\\\/__\///\\\___/\\\_________________\/\\\_____\/\\\/\\\__\/\\\_      
    //    _\/\\\______________/\\\______\//\\\_\/\\\____/\\\\\\\_____\/\\\_____\/\\\//\\\_\/\\\_     
    //     _\/\\\_____________\/\\\_______\/\\\_\/\\\___\/////\\\_____\/\\\_____\/\\\\//\\\\/\\\_    
    //      _\/\\\_____________\//\\\______/\\\__\/\\\_______\/\\\_____\/\\\_____\/\\\_\//\\\/\\\_   
    //       _\/\\\______________\///\\\__/\\\____\/\\\_______\/\\\_____\/\\\_____\/\\\__\//\\\\\\_  
    //        _\/\\\\\\\\\\\\\\\____\///\\\\\/_____\//\\\\\\\\\\\\/___/\\\\\\\\\\\_\/\\\___\//\\\\\_ 
    //         _\///////////////_______\/////________\////////////____\///////////__\///_____\/////__

    public IActionResult Login(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            // If inital ModelState is valid, query for a user with provided email
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            // If no user exists with provided email
            if (userInDb == null)
            {

                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("SomeView");
            }

            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();

            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

            // result can be compared to 0 for failure - Password?
            if (result == 0)
            {
                // handle failure (this should be similar to how "existing email" is handled)

                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Password", "Invalid Email/Password");
                return View("SomeView");
            }
            else
            {
                User user = userInDb;
                HttpContext.Session.SetObjectAsJson("SessionUser", user);

                User loggedInUser = HttpContext.Session.GetObjectFromJson<User>("SessionUser");

                Debug.WriteLine("USER LOGGED IN - SESSION SET");

                return View("Index", loggedInUser);

            }
        }
        else
        {
            Debug.WriteLine("MODEL NOT VALID");
            return View();
        }
    }

    //__/\\\___________________/\\\\\__________/\\\\\\\\\\\\_______/\\\\\_______/\\\________/\\\__/\\\\\\\\\\\\\\\_
    // _\/\\\_________________/\\\///\\\______/\\\//////////______/\\\///\\\____\/\\\_______\/\\\_\///////\\\/////__       
    //  _\/\\\_______________/\\\/__\///\\\___/\\\_______________/\\\/__\///\\\__\/\\\_______\/\\\_______\/\\\_______      
    //   _\/\\\______________/\\\______\//\\\_\/\\\____/\\\\\\\__/\\\______\//\\\_\/\\\_______\/\\\_______\/\\\_______     
    //    _\/\\\_____________\/\\\_______\/\\\_\/\\\___\/////\\\_\/\\\_______\/\\\_\/\\\_______\/\\\_______\/\\\_______    
    //     _\/\\\_____________\//\\\______/\\\__\/\\\_______\/\\\_\//\\\______/\\\__\/\\\_______\/\\\_______\/\\\_______   
    //      _\/\\\______________\///\\\__/\\\____\/\\\_______\/\\\__\///\\\__/\\\____\//\\\______/\\\________\/\\\_______  
    //       _\/\\\\\\\\\\\\\\\____\///\\\\\/_____\//\\\\\\\\\\\\/_____\///\\\\\/______\///\\\\\\\\\/_________\/\\\_______ 
    //        _\///////////////_______\/////________\////////////_________\/////__________\/////////___________\///________

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return View("Index");

    }
}
