using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

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

        public ICollection<Payment>? Payments { get; set; }
    }
}
