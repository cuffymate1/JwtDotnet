using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebStoreApi.Models
{
    [Table("Products", Schema = "dbo")]
    [Comment("ตารางเก็บข้อมูลสินค้า")]
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please enter the Product Name")]
        [Column(TypeName = "nvarchar(64)")]
        public string ProductName { get; set; }
        
        [Required(ErrorMessage = "Please enter the Unit Price")]
        [Column(TypeName = "decimal(10,2)")]
        public int UnitPrice { get; set; }

        [Required(ErrorMessage = "Please enter the Unit Stock")]
        public int UnitInStock { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(128)")]
        public string ProductPicture { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }


        [NotMapped] // จะไม่สร้าง table ในฐานข้อมูล
        public string CategoryName { get; set; }
    }
}