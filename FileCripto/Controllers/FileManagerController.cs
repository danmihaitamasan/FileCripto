using BusinessLogic.Services;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCrypto.Controllers
{
    public class FileManagerController : BaseController
    {
        private readonly FileManagerService Service;
        private IHostingEnvironment env;
        IEmailSender Sender;
        private UserAccountService UserAccountService;
        public FileManagerController(ControllerDependencies dependencies, FileManagerService service, IHostingEnvironment _env, IEmailSender sender, UserAccountService userAccountService)
         : base(dependencies)
        {
            env = _env;
            Service = service;
            Sender = sender;
            UserAccountService = userAccountService;
        }
        [HttpGet]
        public ActionResult AddFile()
        {
            return View(new AddFileModel());
        }
        [HttpGet]
        public ActionResult CheckDownload(Guid id)
        {
            var checkDownloadMode = new CheckDownloadModel
            {
                FileId = id
            };
            return View(checkDownloadMode);

        }
        [HttpPost]
        public async Task<ActionResult> CheckDownload(CheckDownloadModel model)
        {
            var file = await Service.GetFileById(model.FileId);
            if (file != null && (file.Code == model.Code))
            {
                await Service.DownloadFile(file.FileName);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Code", "The code is incorrect");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<ActionResult> OwnFiles()
        {
            var x = await Service.GetOwnFiles(1);
            return View((await Service.GetOwnFiles(1)).Select(x => x.File).ToList());
        }
        [HttpGet]
        public async Task<List<DataAccess.Models.File>> GetOwnFilesPerPage(int page)
        {
            return (await Service.GetOwnFiles(page)).Select(x => x.File).ToList();
        }
        [HttpPost]
        public async Task<ActionResult> AddFileAsync(AddFileModel fileModel)
        {
            if (ModelState.IsValid)
            {

                fileModel.SenderId = CurrentUser.UserId;
                var uploads = Path.Combine(env.WebRootPath, "uploads");
                bool exists = Directory.Exists(uploads);
                if (!exists)
                    Directory.CreateDirectory(uploads);

                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {

                    await fileModel.File.CopyToAsync(stream);
                }
                var fileStream = new StreamReader(fileModel.File.OpenReadStream());
                string mimeType = fileModel.File.ContentType;

                var fileName = await Service.UploadFileToBlob(fileModel.File.FileName, fileStream, mimeType);
                var fileEmailModel= await Service.SaveFileReferenceToDatabase(fileName, fileModel,Sender);
                if (fileEmailModel != null) 
                {
                    var confirmationLink = Url.Action("CheckDownload", "FileManager", new { id = fileEmailModel.FileId }, Request.Scheme);
                    var user = await UserAccountService.GetUserEmailById(fileModel.ReceiverId);
                    Sender.SendFileEmail(user.Email, "You have received a new file", confirmationLink,fileEmailModel.Code, user.UserName);
                }
                return RedirectToAction("Index", "Home");
            }
            return View(fileModel);
        }
    }
}