using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebApi.Services
{
    public class EmailSenderService : IEmailSenderService
    {

        private AppSettings _appSettings { get; set; }

        public EmailSenderService(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;
        }

        public async Task<Response> SendEmail(string address, string subject, string htmlBody)
        {
            return await SendTheEmail(address, subject, htmlBody);
        }

        private async Task<Response> SendTheEmail(string address, string subject, string htmlBody)
        {
            var myMessage = new SendGridMessage();

            if (_appSettings.Environment == "DEV")
            {
                myMessage.AddTo("testemail@domain.com");
            }
            else
            {
                if (address.Contains(";"))
                {
                    var addresses = address.Split(';');
                    foreach (var addr in addresses)
                    {
                        myMessage.AddTo(addr);
                    }
                }
                else
                {
                    myMessage.AddTo(address);
                }

            }

            string fromAddress = _appSettings.SupportEmail;

            myMessage.From = new EmailAddress(fromAddress, "Company, Inc.");
            myMessage.Subject = subject;

            myMessage.HtmlContent = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>";
            myMessage.HtmlContent += "<html xmlns='http://www.w3.org/1999/xhtml' width='100%' style='margin: 0; width: 100%;'>";
            myMessage.HtmlContent += "<head>";
            myMessage.HtmlContent += "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />";
            myMessage.HtmlContent += "<title>Company</title>";
            myMessage.HtmlContent += "<style></style>";
            myMessage.HtmlContent += "</head>";
            myMessage.HtmlContent += "<body style='margin: 0; width: 100%;' width='100%'>";
            myMessage.HtmlContent += "<table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' style='padding: 0; width: 100%; max-width: 1000px;'>";
            myMessage.HtmlContent += "<tr>";
            myMessage.HtmlContent += "<td align='center' valign='top'>";
            myMessage.HtmlContent += "<table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' style='padding: 0; width: 100%;'>";
            myMessage.HtmlContent += "<tr>";
            myMessage.HtmlContent += "<td align='center' valign='top' style='padding: 0;'>";
            myMessage.HtmlContent += "<img src='https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_120x44dp.png'  >";
            myMessage.HtmlContent += "</td>";
            myMessage.HtmlContent += "</tr>";
            myMessage.HtmlContent += "<tr>";
            myMessage.HtmlContent += "<td align='center' valign='top' style='padding: 40px 20px 40px 20px;'>";

            if (_appSettings.Environment != "PROD")
            {
                myMessage.HtmlContent +=
                    string.Format("THIS EMAIL WAS GENERATED IN THE {0} ENVIRONMENT {1} it was meant for: {2}<br/><br/>",
                        _appSettings.Environment, DateTime.Now.ToLongTimeString(), address);
            }
            myMessage.HtmlContent += htmlBody;

            myMessage.HtmlContent += "</td>";
            myMessage.HtmlContent += "</tr>";
            myMessage.HtmlContent += "<tr style='background-color: #2E3C4F;  color: #FFFFFF;'>";
            myMessage.HtmlContent += "<td align='center' valign='top' style='padding: 30px 20px 30px 20px;'>";
            myMessage.HtmlContent += "<a style='color: #FFFFFF;' href='https://domain.com'>domain.com</a>";
            myMessage.HtmlContent += "&nbsp;&nbsp;&nbsp;<a style='color: #FFFFFF;' href='https://domain.com/emailsubscription/unsubscribe?email=" + address + "'>unsubscribe</a>";
            myMessage.HtmlContent += "</td>";
            myMessage.HtmlContent += "</tr>";
            myMessage.HtmlContent += "</table>";
            myMessage.HtmlContent += "</td>";
            myMessage.HtmlContent += "</tr>";
            myMessage.HtmlContent += "</table>";
            myMessage.HtmlContent += "</body>";
            myMessage.HtmlContent += "</html>";

            var client = new SendGridClient(_appSettings.SendGridApi);

            return await client.SendEmailAsync(myMessage);
        }
    }
}
