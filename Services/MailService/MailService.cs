using System.Text;
using MA.SmtpHelper;
using MA.SmtpHelper.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace SurveySystem.Services.MailService;

public class MailService : IMailService
{
    private readonly IConfiguration _configuration;
    private readonly MailSenderConfiguration _senderConfiguration;
    private readonly string _from;
    private readonly string _fromName;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _from = configuration["Email:From"];
        _fromName = configuration["Email:FromName"];

        _senderConfiguration = new MailSenderConfiguration()
        {
            Host = configuration["Smtp:Host"],
            User = configuration["Smtp:User"],
            Password = configuration["Smtp:Password"],
            Port = configuration.GetValue<int>("Smtp:Port"),
            UseSsl = configuration.GetValue<bool>("Smtp:UseSsl")
        };
    }

    private IMailTo DefaultMailSender()
    {
        return MailSender
            .Connect(_senderConfiguration)
            .From(_from, _fromName);
    }

    private string GetVerificationUrl(string userId, string token)
    {
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));

        string webAppHost = _configuration["WebAppHost"];
        string webAppPath = "/account/login";
        string queryParams = $"?token={token}&userId={userId}";
        return webAppHost + webAppPath + queryParams;
    }

    private string CreateEmailVerificationBody(string userId, string token)
    {
        string url = GetVerificationUrl(userId, token);
        return @$"
            <h1>Survey System - Email verification</h1>
            <p>
                Your account is almost ready, click <a href='{url}' target='_blank'>here</a> to verify your email.
            </p>
        ";
    }

    public void SendConfirmationToken(string userEmail, string userId, string token)
    {
        string body = CreateEmailVerificationBody(token, userId);

        DefaultMailSender()
            .SendTo(userEmail, userEmail)
            .Done()
            .Subject("Survey System - Email verification")
            .Body(body, true)
            .Send();
    }
}