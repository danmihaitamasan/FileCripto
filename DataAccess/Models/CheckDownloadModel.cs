using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CheckDownloadModel
    {
        public Guid FileId { get; set; }
        public string Code { get; set; }
    }
}
