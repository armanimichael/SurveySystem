using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Models;

public class Survey
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}