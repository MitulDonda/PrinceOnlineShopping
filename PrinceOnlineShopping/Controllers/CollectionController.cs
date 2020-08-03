using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class CollectionController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;

        public CollectionController(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
        }
      
        [AllowAnonymous]
        public IActionResult Index(string type)
        {
            ViewBag.type = type;

            CollectionIndexViewModel CollectionIndexViewModel = new CollectionIndexViewModel
            {
               
            };
             return View(CollectionIndexViewModel);
        }

        [HttpPost]
        [AllowAnonymous]

        public JsonResult GetAllProductByGender(int pageindex, int pagasize, Gender gender,string sortby)
        {
            

            var products = productRepository.GetAllProductByGender(pageindex, pagasize, gender,sortby);
            var JonResult = Json(products);
            return Json(products);
        }

    }
}

