using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Services;

namespace OnlineShoppingWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Product : Controller
    {

        private readonly ProductRepository productRepository;
        public Product(ProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Get(int pageIndex)
        {
            var products = productRepository.GetAllproductAsync(1, 12);
            var JonResult = Json(products);
            return Json(products);
        }
    }
}
