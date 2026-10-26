using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)                  // EntityTypeBuilder<User> : if you want to write a configuration class for a specific entity, implement this interface, and I'll know how to find and use it automatically.
        {
            builder.HasKey(u => u.Id);                                          // builder.HasKey(...) is a Fluent API method in Entity Framework Core used to explicitly define the Primary Key for a database table

            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Role).IsRequired().HasMaxLength(20);

            builder.HasIndex(u => u.Email).IsUnique();                          // Every user must have a unique email address and also, make searching by email fast using indexing.
        }                                                                       // builder.HasIndex(...) is a Fluent API configuration method used to create a database index on one or more columns of a table for optimizing search speeds.
    }                                                                           // When you run Builder.HasIndex, the database engine allocates a completely separate block of storage space just for that index map which is a hidden system file managed by the database backend.
}