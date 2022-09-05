using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class File : IEntity
    {
        public File()
        {
            FileTransferTables = new HashSet<FileTransferTable>();
        }

        public Guid FileId { get; set; }
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string Code { get; set; }
        public virtual ICollection<FileTransferTable> FileTransferTables { get; set; }
    }
}
