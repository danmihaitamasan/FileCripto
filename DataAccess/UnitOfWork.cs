using DataAccess.EntityFramework;
using DataAccess.Models;
using System.Drawing;
using System;

namespace DataAccess
{
    public class UnitOfWork
    {
        private readonly FileCryptoContext Context;

        public UnitOfWork(FileCryptoContext context)
        {
            Context = context;
        }

        private IRepository<User> users;
        public IRepository<User> Users => users ??= new BaseRepository<User>(Context);

        private IRepository<File> files;
        public IRepository<File> Files => files ??= new BaseRepository<File>(Context);

        private IRepository<FileTransferTable> fileTransferTable;
        public IRepository<FileTransferTable> FileTransferTable => fileTransferTable ??= new BaseRepository<FileTransferTable>(Context);

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}