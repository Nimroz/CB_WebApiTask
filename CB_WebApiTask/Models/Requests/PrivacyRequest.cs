using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class PrivacyRequest
    {
        [Required] public string IcNumber { get; set; } = default!;
        [Required] public bool Accept { get; set; }
        public string Purpose { get; set; } = "Registration"; // or "SignIn"
    }
}
