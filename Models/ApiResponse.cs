namespace SurveySystem.Models;

public class ApiResponse
{
    public string? Message { get; set; } = null;
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}