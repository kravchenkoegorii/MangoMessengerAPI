﻿using MangoAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangoAPI.Infrastructure.Database.Configurations;

public class DocumentEntityConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.ToTable(nameof(DocumentEntity), MangoDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FileName).IsRequired();
        builder.Property(x => x.UploadedAt).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
    }
}