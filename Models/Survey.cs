using System.ComponentModel.DataAnnotations;
using SurveySystem.Dtos;

namespace SurveySystem.Models;

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
    
    [Key]
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;
}