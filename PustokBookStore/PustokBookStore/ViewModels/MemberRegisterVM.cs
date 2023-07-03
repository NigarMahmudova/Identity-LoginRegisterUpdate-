using System.ComponentModel.DataAnnotations;

namespace PustokBookStore.ViewModels
{
    public class MemberRegisterVM
    {
        [Required]
        [MaxLength(35)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ComfirmPassword { get; set; }

    }
}
