using BankSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Config
{
    public class SavingAccountConfig : IEntityTypeConfiguration<SavingAccount>
    {
        public void Configure(EntityTypeBuilder<SavingAccount> builder)
        {
            builder.HasKey(x => x.AccountNumber);
            builder.Property(x => x.AccountNumber).IsRequired();
            builder.Property(x => x.Balance).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.InterestRate).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasOne(x => x.User).WithMany(x => x.SavingAccounts).HasForeignKey(x => x.UserName);
            builder.ToTable("SavingAccounts");

        }
    }
}
