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

        public float GetTotalPrice()
        {
            float price = 0;
            foreach (CartItem item in CartItems)
            {
                price += item.GetPrice();
            }
            return price;
        }

        public int GetItemsCount()
        {
            return CartItems.Count;
        }

        public int AddProduct(Product product)
        {
            if(product.AvailableStock<1)
            {
                throw new ArgumentException($"No enough stock for product {product.Name}");
            }
            var cartItem=CartItems.Where(ci=>ci.ProductId==product.Id).FirstOrDefault();
            if (cartItem is null)
            {
                cartItem = new CartItem
                {
                    Amount = 1,
                    Product = product,
                    ProductId = product.Id,
                    CartId = Id,
                    Cart = this,
                };
                CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Amount++;
            }
            product.AvailableStock--;
            return cartItem.Amount;
        }

        public int DecreaseItemAmount(int productId)
        {
            var cartItem=CartItems.Single(ci=>ci.ProductId == productId);
            cartItem.Amount--;
            cartItem.Product.AvailableStock++;
            return cartItem.Amount;
        }



    }
}
