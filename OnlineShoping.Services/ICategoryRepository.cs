using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Services
{
    public interface ICategoryRepository
    {
        public int GetIdByCategoryName(string category);

        public List<Category> GetCategoryList();
    }
}
