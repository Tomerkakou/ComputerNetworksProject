using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ComputerNetworksProject.Data
{
    
    public class Cart
    {
        public enum Status
        {
            ACTIVE,
            DELETED,
            COMPLETED,
        }

        [DefaultValue(Status.ACTIVE)]
        public Status CartStatus { get; set; }
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public virtual Payment? Payment { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

        public Cart()
        {
            LastUpdate = DateTime.Now;
            CartItems = new List<CartItem>();
        }

        public Cart(User? user = null) : this()
        {
            if (user is not null)
            {
                User = user;
                UserId = user.Id;
            }
        }

    }
}
