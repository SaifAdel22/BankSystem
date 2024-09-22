namespace BankSystem.Entities
{
    public class SavingAccount
    {
        public virtual string AccountNumber { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual decimal InterestRate { get; set; }
        public virtual string UserName { get; set; }
        public virtual User? User { get; set; }
    }
}
