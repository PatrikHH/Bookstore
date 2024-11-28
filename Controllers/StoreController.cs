using Bookstore.DTO;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Bookstore.Controllers
{
    [Authorize]
    public class StoreController : BaseController
    {
        private StoreService _storeService;
        public StoreController(BaseService baseService,  StoreService storeService) : base (baseService)
        {
            this._storeService = storeService;     
        }
        public async Task<IActionResult> Index(string store)
        {
            if (store == null)
                store = TempData.Peek("Store") as string;
            else
                TempData["Store"] = store;
            var allBooks = await _storeService.GetAllBooksInStoreAsync(store);
            
            return View(allBooks);
        }
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create(BookInStoreDTO newBook)
        //{
        //    await _service.CreateAsync(newBook);
        //    return RedirectToAction("Index");
        //}
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var bookToEdit = await _service.GetByIdAsync(id);
        //    if (bookToEdit == null)
        //        return View("NotFound");
        //    return View(bookToEdit);
        //}
        [HttpPost]
        public async Task<IActionResult> Edit(BookInStoreDTO editBook)
        {
            await _storeService.EditAsync(editBook);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddBookSectionAndPrice(int id)
        {
            var bookToEdit = await _storeService.GetByIdAsync(id);
            var booksSectionsDropdownsData = await _storeService.GetNewBooksSectionDropdownsValues();
            ViewBag.BooksSections = new SelectList(booksSectionsDropdownsData.BooksSections, "Id", "BookSection");
            if (bookToEdit == null)
                return View("NotFound");
            return View(bookToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> AddBookSectionAndPrice(BookInStoreDTO editBook)
        {
            await _storeService.AddBookSectionAndPriceAsync(editBook);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Sell(int id)
        {
            var bookToSell = await _storeService.GetByIdAsync(id);
            if (bookToSell == null)
                return View("NotFound");
            await _storeService.SellAsync(bookToSell);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var bookToDelete = await _storeService.GetByIdAsync(id);
            if (bookToDelete == null)
                return View("NotFound");
            await _storeService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddBooksToStore()
        {
            string from = TempData["From"] as string;
            string to = TempData["To"] as string;
            int containerId = (int)TempData["ContainerId"];

            await _storeService.AddBooksFromTheWayToStoreAsync(from, to, containerId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SendBookToWarehouse(int id)
        {
            var sendBook = await _storeService.GetByIdAsync(id);
            if (sendBook == null)
                return View("NotFound");
            return View(sendBook);
        }
        [HttpPost]
        public async Task<IActionResult> SendBookToWarehouse(BookInStoreDTO sendBook)
        {
            await _storeService.EditBookInStoreQuantity(sendBook);
            TempData["From"] = TempData.Peek("Store") as string;
            TempData["To"] = "Warehouse";
            return RedirectToAction("SendBookOnTheWay", "BookOnTheWay", sendBook);
        }
        public IActionResult BookOnTheWay()
        {
            return RedirectToAction("Index", "BookOnTheWay");
        }
    }
}
