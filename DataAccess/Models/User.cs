using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace DataAccess.Models
{
    public partial class User : IEntity
    {
        public User()
        {
            FileTransferTableReceivers = new HashSet<FileTransferTable>();
            FileTransferTableSenders = new HashSet<FileTransferTable>();
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public string PasswordToken { get; set; }
        public bool IsValidated { get; set; }

        public virtual ICollection<FileTransferTable> FileTransferTableReceivers { get; set; }
        public virtual ICollection<FileTransferTable> FileTransferTableSenders { get; set; }
    }
}
