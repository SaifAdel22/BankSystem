using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankSystem.Entities;

namespace BankSystem.Data.Config
{
    public class CheckingAccountConfig : IEntityTypeConfiguration<CheckingAccount>
    {
        public void Configure(EntityTypeBuilder<CheckingAccount> builder)
        {
            builder.HasKey(x=>x.AccountNumber);
            builder.Property(x => x.AccountNumber).IsRequired();
            builder.Property(x => x.Balance).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Fee).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasOne(x => x.User).WithMany(x => x.CheckingAccounts).HasForeignKey(x => x.UserName);
            builder.ToTable("CheckingAccounts");

        }
    }
}
