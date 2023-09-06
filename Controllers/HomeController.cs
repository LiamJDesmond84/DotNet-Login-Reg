using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotNet_Login_Reg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        return View();
    }



    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        // Check initial ModelState
        if (ModelState.IsValid)
        {
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

                return RedirectToAction("Index");
            }
            // other code
        }
        else
        {
            return View(("Register");
        }

    }
    }
