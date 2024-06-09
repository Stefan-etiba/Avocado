using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<RefreshTokens> RefreshTokens { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Payment> Payments { get; set; }

    public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }
    
    public IDbConnection Connection => Database.GetDbConnection();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StudentId);
        
        base.OnModelCreating(modelBuilder);
    }
}

public class DapperContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("Configuration"));
    }
}