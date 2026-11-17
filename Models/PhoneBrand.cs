using System.ComponentModel.DataAnnotations;

namespace PhoneCare.Models;

public class PhoneBrand
{
    public int Id { get; set; }
    [Required, StringLength(60)]
    public string Name { get; set; } = "";
    public ICollection<PhoneModel> Models { get; set; } = new List<PhoneModel>();
}
