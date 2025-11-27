using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models.Requests
{
    public class CreatePinRequest
    {
        [Required] public string IcNumber { get; set; } = default!;
        [Required][StringLength(6, MinimumLength = 6)] public string Pin { get; set; } = default!;
        [Required][StringLength(6, MinimumLength = 6)] public string ConfirmPin { get; set; } = default!;
    }
}
