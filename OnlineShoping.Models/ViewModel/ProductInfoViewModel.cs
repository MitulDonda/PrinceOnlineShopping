using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
    public class ProductInfoViewModel
    {
        public ProductInfoViewModel()
        {

            retaltedProductList = new List<OnScrollProductViewModel>();
            product =new Product();
         }
        public Product product { get; set; }
        public List<OnScrollProductViewModel> retaltedProductList { get; set; }
    }
}
