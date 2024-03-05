﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    [PrimaryKey(nameof(CartId),nameof(ProductId))]
    public class CartItem
    {


        [DefaultValue(0)]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Amount { get; set; }
  
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [Required]
        public Product Product { get; set; }
        
        public int CartId { get; set; }

        [ForeignKey("CartId")]
        [Required]
        public Cart Cart { get; set; }

        public CartItem() { }
        public CartItem(Product product, Cart cart) :this()
        {
            Product = product;
            ProductId=product.Id;
            CartId = cart.Id;
            Cart = cart;
        }

        public float GetPrice()
        {
            return Product.Price*Amount;
        }
    }
}
