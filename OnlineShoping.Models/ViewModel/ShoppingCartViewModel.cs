using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
     public class ShoppingCartViewModel
    {

        public ShoppingCartViewModel()
        {
             ProductList = new List<OnScrollProductViewModel>();
        }

        public List<OnScrollProductViewModel>  ProductList { get; set; }
        public float totalPrice { get; set; }
    }
}
