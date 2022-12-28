using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerService.Models
{

    public enum AccountStatus
    {
        Active,
        Suspended,
        Deleted,
    }

    public class Account
    {
        public int Id { get; set; }

        [StringLength(16, MinimumLength = 4)]
        public string Number { get; set; } = "";

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }

        [StringLength(5, MinimumLength = 3)]
        [Required]
        public string Currency { get; set; } = "LKR";

        [DataType(DataType.DateTime)]
        public string StartTime { get; set; } = DateTime.Now.ToUniversalTime().ToString("u").Replace(" ", "T");

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        public int CustomerId { get; set; }
    }
}
