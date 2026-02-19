using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels
{
    public class CategoryViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
