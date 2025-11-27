using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class RequestOtpRequest
    {
        [Required] public string IcNumber { get; set; } = default!;
        [Required] public string MobileNumber { get; set; } = default!;
        [Required] public string OtpDeliveryMethod { get; set; } = "SMS"; // or "Email"
    }
}
