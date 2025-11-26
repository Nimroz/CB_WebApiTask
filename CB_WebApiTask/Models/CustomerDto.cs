using System.ComponentModel.DataAnnotations;

namespace CB_WebApiTask.Models
{
    public class CustomerDto
    {
        [Key]
        [Display(Name = "IC NUMBER")]
        public required string IcNumber { get; set; }
        [Display(Name = "CUSTOMER NAME")]
        public required string CustomerName { get; set; }
        [Display(Name = "MOBILE NUMBER")]
        public required string MobileNumber { get; set; }
        [Display(Name = "EMAIL ADDRESS")]
        public required string EmailAddress { get; set; }
        public required string PinHash { get; set; }
    }
}
