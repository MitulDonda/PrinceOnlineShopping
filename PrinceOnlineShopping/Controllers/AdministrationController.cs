using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OnlineShoping.Models;
using OnlineShoping.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
   [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IOrderRepository orderRepository,
                                        IProductRepository productRepository
                                        )
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
           this.orderRepository = orderRepository;
            this.productRepository = productRepository;

        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId,string msg)
        {
            ViewBag.roleId = roleId;
            if(msg != null && msg.Equals("rlinuse"))
            { ViewBag.ErrorMessage = $"UserName is already added to this role"; }
            else if(msg != null && msg.Equals("ntblnk"))
            { ViewBag.ErrorMessage = $"UserName is Required"; }
           

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }


            var model = new EditUserRoleViewModel();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {

                    model.Users.Add(userRoleViewModel);
                }
            }

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> EditUsersInRole(EditUserRoleViewModel model, string roleId, string task)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            if (task.Equals("Add"))
            {
                if (model.UserName == null)
                {
                    ModelState.AddModelError(string.Empty, "UserName Required");
                    return RedirectToAction("EditUsersInRole", new { roleId = roleId, msg= "ntblnk" });
                    }
                var user = await userManager.FindByEmailAsync(model.UserName);
                IdentityResult result = null;
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {model.UserName} cannot be found";
                    return View("NotFound");
                }

                if (!(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
                else
                {
                    
                    return RedirectToAction("EditUsersInRole", new { roleId = roleId,msg="rlinuse" });
                }

            }
            else if (task.Equals("Remove"))
            {
                for (int i = 0; i < model.Users.Count; i++)
                {
                    var user = await userManager.FindByIdAsync(model.Users[i].UserId);

                    IdentityResult result = null;

                    if (model.Users[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                    {
                        result = await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else
                    {
                        continue;
                    }

                    if (result.Succeeded)
                    {
                        if (i < (model.Users.Count - 1))
                            continue;
                        else
                            return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }


            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    //throw new Exception("Test Exception");

                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch (DbUpdateException )
                {
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users " +
                        $"in this role. If you want to delete this role, please remove the users from " +
                        $"the role and then try to delete";
                    return View("Error");
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public int OrderCount(DateTime startdate,DateTime enddate)
        {
            int count = 0;
            count = orderRepository.TotalOrderCountBetweenDate(startdate, enddate);
            return count;
        }


        [HttpPost]
        [AllowAnonymous]
        public int CencelOrderCount(DateTime startdate, DateTime enddate)
        {
            int count = 0;
            count = orderRepository.TotalCencelOrderCountBetweenDate(startdate, enddate);
            return count;
        }

        [HttpPost]
        [AllowAnonymous]
        public int TotalProductCount()
        {
            int count = 0;
            count = productRepository.TotalProductCount();
            return count;
        }
    }
}
