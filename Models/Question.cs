using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    
    [JsonIgnore]
    public Guid SurveyId { get; set; }
}