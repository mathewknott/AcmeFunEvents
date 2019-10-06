using System.Threading.Tasks;

namespace AcmeFunEvents.Web.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, string attachmentPath = "");

        Task SendTextEmailAsync(string email, string subject, string message);
    }
}
