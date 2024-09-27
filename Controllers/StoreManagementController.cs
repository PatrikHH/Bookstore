using Bookstore.DTO;
using Bookstore.Services;
using Bookstore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    [Authorize]
    public class StoreManagementController : BaseController
    {
        private StoreManagementService _storeManagementService;
        public StoreManagementController(BaseService baseService, StoreManagementService storeManagementService) : base(baseService)
        {
            this._storeManagementService = storeManagementService;
        }
        public async Task<IActionResult> Index()
        {
            var allStores = await _storeManagementService.GetAllAsync();
            return View(allStores);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(StoreManagementDTO newStore)
        {
            await _storeManagementService.CreateAsync(newStore);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var storeToEdit = await _storeManagementService.GetByIdAsync(id);
            if (storeToEdit == null)
                return View("NotFound");
            return View(storeToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(StoreManagementDTO editStore, string originalName)
        {
            await _storeManagementService.EditStoreNameAsync(editStore, originalName);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var storeToDelete = await _storeManagementService.GetByIdAsync(id);
            if (storeToDelete == null)
                return Json(new { success = false, message = "Store was not found." });
            else if (await _storeManagementService.IsBooksInStore(storeToDelete.Name))
                return Json(new { success = false, message = "There are books in the store, send them to the warehouse first." });
            else { 
                await _storeManagementService.DeleteAsync(id);
                return Json(new { success = true, message = storeToDelete.Name + " was successfully deleted." });
            }
        }
    }
}
