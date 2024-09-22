using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Entities
{
    public class User
    {
        public virtual string UserName {  get; set; }
        public virtual string Password {  get; set; }
        public virtual string UserEmail {  get; set; }
        public virtual List<SavingAccount> SavingAccounts { get; set; } = new List<SavingAccount>();
        public virtual List<CheckingAccount> CheckingAccounts { get; set; } = new List<CheckingAccount>();
    }
}
