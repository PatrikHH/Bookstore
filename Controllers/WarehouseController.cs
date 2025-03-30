using Bookstore.DTO;
using Bookstore.Migrations;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
   [Authorize]
    public class WarehouseController : BaseController
    {
        private WarehouseService _warehouseService;
        public WarehouseController(BaseService baseService, WarehouseService warehouseService) : base(baseService)
        {
            this._warehouseService = warehouseService;
        }
        public async Task<IActionResult> Index()
        {
            var allBooks = await _warehouseService.GetAllAsync();
            return View(allBooks);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookInWarehouseDTO newBook)
        {
            await _warehouseService.CreateAsync(newBook);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var bookToEdit = await _warehouseService.GetByIdAsync(id);
            if (bookToEdit == null)
                return View("NotFound");
            return View(bookToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BookInWarehouseDTO editBook)
        {
            await _warehouseService.EditAsync(editBook);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bookToDelete = await _warehouseService.GetByIdAsync(id);
            if (bookToDelete == null)
                return View("NotFound");
            await _warehouseService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddBooksToWarehouse()
        {
            string? from = TempData["From"] as string;
            string? to = TempData["To"] as string;
            int containerId = (int)TempData["containerId"]!;
            await _warehouseService.AddBooksFromTheWayToWarehouseAsync(from, to, containerId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SendBookToStore(int id)
        {
            var sendBook = await _warehouseService.GetByIdAsync(id);
            if (sendBook == null)
                return View("NotFound");
            return View(sendBook);
        }
        [HttpPost]
        public async Task<IActionResult> SendBookToStore(BookInWarehouseDTO sendBook, string store)
        {
            await _warehouseService.EditBookInWarehouseQuantity(sendBook);
            TempData["From"] = "Warehouse";
            TempData["To"] = store;
            return RedirectToAction("SendBookOnTheWay", "BookOnTheWay", sendBook);
        }
        public IActionResult BooksToStore()
        {
            return RedirectToAction("Index", "BookOnTheWay");
        }
    }
}
