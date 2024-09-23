using BankSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace StudentSystem.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Check if there's already data in the Users table
            if (!context.Users.Any())
            {
                // Add some sample users
                var user1 = new User
                {
                    UserName = "john_doe",
                    Password = "password123", // Consider hashing in real applications
                    UserEmail = "john@example.com"
                };

                var user2 = new User
                {
                    UserName = "jane_smith",
                    Password = "password456",
                    UserEmail = "jane@example.com"
                };

                context.Users.AddRange(user1, user2);
                context.SaveChanges();

                // Add saving accounts
                context.SavingAccounts.AddRange(
                    new SavingAccount
                    {
                        AccountNumber = GenerateAccountNumber(),
                        Balance = 1000m,
                        InterestRate = 1.5m,
                        User = user1
                    },
                    new SavingAccount
                    {
                        AccountNumber = GenerateAccountNumber(),
                        Balance = 500m,
                        InterestRate = 2.0m,
                        User = user2
                    }
                );

                // Add checking accounts
                context.CheckingAccounts.AddRange(
                    new CheckingAccount
                    {
                        AccountNumber = GenerateAccountNumber(),
                        Balance = 200m,
                        Fee = 2.0m,
                        User = user1
                    },
                    new CheckingAccount
                    {
                        AccountNumber = GenerateAccountNumber(),
                        Balance = 300m,
                        Fee = 3.0m,
                        User = user2
                    }
                );

                context.SaveChanges();
            }
        }

        private static string GenerateAccountNumber()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 10).Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[random.Next(36)]).ToArray());
        }
    }
}
