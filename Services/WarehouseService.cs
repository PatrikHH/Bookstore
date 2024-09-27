using Bookstore.DTO;
using Bookstore.Migrations;
using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class WarehouseService
    {
        private ApplicationDbContext _dbContext;
        public WarehouseService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        internal async Task<List<BookInWarehouseDTO>> GetAllAsync()
        {
            var allBooks = await _dbContext.BooksInWarehouse.ToListAsync();
            var allBooksDTO = new List<BookInWarehouseDTO>();
            foreach (var book in allBooks)
                allBooksDTO.Add(ModelToDTO(book));
            return allBooksDTO;
        }
        public async Task<List<BookOnTheWayDTO>> GetAllBooksOnTheWayAsync()
        {
            var allBooks = await _dbContext.BooksOnTheWay.ToListAsync();
            var allBooksDTO = new List<BookOnTheWayDTO>();
            foreach (var book in allBooks)
                allBooksDTO.Add(ModelToDTO(book));
            return allBooksDTO;
        }
        internal async Task<BookInWarehouseDTO> GetByIdAsync(int id)
        {
            var book = await _dbContext.BooksInWarehouse.FirstOrDefaultAsync(x => x.Id == id);
            return ModelToDTO(book);
        }
        internal async Task AddBooksFromTheWayToWarehouseAsync(string from, string to, int containerId)
        {
            var allBooksOnTheWay = await GetAllBooksOnTheWayAsync();
            var allBooksInWarehouse = await GetAllAsync();
            bool isBookFound = false;

            foreach (var bookOnTheWay in allBooksOnTheWay)
            {
                if (bookOnTheWay.From == from && bookOnTheWay.To == to && bookOnTheWay.ContainerId == containerId && bookOnTheWay.IsSent)
                {
                    foreach (var bookInWarehouse in allBooksInWarehouse)
                        if (bookInWarehouse.ISBN == bookOnTheWay.ISBN)
                        {
                            isBookFound = true;
                            await EditBookInWarehouseQuantityAfterAdd(bookOnTheWay, bookInWarehouse);
                            break;
                        }
                    if (!isBookFound)
                        await AddBookToStoreAsync(bookOnTheWay);                     
                    else
                        isBookFound = false;
                    await DeleteBookOnTheWay(bookOnTheWay);
                }
            }
        }
        internal async Task EditBookInWarehouseQuantityAfterAdd(BookOnTheWayDTO bookOnTheWayDTO, BookInWarehouseDTO bookInWarehouseDTO)
        {
            bookInWarehouseDTO.Quantity += bookOnTheWayDTO.Quantity;
            _dbContext.ChangeTracker.Clear();  //resi problem s trackovanim
            _dbContext.Update(DTOToModel(bookInWarehouseDTO));
           
            await _dbContext.SaveChangesAsync();
        }
        internal async Task AddBookToStoreAsync(BookOnTheWayDTO bookOnTheWayDTO)
        {
            _dbContext.ChangeTracker.Clear();  
            await _dbContext.BooksInWarehouse.AddAsync(DTOToModel(DTOToDTO(bookOnTheWayDTO)));
            await _dbContext.SaveChangesAsync();
        }
        private async Task DeleteBookOnTheWay(BookOnTheWayDTO bookOnTheWayDTO)
        {

            _dbContext.BooksOnTheWay.Remove(DTOToModel(bookOnTheWayDTO));

            await _dbContext.SaveChangesAsync();
        }
        internal async Task CreateAsync(BookInWarehouseDTO newBook)
        {
            await _dbContext.BooksInWarehouse.AddAsync(DTOToModel(newBook));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task EditBookInWarehouseQuantity(BookInWarehouseDTO sendBook)
        {
            sendBook.Quantity -= sendBook.SelectQuantity;
            if (sendBook.Quantity == 0)
                await DeleteAsync(sendBook.Id);
            else
                await EditAsync(sendBook);
            await _dbContext.SaveChangesAsync();
        }
        internal async Task EditAsync(BookInWarehouseDTO editBook)
        {
            _dbContext.Update(DTOToModel(editBook));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task DeleteAsync(int id)
        {
            var bookToDelete = await _dbContext.BooksInWarehouse.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.BooksInWarehouse.Remove(bookToDelete);
            await _dbContext.SaveChangesAsync();
        }
        private static BookInWarehouseDTO ModelToDTO(BookInWarehouse book)
        {
            return new BookInWarehouseDTO()
            {
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
            };
        }
        private static BookInWarehouse DTOToModel(BookInWarehouseDTO bookDTO)
        {
            return new BookInWarehouse()
            {
                Id = bookDTO.Id,
                Name = bookDTO.Name,
                Author = bookDTO.Author,
                ISBN = bookDTO.ISBN,
                Quantity = bookDTO.Quantity,
            };
        }
        private static BookOnTheWayDTO ModelToDTO(BookOnTheWay book)
        {
            return new BookOnTheWayDTO()
            {
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
                From = book.From,
                To = book.To,
                IsSent = book.IsSent,
                ContainerId = book.ContainerId
            };
        }
        private static BookOnTheWay DTOToModel(BookOnTheWayDTO bookDTO)
        {
            return new BookOnTheWay()
            {
                Id = bookDTO.Id,
                Name = bookDTO.Name,
                Author = bookDTO.Author,
                ISBN = bookDTO.ISBN,
                Quantity = bookDTO.Quantity,
                From = bookDTO.From,
                To = bookDTO.To,
            };
        }
        private static BookInWarehouseDTO DTOToDTO(BookOnTheWayDTO book)
        {
            return new BookInWarehouseDTO()
            {
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
            };
        }

    }
}
