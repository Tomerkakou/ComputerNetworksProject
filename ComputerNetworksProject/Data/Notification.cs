using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    [PrimaryKey("Email","ProductId")]
    public class Notification
    {
        public string Email { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
