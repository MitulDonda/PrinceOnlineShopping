using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Services;
using OnlineShoping.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace PrinceOnlineShopping.Controllers
{
    public class AddToCartController : Controller
    {
        private readonly IProductRepository productRepository;

        public AddToCartController( IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IActionResult Index()
        {

            List<Product> user = new List<Product>();
            user.Add(
                new Product
                {
                    Price=150,
                    ProductName="final"
                }
                );

            HttpContext.Session.SetComplexData("UserData", user);
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Add(int product)
        {

            if (HttpContext.Session.GetComplexData<List<int>>("cart") == null)
            {
                List<int> products = new List<int>();

                products.Add(product);
                HttpContext.Session.SetComplexData("cart", products);
                ViewBag.cart = products.Count();
                HttpContext.Session.SetInt32("count", 1);

            }
            else
            {
                List<int> products = HttpContext.Session.GetComplexData<List<int>>("cart");
                products.Add(product);
                HttpContext.Session.SetComplexData("cart", products);
                ViewBag.cart = products.Count();
                int count = (int)(HttpContext.Session.GetInt32("count") + 1);
                HttpContext.Session.SetInt32("count", count);
            }
            List<OnScrollProductViewModel> productList = new List<OnScrollProductViewModel>();


            if (HttpContext.Session.GetComplexData<List<int>>("cart") != null)
            {
               List<int> productsId = HttpContext.Session.GetComplexData<List<int>>("cart");
                for (int i = 0; i < productsId.Count; i++)
                {
                    Product productdata = productRepository.GetProductDetailById(productsId[i]);
                    OnScrollProductViewModel dropdownProduct = new OnScrollProductViewModel
                    {
                        price = productdata.Price,
                        productId = productdata.ProductId,
                        productImage = productdata.ProductImage,
                        discount = productdata.Discount,
                       
                        discountPrice = productdata.Price - productdata.Price * productdata.Discount / 100,
                        shortDescription = productdata.ShortDescription
                    };
                    productList.Add(dropdownProduct);
                   
                }
               
            }
            return Json(productList);

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Remove(int product)
        {
            if (HttpContext.Session.GetComplexData<List<int>>("cart") != null)
            {
                List<int> products = HttpContext.Session.GetComplexData<List<int>>("cart");
                products.Remove(product);
                HttpContext.Session.SetComplexData("cart", products);
                int count = (int)(HttpContext.Session.GetInt32("count") -1);
                HttpContext.Session.SetInt32("count", count);
            }


            return RedirectToAction("Index", "Home");
        }
    }
}
