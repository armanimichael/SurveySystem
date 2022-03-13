using System.ComponentModel.DataAnnotations;
using SurveySystem.Data.Models.Dtos;

namespace SurveySystem.Data.Models;

public class Survey : SurveyDto
{
    public Survey()
    {
        
    }

    public Survey(string name, string? description, bool? isVisible)
    {
        Name = name;
        Description = description;
        IsVisible = isVisible;
    }
    
    public Survey(Guid id, string name, string? description, bool? isVisible)
    {
        Id = id;
        Name = name;
        Description = description;
        IsVisible = isVisible;
    }
    
    [Key]
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;
}