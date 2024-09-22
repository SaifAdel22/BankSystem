using BankSystem.Entities;
using StudentSystem.Data;
using System.Text.RegularExpressions;

namespace StudentSystem
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public string Register(string username, string password, string userEmail)
        {
            if (!ValidateEmail(userEmail)) { return "Invalid User Email"; }
            if (!ValidateUsername(username)) { return "Invalid User Name"; }
            if (!ValidatePassword(password)) { return "Invalid Password"; }

            if (_context.Users.Any(c => c.UserName == username))
                return "User Name already exists";

            var newUser = new User { UserName = username, Password = password, UserEmail = userEmail };
            _context.Users.Add(newUser);
            _context.SaveChanges();  // Save changes to the database
            return $"{username} Registered";
        }

        private bool ValidateUsername(string username)
        {
            return Regex.IsMatch(username, "^[a-zA-Z][a-zA-Z0-9]{2,}$");
        }

        private bool ValidatePassword(string password)
        {
            return password.Length > 6 &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit);
        }

        private bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }

        public bool Login(string userName, string password)
        {
            return _context.Users.Any(x => x.UserName == userName && x.Password == password);
        }
    }
}
