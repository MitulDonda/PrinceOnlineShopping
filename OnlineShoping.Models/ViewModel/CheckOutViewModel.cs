using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
    public class CheckOutViewModel
    {
        public CheckOutViewModel()
        {

            ProductList = new List<OnScrollProductViewModel>();
            order = new Order();
        }
        public List<OnScrollProductViewModel> ProductList { get; set; }
        public float totalPrice { get; set; }
        public Order order { get; set; }
    }
}
