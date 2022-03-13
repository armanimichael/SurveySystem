using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Data.Models.Dtos;

[Serializable]
public class SurveyDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public bool? IsVisible { get; set; }
}