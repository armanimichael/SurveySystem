using System.ComponentModel.DataAnnotations;
using SurveySystem.Dtos;

namespace SurveySystem.Models;

public class Survey : SurveyDto
{
    [Key]
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;
    public bool IsVisible { get; set; }
}