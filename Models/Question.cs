using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SurveySystem.Dtos;

namespace SurveySystem.Models;

public class Question : QuestionDto
{
    public Question()
    {
    }

    public Question(string title, string description, Guid surveyId, bool isMultipleChoices)
    {
        Title = title;
        Description = description;
        SurveyId = surveyId;
        IsMultipleChoices = isMultipleChoices;
    }

    [Key]
    public Guid Id { get; set; }

    [JsonIgnore]
    public virtual Survey Survey { get; set; } = null!;
}