using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Entities
{
    public class OtpSession
    {
        [Key]
        public int Id { get; set; }
        public string? IcNumber { get; set; }
        public string MobileNumber { get; set; } = default!;
        public string? EmailAddress { get; set; }
        public string Code { get; set; } = default!;
        public string Purpose { get; set; } = "Registration"; // Registration or SignIn
        public string DeliveryMethod { get; set; } = "SMS"; // SMS or EMAIL
        public bool IsVerified { get; set; } = false;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
