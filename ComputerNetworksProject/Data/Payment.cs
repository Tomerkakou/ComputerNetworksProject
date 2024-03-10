
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace ComputerNetworksProject.Data
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Credit card is required")]
        [NotMapped]
        [StringLength(16, ErrorMessage = "Use valid Credit card")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Use valid Credit card")]
        public string CreditCardNumber { get; set; }

        public byte[]? CreditCardNumberEncrypt { get; set; }

        [Required(ErrorMessage ="CVV is required")]
        [StringLength(3, ErrorMessage = "Use valid cvv")]
        [RegularExpression("^[0-9]+$",ErrorMessage ="Use valid cvv")]
        public string Cvv { get; set; }

        [Required(ErrorMessage = "Month expires is required")]
        [Range(1, 12,ErrorMessage ="Use valid month")]
        public int MonthExp { get; set; }
        [Required(ErrorMessage = "Year expires is required")]
        [Range(2024, 2040, ErrorMessage = "Use valid year")]
        public int YearExp { get; set; }

        public DateTime DateOfPay { get; set; }

        [NotMapped]
        public bool Save { get; set; }
        public Payment()
        {
            DateOfPay = DateTime.Now;
        }
    }
}
