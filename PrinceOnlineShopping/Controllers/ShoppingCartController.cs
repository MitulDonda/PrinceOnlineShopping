using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository productRepository;

        public ShoppingCartController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<OnScrollProductViewModel> productList = new List<OnScrollProductViewModel>();
            ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel();

            if (HttpContext.Session.GetComplexData<List<int>>("cart") != null)
            {
                List<int> productsId = HttpContext.Session.GetComplexData<List<int>>("cart");
                float totalPrice = 0;
                for (int i = 0; i < productsId.Count; i++)
                {
                    Product productdata = productRepository.GetProductDetailById(productsId[i]);
                    OnScrollProductViewModel dropdownProduct = new OnScrollProductViewModel
                    {
                        price = productdata.Price,
                        productId = productdata.ProductId,
                        productImage = productdata.ProductImage,
                        discount =productdata.Discount,
                        discountPrice = productdata.Price - productdata.Price * productdata.Discount / 100,
                        shortDescription = productdata.ShortDescription
                    };
                    totalPrice += productdata.Price - productdata.Price * productdata.Discount / 100;
                    productList.Add(dropdownProduct);

                }
                shoppingCartViewModel = new ShoppingCartViewModel
                {
                    ProductList = productList,
                    totalPrice =totalPrice

                };
            }
            return View(shoppingCartViewModel);
        }

    }
}
