using Microsoft.EntityFrameworkCore;
using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShoping.Services
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly AppDbContext context;
        public CategoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public List<Category> GetCategoryList()
        {
            return context.Category.ToList();
;        }

        public int GetIdByCategoryName(string category)
        {
            return context.Category.Where(c => c.CategoryName == category).Select(c => c.CategoryId).FirstOrDefault();
        }


    }
}
