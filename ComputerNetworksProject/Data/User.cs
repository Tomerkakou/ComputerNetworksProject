using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    public class User : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? ImgType { get; set; }

        [ForeignKey("PaymentId")]
        public int? PaymentId { get; set; }
        public Payment? SavedPayment { get; set; }

        [ForeignKey("ShippingId")]
        public int? ShippingId { get; set; }
        public Shipping? SavedShipping { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
