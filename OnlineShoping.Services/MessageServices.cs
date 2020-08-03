using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace DemoApp.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public IConfiguration iconfiguration;
        public AuthMessageSender(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                //From Address  
                string FromAddress = "princedigitalsaree@gmail.com";
                string FromAdressTitle = "Prince Digital";
                string Gmailid = iconfiguration["GmailId"];
                string Gmailpassword = iconfiguration["GmailPassword"];
                //To Address  
                string ToAddress = email;
                string ToAdressTitle =email;
                string Subject = subject;
                string BodyContent = message;

                //Smtp Server  
                string SmtpServer = "smtp.gmail.com";
                //Smtp Port Number  
                int SmtpPortNumber = 587;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress
                                        (FromAdressTitle,
                                         FromAddress
                                         ));
                mimeMessage.To.Add(new MailboxAddress
                                         (ToAdressTitle,
                                         ToAddress
                                         ));
                mimeMessage.Subject = Subject; //Subject
                mimeMessage.Body = new TextPart("HTML")
                {
                    Text = BodyContent
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate(
                        Gmailid,
                        Gmailpassword
                        );
                    await client.SendAsync(mimeMessage);
                   
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
