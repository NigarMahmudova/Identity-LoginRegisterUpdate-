using System.ComponentModel.DataAnnotations;

namespace PustokBookStore.ViewModels
{
    public class MemberLoginVM
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
