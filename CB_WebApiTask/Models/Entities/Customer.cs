using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Entities
{
    public class Customer
    {

        [Key]//Ic Number Used As PK not Using id for simplicity
        [Display(Name = "IC NUMBER")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "IC Number must be exactly 8 digits.")]
        public required string IcNumber { get; set; }
        [Display(Name = "CUSTOMER NAME")]
        public required string CustomerName { get; set; }
        [Display(Name = "MOBILE NUMBER")]
        public required string MobileNumber { get; set; }
        [Display(Name = "EMAIL ADDRESS")]
        public required string EmailAddress { get; set; }
        public string? PinHash { get; set; }

        //flow flags
        public bool IsOtpVerified { get; set; } = false;
        public bool IsPrivacyAccepted { get; set; } = false;
        public bool IsFullyRegistered { get; set; } = false;

        public DateTime? LastLoginAt { get; set; }
    }
}
