using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FileSharingPractice.Helpers.Mail
{
    public class Mailhelper : IMailService
    {
        private readonly IConfiguration _config;

        public Mailhelper(IConfiguration config)
        {
            this._config = config;
        }
        public void SendEmail(InputEmailMessage model)
        {
            using (SmtpClient client = new SmtpClient(_config.GetValue<string>("Mail:Host"), _config.GetValue<int>("Mail:Port")))
            {
   
                var msg = new MailMessage();
                msg.To.Add(model.Email);
                msg.Subject = model.Subject;
                msg.Body = model.Body;
                msg.From = new MailAddress(_config.GetValue<string>("Mail:From"), _config.GetValue<string>("Mail:Sender"), System.Text.Encoding.UTF8);
                msg.IsBodyHtml = true;
                client.Credentials = new System.Net.NetworkCredential(_config.GetValue<string>("Mail:From"), _config.GetValue<string>("Mail:PWD"));
                client.SendMailAsync(msg);


            } 
   
        }
    }
}
