using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class VerifyOtpRequest
    {
        [Required] public string IcNumber { get; set; } = default!;
        [Required] public string Otp { get; set; } = default!;
        //purpose to reuse: "Registration" or "SignIn"
        public string Purpose { get; set; } = "Registration";
    }
}
