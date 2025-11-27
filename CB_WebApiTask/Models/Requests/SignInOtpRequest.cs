using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class SignInOtpRequest
    {
        [Required]
        public string IcNumber { get; set; } = default!;
        // SMS or EMAIL
        [Required]
        public string DeliveryMethod { get; set; } = "SMS"; 
    }

}
