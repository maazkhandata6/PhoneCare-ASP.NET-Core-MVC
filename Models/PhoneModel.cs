using System.ComponentModel.DataAnnotations;

namespace PhoneCare.Models;

public class PhoneModel
{
    public int Id { get; set; }
    [Required, StringLength(80)]
    public string Name { get; set; } = "";
    public int PhoneBrandId { get; set; }
    public PhoneBrand? PhoneBrand { get; set; }
}
