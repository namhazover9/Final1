using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWeb1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        public string? Condition { get; set; }
        public string? Status { get; set; } = "Available";
        public bool? IsPay { get; set; } = false;
        public bool? IsDeleted { get; set; } = false;
        public string? isBrowsed { get; set; } = "Pending";
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }



        [ValidateNever] 
        public List<ProductImage> ProductImages { get; set; }
    }
}
