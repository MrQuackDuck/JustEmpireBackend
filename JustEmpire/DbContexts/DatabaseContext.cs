﻿using JustEmpire.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace JustEmpire.DbContexts;

public class DatabaseContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceCategory> ServiceCategories { get; set; }
    public DbSet<ServiceImage> ServiceImages { get; set; }
    public DbSet<ServiceVersion> ServiceVersions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PageView> PageViews { get; set; }

    public string DbPath { get; }

    public DatabaseContext() => DbPath = "data.db";

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}