using System.Threading.Tasks;

namespace EmailServiceNetCore.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    
}