using Bookstore.DTO;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    [Authorize]
    public class BooksSectionController : BaseController
    {
        private BooksSectionService _booksSectionService;
        public BooksSectionController(BaseService baseService, BooksSectionService booksSectionService) : base(baseService)
        {
            this._booksSectionService = booksSectionService;
        }
        public async Task<IActionResult> Index()
        {
            var allSections = await _booksSectionService.GetAllAsync();
            return View(allSections);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BooksSectionDTO newSection)
        {
            await _booksSectionService.CreateAsync(newSection);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var sectionToEdit = await _booksSectionService.GetByIdAsync(id);
            if (sectionToEdit == null)
                return View("NotFound");
            return View(sectionToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BooksSectionDTO editSection)
        {
            await _booksSectionService.EditAsync(editSection);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var sectionToDelete = await _booksSectionService.GetByIdAsync(id);
            if (sectionToDelete == null)
                return View("NotFound");
            await _booksSectionService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
