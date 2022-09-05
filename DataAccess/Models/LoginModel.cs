using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class LoginModel
    {
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Max 50 characters")]
        public string Email { get; set; }
        [RegularExpression("^(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*0-9]{10,}$", ErrorMessage = "The Password must have at least one numeric, one special character, one uppercase letter and have a length of at least 10.")]
        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Max 50 characters")]
        public string Password { get; set; }
        public string ReturnURL { get; set; }
        public string InputCode { get; set; }
        public string Key { get; set; }
        public bool AreCredentialsInvalid { get; set; }
    }
}
