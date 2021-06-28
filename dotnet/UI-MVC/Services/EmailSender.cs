using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace UI.MVC.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;


        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            _logger = logger;
            Options = optionsAccessor.Value;
            _logger.Log(LogLevel.Debug, optionsAccessor.Value.SendGridKey);
            _logger.Log(LogLevel.Debug, optionsAccessor.Value.SendGridUser);
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            //Als het breekt in production terug zo als tijdelijke fix. Liefst in werking krijgen via startup.cs en options accessor
            /*if (Options.SendGridKey == null)
            {
                return Execute("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", subject,
                    message, email);
            }*/

            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("arthur.decraemer@student.kdg.be", Options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}