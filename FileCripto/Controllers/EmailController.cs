using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileCrypto.Controllers
{
    public class EmailController : BaseController
    {
        private readonly EmailService Service;
        public EmailController(ControllerDependencies dependencies, EmailService service)
           : base(dependencies)
        {
            Service = service;
        }

        public async Task<ActionResult> ConfirmEmail(string token, string email)
        {

            var result = await Service.ConfirmEmailAsync(email, token);
            return View(result == true ? "ConfirmEmail" : "Error_InternalServerError");
        }
        public async Task<ActionResult> ConfirmEmailPassword(string token, string email)
        {
            var result = await Service.ConfirmEmailPasswordAsync(email, token);
            return result != null ? RedirectToAction("ResetPassword", "UserAccount", new { id = result.UserId }) : View("Error_InternalServerError");
        }
    }
}
