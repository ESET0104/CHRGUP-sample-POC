//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace esyasoft.mobility.CHRGUP.service.persistence.Notifications
//{
//    internal class SmtpEmailService
//    {
//    }
//}

using esyasoft.mobility.CHRGUP.service.persistence.Notifications;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient(
            _config["Email:SmtpHost"],
            int.Parse(_config["Email:SmtpPort"]))
        {
            Credentials = new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(_config["Email:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(to);

        await smtpClient.SendMailAsync(message);
    }
}
