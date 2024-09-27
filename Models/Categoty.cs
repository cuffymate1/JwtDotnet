using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebStoreApi.Models
{
    [Table("Categoty", Schema = "dbo")]
    public class Category
    {
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "Please enter the Category Name")]
        [Column(TypeName = "nvarchar(50)")]
        public string CategoryName { get; set; }

        [Required]
        public int CategoryStatus { get; set; }
    }
}