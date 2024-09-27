using Bookstore.DTO;
using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class BooksSectionService
    {
        private ApplicationDbContext _dbContext;
        public BooksSectionService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        internal async Task<IEnumerable<BooksSectionDTO>> GetAllAsync()
        {
            var allSections = await _dbContext.BooksSections.ToListAsync();
            var allSectionsDTO = new List<BooksSectionDTO>();
            foreach (var section in allSections)
            {
                allSectionsDTO.Add(ModelToDTO(section));
            }
            return allSectionsDTO;
        }
        internal async Task<BooksSectionDTO> GetByIdAsync(int id)
        {
            var section = await _dbContext.BooksSections.FirstOrDefaultAsync(x => x.Id == id);
            return ModelToDTO(section);
        }
        internal async Task CreateAsync(BooksSectionDTO newSection)
        {
            await _dbContext.BooksSections.AddAsync(DTOToModel(newSection));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task EditAsync(BooksSectionDTO editSection)
        {
            _dbContext.Update(DTOToModel(editSection));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task DeleteAsync(int id)
        {
            var sectionToDelete = await _dbContext.BooksSections.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.BooksSections.Remove(sectionToDelete);
            await _dbContext.SaveChangesAsync();
        }
        private static BooksSectionDTO ModelToDTO(BooksSection section)
        {
            return new BooksSectionDTO()
            {
                Id = section.Id,
                Section = section.BookSection
            };
        }
        private static BooksSection DTOToModel(BooksSectionDTO sectionDTO)
        {
            return new BooksSection()
            {
                Id = sectionDTO.Id,
                BookSection = sectionDTO.Section
            };
        }
    }
}
