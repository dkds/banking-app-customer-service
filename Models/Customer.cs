using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public enum CustomerStatus
    {
        Active,
        Suspended,
        Deleted,
    }

    public class Customer
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(60, MinimumLength = 1)]
        public string? MiddleName { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string LastName { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string AddressLine1 { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName + (string.IsNullOrEmpty(MiddleName) ? "" : " " + MiddleName);
            }
        }

        public string? AddressLine2 { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string AddressCity { get; set; }

        public string Address
        {
            get
            {
                return AddressLine1 + (string.IsNullOrEmpty(AddressLine2) ? "" : ", " + AddressLine2) + ", " + AddressCity;
            }
        }

        [StringLength(20, MinimumLength = 8)]
        public string NIC { get; set; }

        [DataType(DataType.Date)]
        public string BirthDate { get; set; }

        [DataType(DataType.DateTime)]
        public string RegisterTime { get; set; } = DateTime.Now.ToUniversalTime().ToString("u").Replace(" ", "T");

        [Required]
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    }
}
