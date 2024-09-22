using BankSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSystem.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x=>x.UserName);
            builder.Property(x=>x.UserName).IsRequired().HasMaxLength(50);
            builder.Property(x=>x.Password).IsRequired().HasMaxLength(50);
            builder.Property(x => x.UserEmail).IsRequired().HasMaxLength(100);
            builder.ToTable("Users");
        }
    }
}
