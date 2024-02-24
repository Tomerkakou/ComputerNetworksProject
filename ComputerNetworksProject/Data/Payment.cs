using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [NotMapped]
        [StringLength(16)]
        [RegularExpression("/^[0-9]+$/")]
        private string CreditCardNumber { get; set; }

        public byte[]? CreditCardNumberEncrypt { get; set; }

        [Required]
        [StringLength(3)]
        [RegularExpression("/^[0-9]+$/")]
        public string Cvv { get; set; }

        [Required]
        public int MonthExp { get; set; }
        [Required]
        public int YearExp { get; set; }

        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public DateTime DateOfPay { get; set; }

        public Payment()
        {
            DateOfPay = DateTime.Now;
        }

        public Payment(string creditCardNumber,string cvv,int monthExp,int yearExp,Cart cart,User? user):this()
        {
            CreditCardNumber = creditCardNumber;
            Cvv= cvv;
            MonthExp = monthExp;
            YearExp = yearExp;
            Cart = cart;
            CartId= cart.Id;

            if(user is not null)
            {
                User = user;
                UserId = user.Id;
            }
        }
    }
}
