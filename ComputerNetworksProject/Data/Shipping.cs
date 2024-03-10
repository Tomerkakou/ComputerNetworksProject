using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    public class Shipping
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        [Required(ErrorMessage ="Building number required")]
        public int BuildingNum { get; set; }
        [Required(ErrorMessage = "Apartment number required")]
        public int ApartmentNum { get; set; }
        [Required(ErrorMessage = "Zip code required")]
        [StringLength(7, ErrorMessage = "Use valid Zip")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Use valid Zip")]
        public string ZipCode { get; set; }

        [NotMapped]
        public bool Save {  get; set; }
    }
}
