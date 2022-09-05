using System;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class CurrentUserDto
    {
        public CurrentUserDto()
        {
        }

        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Image { get; set; }
    }
}