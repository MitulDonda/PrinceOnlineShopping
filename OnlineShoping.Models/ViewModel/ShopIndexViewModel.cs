using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
    public class ShopIndexViewModel
    {
        public ShopIndexViewModel()
        {
            CategoryList = new List<Category>();
        }
        public List<Category> CategoryList { get; set; }
    }
}
