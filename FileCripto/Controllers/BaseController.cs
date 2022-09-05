using Microsoft.AspNetCore.Mvc;
using BusinessLogic;
using FileCrypto;

namespace FileCrypto.Controllers
{
    public class BaseController : Controller
    {
        protected readonly CurrentUserDto CurrentUser;

        public BaseController(ControllerDependencies dependencies)
            : base()
        {
            CurrentUser = dependencies.CurrentUser;
        }
    }
}
