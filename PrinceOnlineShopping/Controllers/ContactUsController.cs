using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineShoping.Models;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class ContactUsController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;

        private readonly AppDbContext _context;

        public ContactUsController(UserManager<ApplicationUser> userManager ,AppDbContext context)
        {
            _context = context;
            this.userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddComments(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.ApplicationUser = await userManager.GetUserAsync(HttpContext.User);
                _context.Add(comment);
                _context.SaveChanges();
                ViewData["msg"] = "Thanks. Your Comment is saved and sent to Admin. Admin may contact via Eamil.";
                return View("Index");

            }
                return View("Index");

        }
    }
}
