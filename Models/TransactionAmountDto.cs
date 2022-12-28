namespace CustomerService.Models
{
    public enum TransactionType
    {
        Credit,
        Debit,
        Undefined,
    }

    public class TransactionAmountDto
    {

        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }
    }
}
