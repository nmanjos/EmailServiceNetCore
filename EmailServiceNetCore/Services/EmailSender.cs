using System;
using System.Threading.Tasks;
using EmailServiceNetCore.Options;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailServiceNetCore.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions emailOptions;
        private readonly IHostingEnvironment _env;
        private readonly ILogger logger;

        public EmailSender(
            IOptions<EmailOptions> EmailOptions,
            IHostingEnvironment env,  ILogger<EmailSender> Logger)
        {
            emailOptions = EmailOptions.Value;
            _env = env;
            logger = Logger;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(emailOptions.SenderName, emailOptions.Sender));

                mimeMessage.To.Add(new MailboxAddress(email));

                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    if (_env.IsDevelopment())
                    {
                        // The third parameter is useSSL (true if the client should make an SSL-wrapped
                        // connection to the server; otherwise, false).
                        await client.ConnectAsync(emailOptions.MailServer, emailOptions.MailPort, true);
                    }
                    else
                    {
                        await client.ConnectAsync(emailOptions.MailServer);
                    }

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(emailOptions.Sender, emailOptions.Password);

                    await client.SendAsync(mimeMessage);

                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}