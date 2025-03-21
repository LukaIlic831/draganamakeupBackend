using System.Net;
using System.Net.Mail;

namespace DraganaMakeup.Services;

public class EmailService
{

    public EmailService()
    {
    }


    public async Task SendEmail(string body)
    {
        try
        {
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL");
            var senderName = "DraganaMakeup";
            var password = Environment.GetEnvironmentVariable("GOOGLE_APP_PASSWORD");
            var toEmail = Environment.GetEnvironmentVariable("RECEIVER_EMAIL");

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(toEmail))
            {
                throw new Exception("Sender email or password or receiver email is not set in environment variables.");
            }

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Zakazan Termin",
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}