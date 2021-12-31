using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SurveySystem.Dtos;

namespace SurveySystem.Models;

public class Question : QuestionDto
{
    public Question()
    {
        
    }

    public Question(string title, string description, Guid surveyId)
    {
        Title = title;
        Description = description;
        SurveyId = surveyId;
    }
    
    [Key]
    public Guid Id { get; set; }

    [Required]
    public bool IsMultipleChoices { get; set; }

    [JsonIgnore]
    public virtual Survey Survey { get; set; } = null!;
}