using System.IO;
using System.Threading.Tasks;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AcmeFunEvents.Web.Services
{
    public class MessagingServices : IEmailSender
    {
        private readonly IOptions<AppOptions> _optionsAccessor;

        public MessagingServices(IOptions<AppOptions> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor;
        }
        
        public async Task SendEmailAsync(string email, string subject, string message, string attachmentPath = "")
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_optionsAccessor.Value.Smtp.ReplyAddress));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = subject;
            var builder = new BodyBuilder { HtmlBody = message };
            if (!string.IsNullOrEmpty(attachmentPath))
            {
                builder.Attachments.Add(attachmentPath);
            }
            emailMessage.Body = builder.ToMessageBody();
            await SendEmail(emailMessage);
        }
        
        public async Task SendTextEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_optionsAccessor.Value.Smtp.ReplyAddress));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };
            await SendEmail(emailMessage);
        }
        
        private async Task SendEmail(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.LocalDomain = _optionsAccessor.Value.Smtp.Server;
                await client.ConnectAsync(_optionsAccessor.Value.Smtp.Server, _optionsAccessor.Value.Smtp.Port).ConfigureAwait(false);
                client.Authenticate(_optionsAccessor.Value.Smtp.User, _optionsAccessor.Value.Smtp.Pass);
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
        
        public Task SendToFile(string email, string subject, string message)
        {
            var emailMessage = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";

            File.AppendAllText("emails.txt", emailMessage);

            return Task.FromResult(0);
        }
    }
}