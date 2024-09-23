using BankSystem.Entities;
using StudentSystem.Data;

namespace StudentSystem
{
    public class AccountService
    {
        private readonly AppDbContext context;

        public AccountService(AppDbContext _context)
        {
            context = _context;
        }

        public void AddSavingAccount(string userName, decimal initialBalance, decimal interestRate)
        {
            var user = context.Users.SingleOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                var savingAccount = new SavingAccount
                {
                    AccountNumber = GenerateAccountNumber(),
                    Balance = initialBalance,
                    InterestRate = interestRate,
                    User = user
                };
                context.SavingAccounts.Add(savingAccount);
                context.SaveChanges();
                Console.WriteLine($"Saving account added for {userName}");
            }
        }

        public void AddCheckingAccount(string userName, decimal initialBalance, decimal fee)
        {
            var user = context.Users.SingleOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                var checkingAccount = new CheckingAccount
                {
                    AccountNumber = GenerateAccountNumber(),
                    Balance = initialBalance,
                    Fee = fee,
                    User = user
                };
                context.CheckingAccounts.Add(checkingAccount);
                context.SaveChanges();
                Console.WriteLine($"Checking account added for {userName}");
            }
        }

        public void Deposit(string accountNumber, decimal amount)
        {
            var checkingAccount = context.CheckingAccounts.SingleOrDefault(x => x.AccountNumber == accountNumber);
            var savingAccount = context.SavingAccounts.SingleOrDefault(a => a.AccountNumber == accountNumber);

            if (checkingAccount != null)
            {
                checkingAccount.Balance += amount;
                context.SaveChanges();
                Console.WriteLine($"Deposited {amount} to checking account {accountNumber}. New balance: {checkingAccount.Balance}");
            }
            else if (savingAccount != null)
            {
                savingAccount.Balance += amount;
                context.SaveChanges();
                Console.WriteLine($"Deposited {amount} to saving account {accountNumber}. New balance: {savingAccount.Balance}");
            }
            else
            {
                Console.WriteLine("Account not found.");
            }
        }

        public void Withdraw(string accountNumber, decimal amount)
        {
            var checkingAccount = context.CheckingAccounts.SingleOrDefault(x => x.AccountNumber == accountNumber);
            var savingAccount = context.SavingAccounts.SingleOrDefault(a => a.AccountNumber == accountNumber);

            if (checkingAccount != null)
            {
                if (checkingAccount.Balance >= amount)
                {
                    checkingAccount.Balance -= amount;
                    context.SaveChanges();
                    Console.WriteLine($"Withdrew {amount} from checking account {accountNumber}. New balance: {checkingAccount.Balance}");
                }
                else
                {
                    Console.WriteLine($"Insufficient funds in checking account {accountNumber}. Current balance: {checkingAccount.Balance}");
                }
            }
            else if (savingAccount != null)
            {
                if (savingAccount.Balance >= amount)
                {
                    savingAccount.Balance -= amount;
                    context.SaveChanges();
                    Console.WriteLine($"Withdrew {amount} from saving account {accountNumber}. New balance: {savingAccount.Balance}");
                }
                else
                {
                    Console.WriteLine($"Insufficient funds in saving account {accountNumber}. Current balance: {savingAccount.Balance}");
                }
            }
            else
            {
                Console.WriteLine("Account not found.");
            }
        }

        public void DeductFee(string accountNumber)
        {
            var checkingAccount = context.CheckingAccounts.SingleOrDefault(a => a.AccountNumber == accountNumber);

            if (checkingAccount != null)
            {
                if (checkingAccount.Balance >= checkingAccount.Fee)
                {
                    checkingAccount.Balance -= checkingAccount.Fee;
                    context.SaveChanges();
                    Console.WriteLine($"Fee of {checkingAccount.Fee} deducted from checking account {accountNumber}. New balance: {checkingAccount.Balance}");
                }
                else
                {
                    Console.WriteLine($"Insufficient funds to deduct fee from checking account {accountNumber}. Current balance: {checkingAccount.Balance}");
                }
            }
            else
            {
                Console.WriteLine("Checking account not found.");
            }
        }

        public void AddInterest(string accountNumber)
        {
            var savingAccount = context.SavingAccounts.SingleOrDefault(a => a.AccountNumber == accountNumber);

            if (savingAccount != null)
            {
                var interest = savingAccount.Balance * (savingAccount.InterestRate / 100);
                savingAccount.Balance += interest;
                context.SaveChanges();
                Console.WriteLine($"Interest of {interest} added to saving account {accountNumber}. New balance: {savingAccount.Balance}");
            }
            else
            {
                Console.WriteLine("Saving account not found.");
            }
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 10).Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[random.Next(36)]).ToArray());
        }
    }
}


