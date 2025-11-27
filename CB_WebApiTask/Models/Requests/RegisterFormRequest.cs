using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class RegisterFormRequest
    {
        [Required] public string IcNumber { get; set; } = default!;
        [Required] public string CustomerName { get; set; } = default!;
        [Required] public string MobileNumber { get; set; } = default!;
        [Required] public string EmailAddress { get; set; } = default!;
    }

}
