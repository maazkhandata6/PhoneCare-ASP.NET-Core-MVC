using System.ComponentModel.DataAnnotations;

namespace PhoneCare.Models;

public class RepairRequest
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "Phone brand")]
    public string PhoneBrand { get; set; } = "";

    [Required, StringLength(100)]
    [Display(Name = "Phone model")]
    public string PhoneModel { get; set; } = "";

    public int RepairServiceId { get; set; }
    public RepairService? RepairService { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Problem description")]
    public string ProblemDescription { get; set; } = "";

    [StringLength(30)]
    public string Status { get; set; } = "Submitted";

    public decimal EstimatedCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
