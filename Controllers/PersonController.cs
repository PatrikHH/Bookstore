using Bookstore.DTO;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    [Authorize]
    public class PersonController : BaseController
    {
        private PersonService _personService;
        public PersonController(BaseService baseService, PersonService personService) : base(baseService) 
        {
            this._personService = personService;
        }
        public async Task<IActionResult> Index()
        {
            var allPersons = await _personService.GetAllAsync();
            return View(allPersons);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PersonDTO newPerson)
        {
            await _personService.CreateAync(newPerson);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var personToEdit = await _personService.GetByIdAsync(id);
            if (personToEdit == null)
                return View("NotFound");
            return View(personToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PersonDTO editPerson)
        {
            await _personService.EditAsync(editPerson);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var personToDelete = await _personService.GetByIdAsync(id);
            if (personToDelete == null)
                return View("NotFound");
            await _personService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
