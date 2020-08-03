using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineShoping.Models.ViewModel
{
    public class AddProductViewModel
    {
        public Product Product { get; set; }

        public List<IFormFile> Photos { get; set; }
        
     
        public int[] SelectedColorid { get; set; }
    }
}
