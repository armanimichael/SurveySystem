using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Data.Models;

namespace SurveySystem.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Survey> Surveys { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Survey>(survey =>
        {
            survey.HasIndex(s => s.Name).IsUnique(true);

            // Survey is hidden when created (the user should set it to visible)
            survey.Property(s => s.IsVisible).HasDefaultValue(false);
        });
    }
}