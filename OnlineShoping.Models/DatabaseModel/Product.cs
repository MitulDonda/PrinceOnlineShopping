using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoping.Models.DatabaseModel
{
    public class Product
    {
        public Product()
        {
            this.ProductImage = new HashSet<ProductImage>();
            this.ProductColor = new HashSet<ProductColor>();
            this.ProductReviews = new HashSet<ProductReview>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
         
        public float Price { get; set; }

        public ProductSize? ProductSize { get; set; }
        [Required]
        public Gender? Gender { get; set; }

        public int Stock { get; set; }

        public float Discount { get; set; }
        public int? CategoryId { get; set; }

        public DateTime AddedDateTime { get; set; }
        
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }

        public  ICollection<ProductColor> ProductColor { get; set; }



    }
}
