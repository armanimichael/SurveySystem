using System.Text.Json.Serialization;

namespace SurveySystem.Models;

public class ApiResponse
{
    public string? Message { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public object? MetaData { get; set; }

    [JsonIgnore]
    public int HttpStatusCode { get; set; }
}