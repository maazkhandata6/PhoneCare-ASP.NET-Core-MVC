using System.ComponentModel.DataAnnotations;

namespace PhoneCare.Models;

public class RepairService
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = "";

    [StringLength(300)]
    public string Description { get; set; } = "";

    [Range(0, 1000000)]
    [Display(Name = "Base price (PKR)")]
    public decimal BasePrice { get; set; }

    [Range(1, 1000)]
    [Display(Name = "Estimated hours")]
    public int EstimatedHours { get; set; } = 2;

    public bool IsActive { get; set; } = true;
}
