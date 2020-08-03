using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrinceOnlineShopping.View_Components
{
    public class PageCountViewComponent : ViewComponent
    {
        private readonly IProductRepository productRepository;

        public PageCountViewComponent(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IViewComponentResult Invoke(string device)
        {
            List<OnScrollProductViewModel> productList = new List<OnScrollProductViewModel>();

           var count = (int)(HttpContext.Session.GetInt32("count") ==null ? 0: HttpContext.Session.GetInt32("count"));
            ViewBag.count = count;
            float totalPrice = 0;

            if(count > 0)
            {
                if (HttpContext.Session.GetComplexData<List<int>>("cart") != null)
                {
                    List<int> productsId = HttpContext.Session.GetComplexData<List<int>>("cart");
                    for (int i = 0; i < productsId.Count; i++)
                    {
                        Product product = productRepository.GetProductDetailById(productsId[i]);
                        OnScrollProductViewModel dropdownProduct = new OnScrollProductViewModel
                        {
                            price = product.Price,
                            productId = product.ProductId,
                            productImage = product.ProductImage,
                            discountPrice = product.Price - product.Price * product.Discount / 100,
                            shortDescription = product.ShortDescription
                        };
                        productList.Add(dropdownProduct);
                        if (i >= 2)
                            break;
                    }
                    for (int i = 0; i < productsId.Count; i++)
                    {
                        totalPrice += productRepository.GetProductDiscountPriceByID(productsId[i]);
                    }
                }
            }
            ViewBag.totalPrice = totalPrice;
            ViewBag.device = device;
            return View(productList);
        }
    }
}
