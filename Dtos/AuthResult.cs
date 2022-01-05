using System.Text.Json.Serialization;

namespace SurveySystem.Dtos;

public record AuthResult(string Token, [property: JsonIgnore] string RefreshToken);