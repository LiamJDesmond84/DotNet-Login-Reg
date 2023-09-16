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


            return View("Index");
        }
        else
        {
            Debug.WriteLine("NOT NULL");
            User loginUser = HttpContext.Session.GetObjectFromJson<User>("SessionUser");
            return View("Index", loginUser);
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






    [HttpPost]
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

                Debug.WriteLine("WITH USER MODEL");

                Debug.WriteLine(HttpContext.Session.GetObjectFromJson<User>("SessionUser"));

                Debug.WriteLine("NOOOOOOOOO USER MODEL");



                return RedirectToAction("Index", user);
            }
            // other code
        }
        else
        {

            return View("Index");
        }

    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            // If inital ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            // If no user exists with provided email
            if (userInDb == null)
            {
                Debug.WriteLine("WITH USER MODEL");
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
                Debug.WriteLine("INVALID EMAIL/PASSWORD");
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Password", "Invalid Email/Password");
                return View("SomeView");
            }
            else
            {
                User user = userInDb;
                HttpContext.Session.SetObjectAsJson("SessionUser", user);
                return View("Index");

            }
        }
        else
        {
            Debug.WriteLine("MODEL NOT VALID");
            return View();
        }
    }
}
