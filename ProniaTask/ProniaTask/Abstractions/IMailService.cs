using ProniaTask.Models;

namespace ProniaTask.Abstractions
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
