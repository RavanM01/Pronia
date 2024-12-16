using System.ComponentModel.DataAnnotations;

namespace ProniaTask.ViewModels.Account
{
    public record ForgotPasswordVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
