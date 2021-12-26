using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Models;

public class Question
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public bool IsMultipleChoices { get; set; }
}