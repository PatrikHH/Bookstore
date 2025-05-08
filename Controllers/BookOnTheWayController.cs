using Bookstore.DTO;
using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    [Authorize]
    public class BookOnTheWayController : BaseController
    {
        private BookOnTheWayService _booksOnTheWayService;
        public BookOnTheWayController(BaseService baseService, BookOnTheWayService booksOnTheWayService) : base(baseService)
        {
            _booksOnTheWayService = booksOnTheWayService;
        }
        public async Task<IActionResult> Index()
        {
            var allBooks = await _booksOnTheWayService.GetAllAsync();
            var allStores = ViewData["AllStores"] as List<StoreManagement>;
            return View(allBooks);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var bookToEditQuantity = await _booksOnTheWayService.GetByIdAsync(id);
            if (bookToEditQuantity == null)
                return View("NotFound");
            return View(bookToEditQuantity);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string from, int ISBN, int quantity, int id)
        {
            await _booksOnTheWayService.EditBookQuantityAsync(from, ISBN, quantity, id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string from, int ISBN, int id)
        {
            var bookToDelete = await _booksOnTheWayService.GetByIdAsync(id);

            if (bookToDelete == null)
                return View("NotFound");
            await _booksOnTheWayService.EditBookQuantityAsync(from, ISBN, 0, id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LockBooks(int containerID)
        {

            await _booksOnTheWayService.LockBooksAsync(containerID);
            return RedirectToAction("Index");
        }       
        public async Task<IActionResult> SendBookOnTheWay(BookInWarehouseDTO sendBookDTO)
        {
            string? from = TempData["From"] as string;
            string? to = TempData["To"] as string;            

            await _booksOnTheWayService.SendBookOnTheWayAsync(sendBookDTO, from, to);
            if (from.Equals("Warehouse"))
                return RedirectToAction("Index", "Warehouse");
            return RedirectToAction("Index", "Store");
        }
        public async Task<IActionResult> SendBookToWarehouse(BookInStoreDTO sendBookDTO)
        {
            string? from = TempData["From"] as string;
            string? to = TempData["To"] as string;

            await _booksOnTheWayService.SendBookOnTheWayAsync(sendBookDTO, from, to);
            return RedirectToAction("Index", "Store");
        }
        public async Task<IActionResult> AddBooksToStore(string from, string to, int id)
        {
            TempData["From"] = from;
            TempData["To"] = to;
            TempData["ContainerId"] = id;
            TempData["Store"] = to;

            return RedirectToAction("AddBooksToStore", "Store");
        }
        public async Task<IActionResult> AddBooksToWarehouse(string from, string to, int id)
        {
            TempData["From"] = from;
            TempData["To"] = to;
            TempData["ContainerId"] = id;

            return RedirectToAction("AddBooksToWarehouse", "Warehouse");
        }
    }
}
