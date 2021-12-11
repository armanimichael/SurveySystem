using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Dtos;

[Serializable]
public class SurveyDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}