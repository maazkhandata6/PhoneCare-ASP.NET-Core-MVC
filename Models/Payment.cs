using System.ComponentModel.DataAnnotations;
namespace PhoneCare.Models;
public class Payment {
 public int Id {get;set;}
 public int RepairRequestId {get;set;}
 public RepairRequest? RepairRequest {get;set;}
 [Range(0,10000000)] public decimal Amount {get;set;}
 [Required,StringLength(30)] public string Method {get;set;}="Cash";
 [Required,StringLength(30)] public string Status {get;set;}="Paid";
 [StringLength(100)] public string? Reference {get;set;}
 public DateTime PaidAt {get;set;}=DateTime.UtcNow;
}