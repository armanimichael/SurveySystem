using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveySystem.Models;

namespace SurveySystem.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Survey> Surveys { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
}