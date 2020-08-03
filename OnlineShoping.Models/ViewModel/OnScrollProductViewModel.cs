using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
   public class OnScrollProductViewModel
    {

        public int RowNumber { get; set; }

        public int productId { get; set; }

        public string CategoryName { get; set; }

        public string shortDescription { get; set; }

        public ICollection<ProductImage> productImage { get; set; }

        public float price { get; set; }

        public float discount { get; set; }

        public float discountPrice { get; set; }
         
        public int PageCount { get; set; }
    
    }
}
