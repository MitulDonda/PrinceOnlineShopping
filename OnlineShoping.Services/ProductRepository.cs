using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShoping.Services
{
    public class ProductRepository : IProductRepository
    {
        private AppDbContext context;
        private readonly ICategoryRepository categoryRepository;

        public ProductRepository(AppDbContext context,ICategoryRepository categoryRepository)
        {
            this.context = context;
            this.categoryRepository = categoryRepository;
        }


        public List<OnScrollProductViewModel> GetAllproductAsync(int PageIndex, int PageSize)
        {
            try
            {
                // var result1 = context.Database.ExecuteSqlCommand("select * from Products");

                var products = context.Products.Include(c => c.Category).Include(i => i.ProductImage).OrderByDescending(p => p.AddedDateTime).ToList();


                int PageCount = 1 + ((products.Count - 1) / PageSize);
                var TempProduct = products
                        .Select((t, i) => new OnScrollProductViewModel
                        {
                            RowNumber = i + 1,
                            productId = t.ProductId,
                            CategoryName = t.Category.CategoryName,
                            shortDescription = t.ShortDescription,
                            productImage = t.ProductImage,
                            price = t.Price,
                            PageCount = PageCount,
                            discount = t.Discount,
                            discountPrice = (float)Math.Round(t.Price - (t.Price * t.Discount) / 100,2)
                        }).ToList();


                int MinRownumber = (PageIndex - 1) * PageSize + 1;
                int MaxRownumber = (((PageIndex - 1) * PageSize + 1) + PageSize) - 1;

                var FinalData = from T in TempProduct
                                where T.RowNumber >= MinRownumber && T.RowNumber <= MaxRownumber
                                select T;

                var Fi = FinalData.ToList(); ;
                return FinalData.ToList();

                //var customers = await context.Database.SqlQuery<Product>("GetProductForScroll").ToListAsync();

            }
            catch (Exception )
            {
                throw;
            }
        }

        public Product GetProductById(int id)
        {
            try
            {
                Product product = (Product)context.Products.
                     Include(c => c.Category).
                     Include(p => p.ProductImage).
                     Include(c => c.ProductColor).
                     ThenInclude(p => p.Color).FirstOrDefault(m => m.ProductId == id);
                return product;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public List<Product> GetTop5product(int catId)
        {
            try
            {
                return context.Products.Where(c => c.CategoryId == catId)
                    .Include(c => c.Category)
                    .Include(pi => pi.ProductImage)
                    .Include(pc => pc.ProductColor).ThenInclude(pcc => pcc.Color)
                    .OrderByDescending(p => p.AddedDateTime).Take(5).ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        //[Obsolete]
        //public static void ExecProcedure(string name, Dictionary<string, string> param = null)
        //{

        //    List<SqlParameter> sqlParametesList = new List<SqlParameter>();

        //    try
        //    {
        //        if (param != null)
        //        {
        //            name = NormalizeProcedureName(name, param);
        //            foreach (var item in param)
        //            {
        //                sqlParametesList.Add(new SqlParameter("@" + item.Key + "", item.Value));
        //            }
        //        }
        //       var result= context.Database.ExecuteSqlCommand(name, sqlParametesList.ToArray());
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(string.Format("Error in ExecProcedure: {0}", e.Message));
        //    }
        //}

        //public IEnumerable<MeterBillInput> SPGetMeterBillEntryData(DateTime entryDate)
        //{
        //    var meterBill = _unitOfWork.SP_GetMeterBillEntryDataRepository.Set().FromSql(
        //                     Connection.GetQueryString("dbo.SP_GetMeterBillEntryData", new Dictionary<string, string>
        //                     {
        //                        { "@entryDate",entryDate.ToString() }
        //                     })).ToList();
        //    return Mapper.Map<IEnumerable<MeterBillInput>>(meterBill);
        //}
        private static string NormalizeProcedureName(string name, Dictionary<string, string> param)
        {
            if (!name.Contains("@"))
            {
                StringBuilder sb = new StringBuilder();
                string delimiter = "";
                sb.Append(name);

                foreach (var item in param)
                {
                    sb.Append(delimiter);
                    sb.Append(" @");
                    sb.Append(item.Key);
                    delimiter = ",";
                }

                name = sb.ToString();
            }
            return name;
        }

        public Product GetProductDetailById(int productId)
        {
            return (Product)context.Products
                    .Include(c => c.Category)
                    .Include(pi => pi.ProductImage)
                    .Include(pc => pc.ProductColor).ThenInclude(c => c.Color)
                    .Where(p => p.ProductId == productId).FirstOrDefault();

        }

        public List<OnScrollProductViewModel> GetAllProductByCategory(int PageIndex, int PageSize, int catid,string sortby)
        {
            var products = new List<Product>();
           

            if (sortby.Equals("newarriaval"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.CategoryId == catid)
                  .Include(i => i.ProductImage).
                  OrderByDescending(p => p.AddedDateTime).ToList();

            }
            else if (sortby.Equals("priceltoh"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.CategoryId == catid)
                 .Include(i => i.ProductImage).
                 OrderBy(p => p.Price).ToList();

            }
            else if (sortby.Equals("pricehtol"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.CategoryId == catid)
                 .Include(i => i.ProductImage).
                 OrderByDescending(p => p.Price).ToList();

            }
            else if (sortby.Equals("discount"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.CategoryId == catid)
                  .Include(i => i.ProductImage).
                  OrderByDescending(p => p.Discount).ToList();

            }

            int PageCount = 1 + ((products.Count - 1) / PageSize);
            var TempProduct = products
                    .Select((t, i) => new OnScrollProductViewModel
                    {
                        RowNumber = i + 1,
                        productId = t.ProductId,
                        CategoryName = t.Category.CategoryName,
                        shortDescription = t.ShortDescription,
                        productImage = t.ProductImage,
                        price = t.Price,
                        PageCount = PageCount,
                        discount = t.Discount,
                        discountPrice = (float)Math.Round(t.Price - (t.Price * t.Discount) / 100, 2)
                    }).ToList();


            int MinRownumber = (PageIndex - 1) * PageSize + 1;
            int MaxRownumber = (((PageIndex - 1) * PageSize + 1) + PageSize) - 1;

            var FinalData = from T in TempProduct
                            where T.RowNumber >= MinRownumber && T.RowNumber <= MaxRownumber
                            select T;

            var Fi = FinalData.ToList(); ;
            return FinalData.ToList();
        }

        public List<OnScrollProductViewModel> GetAllProductByGender(int PageIndex, int PageSize, Gender gender, string sortby)
        {
            var products = new List<Product>();
            if (sortby .Equals("newarriaval"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.Gender == gender)
                   .Include(i => i.ProductImage).
                   OrderByDescending(p => p.AddedDateTime).ToList();
            }
            else if (sortby.Equals("priceltoh"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.Gender == gender)
                   .Include(i => i.ProductImage).
                   OrderBy(p => p.Price).ToList();
            }
            else if(sortby.Equals("pricehtol"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.Gender == gender)
                   .Include(i => i.ProductImage).
                   OrderByDescending(p => p.Price).ToList();
            }
            else if (sortby.Equals("discount"))
            {
                 products = context.Products.Include(c => c.Category).Where(c => c.Gender == gender)
                   .Include(i => i.ProductImage).
                   OrderByDescending(p => p.Discount).ToList();
            }


            int PageCount = 1 + ((products.Count - 1) / PageSize);
            var TempProduct = products
                    .Select((t, i) => new OnScrollProductViewModel
                    {
                        RowNumber = i + 1,
                        productId = t.ProductId,
                        CategoryName = t.Category.CategoryName,
                        shortDescription = t.ShortDescription,
                        productImage = t.ProductImage,
                        price = t.Price,
                        PageCount = PageCount,
                        discount = t.Discount,
                        discountPrice = (float)Math.Round(t.Price - (t.Price * t.Discount) / 100, 2)
                    }).ToList();


            int MinRownumber = (PageIndex - 1) * PageSize + 1;
            int MaxRownumber = (((PageIndex - 1) * PageSize + 1) + PageSize) - 1;

            var FinalData = from T in TempProduct
                            where T.RowNumber >= MinRownumber && T.RowNumber <= MaxRownumber
                            select T;

            var Fi = FinalData.ToList(); ;
            return FinalData.ToList();
        }

        public float GetProductDiscountPriceByID(int id)
        {
           float price = context.Products.Where(p => p.ProductId == id).Select(p => p.Price).FirstOrDefault();
           float discount = context.Products.Where(p => p.ProductId == id).Select(p => p.Discount).FirstOrDefault();
            return price - price * discount / 100;

        }

        public int TotalProductCount()
        {
            return context.Products.Count();
        }

        public List<OnScrollProductViewModel> GetRelatedProdcut(int productid)
        {
            Product product = GetProductDetailById(productid);
            List<Product>  products = context.Products.
                Include(c => c.Category)
                   .Include(i => i.ProductImage)
                   .Where(p => p.CategoryId == product.CategoryId)
                   .Where(p=>p.ProductId < productid)
                   .OrderBy(p => p.ProductId)
                   .Take(4)
                   .ToList();

            var TempProduct = products
                   .Select((t, i) => new OnScrollProductViewModel
                   {
                       
                       productId = t.ProductId,
                       CategoryName = t.Category.CategoryName,
                       shortDescription = t.ShortDescription,
                       productImage = t.ProductImage,
                       price = t.Price,
                       
                       discount = t.Discount,
                       discountPrice = (float)Math.Round(t.Price - (t.Price * t.Discount) / 100, 2)
                   }).ToList();
            return TempProduct;
        }
    }
}
