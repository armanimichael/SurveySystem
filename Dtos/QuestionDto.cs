using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Dtos;

[Serializable]
public class QuestionDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;
    
    [Required]
    public Guid SurveyId { get; set; }
}