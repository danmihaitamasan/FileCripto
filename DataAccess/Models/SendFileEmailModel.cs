using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class SendFileEmailModel
    {
        public Guid FileId { get; set; }
        public string Code { get; set; }
    }
}
