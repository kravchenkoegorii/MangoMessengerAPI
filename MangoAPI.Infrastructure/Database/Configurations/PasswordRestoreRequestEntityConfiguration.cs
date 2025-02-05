﻿using MangoAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangoAPI.Infrastructure.Database.Configurations;

public class PasswordRestoreRequestEntityConfiguration : IEntityTypeConfiguration<PasswordRestoreRequestEntity>
{
    public void Configure(EntityTypeBuilder<PasswordRestoreRequestEntity> builder)
    {
        builder.ToTable(nameof(PasswordRestoreRequestEntity), MangoDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}