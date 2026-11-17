using System.ComponentModel.DataAnnotations;

namespace PhoneCare.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Required, StringLength(20)]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    [StringLength(20)]
    public string Role { get; set; } = "Customer";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<RepairRequest> RepairRequests { get; set; } = new List<RepairRequest>();
}
