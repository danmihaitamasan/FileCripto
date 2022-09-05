using AutoMapper;
using DataAccess;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BusinessLogic
{
    public class ServiceDependencies
    {
        public IMapper Mapper { get; set; }
        public UnitOfWork UnitOfWork { get; set; }
        public CurrentUserDto CurrentUser { get; set; }
        public CloudBlobClient CloudBlobClient { get; set; }
        public ServiceDependencies(IMapper mapper, UnitOfWork unitOfWork, CurrentUserDto currentUser)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            CurrentUser = currentUser;
            CloudBlobClient= CloudStorageAccount
                .Parse("DefaultEndpointsProtocol=https;AccountName=comanalexip;AccountKey=NonTO/6zGC3nSpEJXe8dyosSxKCMlSxKz3vIs0uFZNqbZZvdcQwIpYC4h6waX/vAf+LoZnK5YHiZ+AStjHKWug==;EndpointSuffix=core.windows.net")
                .CreateCloudBlobClient();
        }
    }
}