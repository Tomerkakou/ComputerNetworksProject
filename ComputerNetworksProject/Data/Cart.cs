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

        public void MergeCart(Cart other)
        {
            foreach (CartItem item in other.CartItems)
            {
                var cartItem = CartItems.Where(ci => ci.ProductId == item.ProductId).FirstOrDefault();
                if(cartItem != null)
                {
                    cartItem.Amount++;
                }
                else
                {
                    CartItems.Add(new CartItem
                    {
                        ProductId = item.ProductId,
                        Amount = item.Amount,
                        Product=item.Product,
                        Cart=this,
                        CartId=item.CartId,
                    });
                }
            }
            LastUpdate = DateTime.Now;
        }

        public CartItem AddProduct(Product product)
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
            LastUpdate = DateTime.Now;
            return cartItem;
        }

        public CartItem DecreaseItemAmount(int productId)
        {
            var cartItem=CartItems.Single(ci=>ci.ProductId == productId);
            cartItem.Amount--;
            cartItem.Product.AvailableStock++;
            LastUpdate = DateTime.Now;
            return cartItem;
        }

        public CartItem DeleteItem(int productId)
        {
            var cartItem = CartItems.Single(ci => ci.ProductId == productId);
            cartItem.Product.AvailableStock += cartItem.Amount;
            CartItems.Remove(cartItem);
            LastUpdate = DateTime.Now;
            return cartItem;
        }

        public List<Product> ClearCart()
        {
            foreach(var cartItem in CartItems)
            {
                cartItem.Product.AvailableStock += cartItem.Amount;
            }
            var products=CartItems.Select(ci => ci.Product).ToList();
            CartItems.Clear();
            return products;
        }



    }
}
