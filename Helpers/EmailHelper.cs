using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace WebBanGiay.Helpers;

public static class EmailHelper
{
    public static async Task<bool> SendEmailAsync(IConfiguration configuration, string toEmail, string subject, string htmlBody)
    {
        var section = configuration.GetSection("SmtpSettings");
        var host = section["Host"];
        var fromEmail = section["FromEmail"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromEmail))
        {
            return false;
        }

        var portValue = section["Port"];
        var port = 587;
        if (!string.IsNullOrWhiteSpace(portValue) && int.TryParse(portValue, out var parsedPort))
        {
            port = parsedPort;
        }

        var enableSsl = bool.TryParse(section["EnableSsl"], out var sslEnabled) && sslEnabled;
        var userName = section["UserName"];
        var password = section["Password"];
        var fromName = section["FromName"] ?? string.Empty;

        using var message = new MailMessage();
        message.From = new MailAddress(fromEmail, fromName);
        message.To.Add(toEmail);
        message.Subject = subject;
        message.Body = htmlBody;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
        {
            client.Credentials = new NetworkCredential(userName, password);
        }

        try
        {
            await client.SendMailAsync(message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
