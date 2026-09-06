using System.Net;
using System.Net.Mail;
using System.Text;

namespace __NAMESPACE__.EmailClient
{
    public class EmailClient(EmailClientOptions options) : IEmailClient
    {
        private readonly EmailClientOptions Options = options ?? throw new ArgumentNullException(nameof(options));

        public async Task SendEmailAsync(string emailTo, string subject, string body, bool isBodyHtml = false)
            => await SendEmailAsync(new List<string> { emailTo }, null, null, null, subject, body, isBodyHtml);

        public async Task SendEmailAsync(string emailTo, string attachment, string subject, string body, bool isBodyHtml = false)
            => await SendEmailAsync(new List<string> { emailTo }, null, new List<string> { attachment }, null, subject, body, isBodyHtml);

        public async Task SendEmailAsync(string emailTo, Attachment attachment, string subject, string body, bool isBodyHtml = false)
            => await SendEmailAsync(new List<string> { emailTo }, null, null, new List<Attachment> { attachment }, subject, body, isBodyHtml);

        public async Task SendEmailAsync(IEnumerable<string> emailsTo, IEnumerable<string>? emailsCC, IEnumerable<string>? fileAttachments, IEnumerable<Attachment>? attachments, string subject, string body, bool isBodyHtml = false)
        {
            if (emailsTo == null)
                throw new Exception("To email list cannot be null");

            if (!emailsTo.Any())
                throw new Exception("To email list cannot be empty");

            if (string.IsNullOrEmpty(subject))
                throw new Exception("The subject cannot be empty");

            if (string.IsNullOrEmpty(body))
                throw new Exception("The body cannot be empty");

            using var mailMessage = new MailMessage();

            foreach (var emailTo in emailsTo)
                mailMessage.To.Add(emailTo);

            if (emailsCC != null)
            {
                foreach (var emailCC in emailsCC)
                    mailMessage.CC.Add(emailCC);
            }

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                    mailMessage.Attachments.Add(new Attachment(attachment));
            }

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                    mailMessage.Attachments.Add(attachment);
            }

            var from = string.IsNullOrWhiteSpace(Options.SmtpFrom) ? Options.SmtpMail : Options.SmtpFrom;
            var displayName = string.IsNullOrWhiteSpace(Options.SmtpFromDisplayName) ? from : Options.SmtpFromDisplayName;

            mailMessage.From = new MailAddress(from ?? string.Empty, displayName, Encoding.UTF8);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isBodyHtml;

            using var smtpClient = new SmtpClient(Options.SmtpServer, Options.SmtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            smtpClient.Credentials = new NetworkCredential(Options.SmtpMail, Options.SmtpPassword);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
