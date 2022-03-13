namespace SurveySystem.Services.MailService;

public interface IMailService
{
    public void SendConfirmationToken(string userEmail, string userId, string token);
}