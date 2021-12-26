using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Models;

namespace SurveySystem.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Survey> Surveys { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SurveysConfig(builder);
        QuestionsConfig(builder);
    }
    
    private static void QuestionsConfig(ModelBuilder builder)
    {
        builder.Entity<Question>(question =>
        {
            question
                .HasIndex(q => new { q.Title, q.SurveyId })
                .IsUnique();
        });
    }

    private static void SurveysConfig(ModelBuilder builder)
    {
        builder.Entity<Survey>(survey =>
        {
            survey.HasIndex(s => s.Name).IsUnique(true);

            // Survey is hidden when created (the user should set it to visible)
            survey.Property(s => s.IsVisible).HasDefaultValue(false);

            survey
                .HasMany(s => s.Questions)
                .WithOne(q => q.Survey)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}