﻿using MangoAPI.Application.Services;
using MangoAPI.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace MangoAPI.DataAccess.Database
{
    public class DesignTimeUserDbContextFactory : IDesignTimeDbContextFactory<MangoPostgresDbContext>
    {
        public MangoPostgresDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MangoPostgresDbContext>();

            var connectionString = EnvironmentConstants.MangoDatabaseUrl;

            connectionString = StringService.ConvertHerokuDbConnection(connectionString);

            optionsBuilder.UseNpgsql(connectionString ??
                                     throw new InvalidOperationException(
                                         "Wrong Connection String in DesignTimeUserDbContextFactory class."));

            return new MangoPostgresDbContext(optionsBuilder.Options);
        }
    }
}