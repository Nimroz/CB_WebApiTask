using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class RegisterFormRequest
    {
        [StringLength(8, MinimumLength = 8, ErrorMessage = "IC Number must be exactly 8 digits.")]
        [Required] public string IcNumber { get; set; } = default!;
        [Required] public string CustomerName { get; set; } = default!;
        [Required] public string MobileNumber { get; set; } = default!;
        [Required] public string EmailAddress { get; set; } = default!;
    }

}
