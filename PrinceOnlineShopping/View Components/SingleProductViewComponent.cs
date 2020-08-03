using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrinceOnlineShopping.View_Components
{
    public class SingleProductViewComponent :ViewComponent
    {

        private readonly IProductRepository productRepository;
        public SingleProductViewComponent(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public IViewComponentResult Invoke(int id)
        {
            var result = productRepository.GetProductById(id);
            return View(result);
        }

    }
}
