using Application.Core.Abstractions.Emails;
using Contracts.Emails;
using Infrastructure.Emails.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Emails
{
    internal sealed class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage
            {
                From =
                {
                    new MailboxAddress(_mailSettings.SenderDisplayName, _mailSettings.SenderEmail)
                },
                To =
                {
                    MailboxAddress.Parse(mailRequest.EmailTo)
                },
                Subject = mailRequest.Subject,
                Body = new TextPart(TextFormat.Text)
                {
                    Text = mailRequest.Body
                }
            };

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.SmtpPassword);

            await smtpClient.SendAsync(email);

            await smtpClient.DisconnectAsync(true);
        }
    }
}
