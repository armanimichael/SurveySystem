using Microsoft.EntityFrameworkCore;
using SurveySystem.Models;

namespace SurveySystem.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Survey> Surveys { get; set; }
    
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
}