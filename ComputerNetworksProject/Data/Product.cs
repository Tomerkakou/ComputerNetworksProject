﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    public class Product
    {
        public enum Status
        {
            ACTIVE,
            DELETED,
        }

        [DefaultValue(Status.ACTIVE)]
        public Status ProductStatus { get; set; }
        public float? _rate;
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Must be greater then 0")]
        public float Price { get; set; }

        [Required]
        [Range(0,int.MaxValue,ErrorMessage ="Must be greater then 0")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Must be greater then 0")]
        public int? AvailableStock { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Must be greater then 0")]
        public float? PriceDiscount { get; set; }

        [Required]
        [StringLength(15,MinimumLength =1, ErrorMessage = "Length between 1 to 15")]
        public string Name { get; set; }

        [StringLength(60,ErrorMessage ="Maximum 60 letters!")]
        public string? Description { get; set; }

        public ICollection<Rate> Rates { get; set; }

        [Required]
        [ForeignKey("Category")]
        public string CategoryName { get; set; }
        public Category Category { get; set; }
        public DateTime Created { get; set; }

        public byte[]? Img { get; set; }
        public string? ImgType { get; set; }

        [NotMapped]
        public float Rate { get { return getRate(); } }

        private float getRate()
        {
            if(_rate is not null)
            {
                return (float)_rate;
            }
            if (!Rates.Any())
            {
                return 0;
            }
            try
            {
                double averageStars = Rates.Average(rate => rate.Stars);
                return (float)Math.Round(averageStars, 1);
            }catch(Exception ex)
            {
                return 0;
            }
            
        }
        public Product()
        {

        }
        public Product(string name,float price,Category category, string? description,float? priceDiscount,DateTime? created):this()
        {
            if(created is null)
            {
                created = DateTime.Now;
            }
            Created = (DateTime)created;
            Name = name;
            Price = price;
            PriceDiscount = priceDiscount;
            Description = description;
            Category = category;
            CategoryName = category.Name;
        }
    }
}
