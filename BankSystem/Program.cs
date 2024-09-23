using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
                DataSeeder.Seed(context);

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

                            case "Withdraw":
                                if (input.Length == 3 && decimal.TryParse(input[2], out decimal withdrawAmount))
                                {
                                    accountService.Withdraw(input[1], withdrawAmount);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid command format or amount.");
                                }
                                break;

                            case "DeductFee":
                                if (input.Length == 2)
                                {
                                    accountService.DeductFee(input[1]);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid command format.");
                                }
                                break;

                            case "AddInterest":
                                if (input.Length == 2)
                                {
                                    accountService.AddInterest(input[1]);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid command format.");
                                }
                                break;

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
}


