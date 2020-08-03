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
    public class ProductInfoController : Controller
    {
        public readonly IProductRepository productRepository;
        public ProductInfoController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ProductDetails(int Id)
        {
            Product product = productRepository.GetProductById(Id);
            List<OnScrollProductViewModel> retaltedProductList = productRepository.GetRelatedProdcut(Id);
            ProductInfoViewModel productInfoViewModel = new ProductInfoViewModel
            {
                product = product,
                retaltedProductList = retaltedProductList
            };
            return View(productInfoViewModel);
        }
    }
}
