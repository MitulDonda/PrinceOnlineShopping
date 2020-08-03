using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {
            Top5Sarees = new List<Product>();
            Top5Shirt = new List<Product>();
            First12Product = new List<OnScrollProductViewModel>();
        }
       public List<Product> Top5Sarees { get; set; }
       public List<Product> Top5Shirt { get; set; }
       public List<OnScrollProductViewModel> First12Product { get; set; }

    }
}
