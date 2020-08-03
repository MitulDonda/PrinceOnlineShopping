using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Services
{
    public interface IProductRepository
    {
        Product GetProductById(int id);
        List<Product> GetTop5product(int catId);

        List<OnScrollProductViewModel> GetAllproductAsync(int PageIndex, int PageSize);

        Product GetProductDetailById(int productId);
        List<OnScrollProductViewModel> GetAllProductByCategory(int pageindex, int pagasize, int catid, string sortby);
        List<OnScrollProductViewModel> GetAllProductByGender(int pageindex, int pagasize, Gender gender, string sortby);
        public float GetProductDiscountPriceByID(int id);

        public int TotalProductCount();

        public List<OnScrollProductViewModel> GetRelatedProdcut(int productid);
    }
}
