using System.ComponentModel.DataAnnotations;

namespace Discounts.MVC.ViewModels
{
    public class OfferViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; } = string.Empty;

        public string? ImagePath { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Original Price")]
        public decimal OriginalPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Discounted Price")]
        public decimal DiscountedPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Coupon Quantity")]
        public int RemainingCoupons { get; set; }

        [Required]
        [Display(Name = "Valid From")]
        public DateTime ValidFrom { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Valid To")]
        public DateTime ValidTo { get; set; } = DateTime.Today.AddDays(30);

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        public int TimezoneOffset { get; set; }
    }
}
