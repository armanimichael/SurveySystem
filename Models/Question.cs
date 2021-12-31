using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SurveySystem.Dtos;

namespace SurveySystem.Models;

public class Question : QuestionDto
{
    [Key]
    public Guid Id { get; set; }

    [JsonIgnore]
    public virtual Survey Survey { get; set; } = null!;
}