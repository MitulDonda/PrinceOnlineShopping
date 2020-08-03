using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoping.Models.DatabaseModel
{
    public class Color
    {

        public Color()
        {
            this.ProductColor = new HashSet<ProductColor>();
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public  ICollection<ProductColor> ProductColor { get; set; }

       
    }
}
