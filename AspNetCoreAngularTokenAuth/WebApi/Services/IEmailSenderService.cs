using System.Threading.Tasks;
using SendGrid;

namespace WebApi.Services
{
    public interface IEmailSenderService
    {
        Task<Response> SendEmail(string address, string subject, string htmlBody);
    }
}
