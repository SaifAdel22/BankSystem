using Azure.Identity;
using BankSystem.Entities;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Data;

namespace StudentSystem
{
    public class Program
    {
        static bool IsLoggedIn = false;
        static string LoggedInUser = null; // To keep track of the current user

        static void Main(string[] args)
        {
            using (var context = new AppDbContext())
            {
                var userService = new UserService(context);
                var accountService = new AccountService(context);

                while (true)
                {
                    if (!IsLoggedIn)
                    {
                        Console.WriteLine("Register or Login");
                        var log = Console.ReadLine()?.Trim();
                        switch (log)
                        {
                            case "Register":
                                RegisterUser(userService);
                                break;

                            case "Login":
                                LoginUser(userService);
                                break;

                            default:
                                Console.WriteLine("Invalid command. Please enter either 'Register' or 'Login'.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You are logged in as " + LoggedInUser);
                        Console.WriteLine("Available commands:");
                        Console.WriteLine(" - Logout");
                        Console.WriteLine(" - AddSavingAccount <initial balance> <interest rate>");
                        Console.WriteLine(" - AddCheckingAccount <initial balance> <fee>");
                        Console.WriteLine(" - ListAccounts");
                        Console.WriteLine(" - Deposit <Account number> <money>");
                        Console.WriteLine(" - Withdraw <Account number> <money>");
                        Console.WriteLine(" - DeductFee <Account number>");
                        Console.WriteLine(" - AddInterest <Account number>");
                        Console.WriteLine("Type your command below:");
                        var command = Console.ReadLine();
                        var input = command.Split(" ");
                        switch (input[0])
                        {
                            case "Logout":
                                Console.WriteLine("You Logged out");
                                IsLoggedIn = false;
                                LoggedInUser = null;
                                break;

                            case "AddSavingAccount":
                                AddSaving(accountService);
                                break;

                            case "AddCheckingAccount":
                                AddChecking(accountService);
                                break;

                            case "ListAccounts":
                                ListAll(accountService, context);
                                break;

                            case "Deposit":
                                if (input.Length == 3 && decimal.TryParse(input[2], out decimal depositAmount))
                                {
                                    accountService.Deposit(input[1], depositAmount);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid command format or amount.");
                                }
                                break;

                            // Additional commands can be added here.

                            default:
                                Console.WriteLine("Invalid command.");
                                break;
                        }
                    }
                }
            }
        }

        static void RegisterUser(UserService userService)
        {
            Console.WriteLine("Enter User Name:");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();
            Console.WriteLine("Enter User Email:");
            string email = Console.ReadLine();

            var registerResult = userService.Register(userName, password, email);
            Console.WriteLine(registerResult);

            if (registerResult == $"{userName} Registered")
            {
                IsLoggedIn = true;
                LoggedInUser = userName;
            }
        }

        static void LoginUser(UserService userService)
        {
            Console.WriteLine("Enter User Name:");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();

            if (userService.Login(userName, password))
            {
                Console.WriteLine("Welcome " + userName);
                IsLoggedIn = true;
                LoggedInUser = userName;
            }
            else
            {
                Console.WriteLine("Wrong UserName or Password");
            }
        }

        static void AddSaving(AccountService accountService)
        {
            Console.WriteLine("Enter User Name:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter Initial Balance:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
            {
                Console.WriteLine("Invalid initial balance. Please enter a valid number.");
                return;
            }

            Console.WriteLine("Enter Interest Rate:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal interestRate))
            {
                Console.WriteLine("Invalid interest rate. Please enter a valid number.");
                return;
            }

            accountService.AddSavingAccount(username, initialBalance, interestRate);
        }

        static void AddChecking(AccountService accountService)
        {
            Console.WriteLine("Enter User Name:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter Initial Balance:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
            {
                Console.WriteLine("Invalid initial balance. Please enter a valid number.");
                return;
            }

            Console.WriteLine("Enter Fee:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal fee))
            {
                Console.WriteLine("Invalid fee. Please enter a valid number.");
                return;
            }

            accountService.AddCheckingAccount(username, initialBalance, fee);
        }

        static void ListAll(AccountService accountService, AppDbContext context)
        {
            var user = context.Users
                .Include(u => u.SavingAccounts)
                .Include(u => u.CheckingAccounts)
                .SingleOrDefault(u => u.UserName == LoggedInUser);

            if (user != null)
            {
                Console.WriteLine($"Accounts for user: {user.UserName}");

                Console.WriteLine("Saving Accounts:");
                foreach (var savingAccount in user.SavingAccounts.OrderBy(a => a.AccountNumber))
                {
                    Console.WriteLine($"-- {savingAccount.AccountNumber} {savingAccount.Balance}");
                }

                Console.WriteLine("Checking Accounts:");
                foreach (var checkingAccount in user.CheckingAccounts.OrderBy(a => a.AccountNumber))
                {
                    Console.WriteLine($"-- {checkingAccount.AccountNumber} {checkingAccount.Balance}");
                }
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }
    }

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

        private string GenerateAccountNumber()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 10).Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[random.Next(36)]).ToArray());
        }
    }
}
