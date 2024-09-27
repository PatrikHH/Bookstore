using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bookstore.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected BaseService _service;
        public BaseController(BaseService service)
        {
            this._service = service;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)

        {
            if (_service != null)
            {   
                var allStores = await _service.GetAllStores();
                ViewData["AllStores"] = allStores.StoresManagement.ToList();
            }          
            await next();
        }
    }
}
//https://doc.nette.org/cs/dependency-injection/passing-dependencies