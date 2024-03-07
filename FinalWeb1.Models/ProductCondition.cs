using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinalWeb1.Models
{
    public class ProductCondition
    {
        [Key]
        public int ConditionId { get; set; }
        [Required]       
        public string Name { get; set; }

    }
}
