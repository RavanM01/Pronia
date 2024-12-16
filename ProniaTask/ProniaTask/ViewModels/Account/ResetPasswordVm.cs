using System.ComponentModel.DataAnnotations;

namespace ProniaTask.ViewModels.Account
{
    public record ResetPasswordVm
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
