using System.ComponentModel.DataAnnotations;

namespace DatingApp.ApI.DTOS
{
    public class UserToRegister
    {
        [Required]
        public string username { get; set; }

        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="You must enter a password between 4 and 8 characters")]
        public string password { get; set; }
    }

    
}