using System.Text.Json.Serialization;

namespace SurveySystem.Data.Models.Dtos;

public record AuthResult(
    string Token,
    DateTime Expiration,
    [property: JsonIgnore] string RefreshToken,
    [property: JsonIgnore] DateTime RefreshExpire
);