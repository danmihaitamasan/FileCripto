using DataAccess.EntityFramework;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class FileManagerService : BaseService
    {
        private readonly FileCryptoContext Context;
        public FileManagerService(ServiceDependencies serviceDependencies) : base(serviceDependencies)
        {
            Context = new FileCryptoContext();
        }
        public async Task<string> UploadFileToBlob(string strFileName, StreamReader fileData, string fileMimeType)
        {
            try
            {

                return await UploadFileToBlobAsync(strFileName, fileData, fileMimeType); ;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static string GenerateFileName(string fileName)
        {
            string[] strName = fileName.Split('.');
            string strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[^1];
            return strFileName;
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, StreamReader fileData, string fileMimeType)
        {
            try
            {
                string strContainerName = "uploads";
                CloudBlobContainer cloudBlobContainer = CloudBlobClient.GetContainerReference(strContainerName);
                string fileName = GenerateFileName(strFileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromStreamAsync(fileData.BaseStream);
                    return fileName;
                }
                return fileName;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DownloadFile(string fileName)
        {
            CloudBlockBlob blockBlob;
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                CloudBlobContainer cloudBlobContainer = CloudBlobClient.GetContainerReference("uploads");
                blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await blockBlob.DownloadToStreamAsync(memoryStream);
            }
            Stream blobStream = await blockBlob.OpenReadAsync();
            var directory = Directory.GetCurrentDirectory() + "/" + fileName.Split('/')[0];
            Directory.CreateDirectory(directory);
            using (var fileStream = System.IO.File.Create(Directory.GetCurrentDirectory() + "/" + fileName))
            {
                blobStream.Seek(0, SeekOrigin.Begin);
                blobStream.CopyTo(fileStream);
            }
        }

        public async Task<SendFileEmailModel> SaveFileReferenceToDatabase(string fileName, AddFileModel fileModel, DataAccess.IEmailSender sender)
        {
            ExecuteInTransaction(uow =>
            {
                var file = new DataAccess.Models.File
                {
                    FileName = fileName,
                    Folder = "uploads",
                    FileId = Guid.NewGuid()
                };
                file.Code = Encrypter_Decrypter.EncodePasswordToBase64(file.FileId.ToString());
                uow.Files.Insert(file);
                var fileTransferTable = new FileTransferTable
                {
                    FileId = file.FileId,
                    SenderId = fileModel.SenderId,
                    ReceiverId = fileModel.ReceiverId
                };
                uow.FileTransferTable.Insert(fileTransferTable);
                uow.SaveChanges();
                return new SendFileEmailModel()
                {
                    Code=file.Code,
                    FileId=file.FileId
                };
            });
            return null;
        }

        public async Task<DataAccess.Models.File> GetFileById(Guid id) => await Context.Files.FirstOrDefaultAsync(x => x.FileId == id);

        public async Task<List<FileTransferTable>> GetOwnFiles(int v)
        {
            return await Context.FileTransferTables
                .Include(x => x.File)
                .Where(x => x.ReceiverId == CurrentUser.UserId)
                .Skip((v - 1) * 10)
                .Take(10)
                .ToListAsync();
        }
    }
}
