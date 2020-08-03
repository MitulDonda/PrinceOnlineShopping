using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class ShopController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;

        public ShopController(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            this.categoryRepository = categoryRepository;
            this.productRepository =productRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ShopIndexViewModel shopIndex = new ShopIndexViewModel
            {
                CategoryList = categoryRepository.GetCategoryList()
            };

            return View(shopIndex);
        }


        [HttpPost]
        [AllowAnonymous]

        public JsonResult GetAllProductByCategory(int pageindex, int pagasize, string catName,string sortby)
        {
            int catid = categoryRepository.GetIdByCategoryName(catName);

            var products = productRepository.GetAllProductByCategory(pageindex, pagasize,catid,sortby);
            var JonResult = Json(products);
            return Json(products);
        }
        [HttpPost]
        [AllowAnonymous]

        public JsonResult GetCatId(string categoryName)
        {

            var catid =categoryRepository.GetIdByCategoryName(categoryName);
            var JonResult = Json(catid);
            return Json(catid);
        }
    }
}
