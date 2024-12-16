using System.ComponentModel.DataAnnotations;

namespace ProniaTask.ViewModels.Account
{
    public class RegisterVm
    {
        [MinLength(3)]
        public string Name { get; set; }
        [MinLength(3)]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Compare(nameof(ConfirmPassword))]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
