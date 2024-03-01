using ComputerNetworksProject.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;

namespace ComputerNetworksProject.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailSettings> _options;
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(IOptions<EmailSettings> options,  ILogger<EmailSender> logger)
        {
            _options = options;
            _logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string? fromEmail = _options.Value.SenderEmail;
            string? fromName = _options.Value.SenderName;
            string? apiKey = _options.Value.ApiKey;
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(htmlMessage, "<[^>]*>", "");
            var msg = MailHelper.CreateSingleEmail(from, to, subject,
            plainTextContent, htmlBase(htmlMessage));
            var response = await sendGridClient.SendEmailAsync(msg);
        }
        private string htmlBase(string htmlMessage)
        {
            string html = $@"
                        <div style=""background-color: #eaeaea; padding: 2%;"">
                            <div style=""text-align:center; margin: auto; font-size: 14px; color: #353434; max-width: 500px; border-radius: 0.375rem; background: white; padding: 50px"">
                                {htmlMessage}
                            </div>
                        </div>
                        <div style=""text-align: left; line-height: 18px; color: #666666; margin: 24px"">
                            <hr size=""1"" style=""height: 1px; border: none; color: #D8D8D8; background-color: #D8D8D8"">
                            <div style=""text-align: center"">
                            </div>
                        </div>
                        ";
            return html;
        }
    }
}
