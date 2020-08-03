using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;
using PrinceOnlineShopping.Models;

namespace PrinceOnlineShopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }
        [AllowAnonymous]
        public IActionResult Index(string msg)
        {
            try
            {
                int SareeId = categoryRepository.GetIdByCategoryName("Sarees");
                int ShirtId = categoryRepository.GetIdByCategoryName("Shirt");
                List<Product> Top5shirtProduct = productRepository.GetTop5product(ShirtId);
                List<Product> Top5SareetProduct = productRepository.GetTop5product(SareeId);
                List<OnScrollProductViewModel> First12Product = productRepository.GetAllproductAsync(1, 12);

                HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel()
                {
                    Top5Sarees = Top5SareetProduct,
                    Top5Shirt = Top5shirtProduct,
                    First12Product = First12Product
                };
                if (msg != null)
                {
                    ViewBag.info = "Your Password is added successfully. Happy Shopping.";
                }
                return View(homeIndexViewModel);
            }
            catch (Exception )
            {

                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[HttpPost]
        //[AllowAnonymous]

        //public JsonResult GetAllProduct(string json)
        //{
        //    dynamic data = JObject.Parse(json);
        //    int id = data.Id;
        //    var products = productRepository.GetAllproductAsync(id, 8);
        //    var JonResult = Json(products);
        //    return Json(products);
        //}

        [HttpPost]
        [AllowAnonymous]

        public JsonResult GetAllProduct(int pageindex,int pagasize)
        {
           
            var products = productRepository.GetAllproductAsync(pageindex, pagasize);
            var JonResult = Json(products);
            return Json(products);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetMyViewComponent()
        {
            
            return ViewComponent("PageCount");
        }

    }
}
