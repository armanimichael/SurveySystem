namespace SurveySystem.Models;

public class AuthResult
{
    public string Token {get; set;}
    public string RefreshToken {get; set;}

    public AuthResult(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }
}