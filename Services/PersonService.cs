using Bookstore.DTO;
using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class PersonService
    {
        private ApplicationDbContext _dbContext;
        public PersonService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task<IEnumerable<PersonDTO>> GetAllAsync()
        {
            var allPersons = await _dbContext.Persons.ToListAsync();
            var allPersonsDTO = new List<PersonDTO>();
            foreach (var person in allPersons)
            {
                allPersonsDTO.Add(ModelToDTO(person));
            }
            return allPersonsDTO;
        }
        public async Task CreateAync(PersonDTO newPerson)
        {
            await _dbContext.Persons.AddAsync(DTOToModel(newPerson));
            await _dbContext.SaveChangesAsync();
        }
        public async Task<PersonDTO> GetByIdAsync(int id)
        {
            var person = await _dbContext.Persons.FirstOrDefaultAsync(x => x.Id == id);
            return ModelToDTO(person);
        }
        internal async Task EditAsync(PersonDTO editPerson)
        {
            _dbContext.Update(DTOToModel(editPerson));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task DeleteAsync(int id)
        {
            var personToDelete = await _dbContext.Persons.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.Persons.Remove(personToDelete);
            await _dbContext.SaveChangesAsync();
        }
        private static PersonDTO ModelToDTO(Person person)
        {
            return new PersonDTO()
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                UserName = person.UserName,
                Phone = person.Phone,
                Email = person.Email
            };
        }
        private static Person DTOToModel(PersonDTO personDTO)
        {
            return new Person()
            {
                Id = personDTO.Id,
                FirstName = personDTO.FirstName,
                LastName = personDTO.LastName,
                UserName = personDTO.UserName,
                Phone = personDTO.Phone,
                Email = personDTO.Email
            };
        }


    }
}
