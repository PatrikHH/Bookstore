using Bookstore.DTO;
using Bookstore.Models;
using Bookstore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class StoreService
    {
        private ApplicationDbContext _dbContext;
        public StoreService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task<IEnumerable<BookInStoreDTO>> GetAllBooksInStoreAsync(string to)
        {
            var allBooks = await _dbContext.BooksInStore.ToListAsync();
            var allBooksDTO = new List<BookInStoreDTO>();
            foreach (var book in allBooks)
                if (book.StorePlacement == to)
                    allBooksDTO.Add(ModelToDTO(book));
            return allBooksDTO;
        }
        public async Task<IEnumerable<BookOnTheWayDTO>> GetAllBooksOnTheWayAsync()
        {
            var allBooks = await _dbContext.BooksOnTheWay.ToListAsync();
            var allBooksDTO = new List<BookOnTheWayDTO>();
            foreach (var book in allBooks)
                allBooksDTO.Add(ModelToDTO(book));
            return allBooksDTO;
        }
        public async Task<IEnumerable<BookInWarehouseDTO>> GetAllBooksInWarehouseAsync()
        {
            var allBooks = await _dbContext.BooksInWarehouse.ToListAsync();
            var allBooksDTO = new List<BookInWarehouseDTO>();
            foreach (var book in allBooks)
                allBooksDTO.Add(ModelToDTO(book));
            return allBooksDTO;
        }
        public async Task<BookInStoreDTO> GetByIdAsync(int id)
        {
            var book = await _dbContext.BooksInStore.FirstOrDefaultAsync(x => x.Id == id);
            return ModelToDTO(book);
        }
        internal async Task AddBooksFromTheWayToStoreAsync(string from, string to, int containerId)
        {
            var allBooksInStore = await GetAllBooksInStoreAsync(to);
            var allBooksOnTheWay = await GetAllBooksOnTheWayAsync();
            bool isBookFound = false;

            foreach (var bookOnTheWay in allBooksOnTheWay)
            {
                if (bookOnTheWay.From == from && bookOnTheWay.To == to && bookOnTheWay.ContainerId == containerId && bookOnTheWay.IsSent)
                { 
                    foreach (var bookInStore in allBooksInStore)
                        if (bookInStore.ISBN == bookOnTheWay.ISBN)
                        {
                            isBookFound = true;
                            await EditBookInStoreQuantityAfterAdd(bookOnTheWay, bookInStore);
                            break;
                        }
                    if (!isBookFound)
                        await AddBookToStoreAsync(bookOnTheWay, to);
                    else
                        isBookFound = false;
                    await DeleteBookOnTheWay(bookOnTheWay);
                }
            }
        }
        internal async Task EditBookInStoreQuantityAfterAdd(BookOnTheWayDTO bookOnTheWay, BookInStoreDTO bookInStoreDTO)
        {
            bookInStoreDTO.Quantity += bookOnTheWay.Quantity;
            _dbContext.Update(DTOToModel(bookInStoreDTO));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task AddBookToStoreAsync(BookOnTheWayDTO bookOnTheWay, string to)
        {
            await _dbContext.BooksInStore.AddAsync(DTOToModel(DTOToDTO(bookOnTheWay, to)));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task DeleteBookOnTheWay(BookOnTheWayDTO bookOnTheWayDTO)
        {
            _dbContext.BooksOnTheWay.Remove(DTOToModel(bookOnTheWayDTO));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task RequestBooks(List<BookRequestDataDTO> books, string name)
        {
            int maxId = 
            await _dbContext.SaveChangesAsync();
        }
        internal async Task EditBookInStoreQuantity(BookInStoreDTO sendBook)
        {
            sendBook.Quantity -= sendBook.SelectQuantity;
            if (sendBook.Quantity == 0)
                await DeleteAsync(sendBook.Id);
            else
                await EditAsync(sendBook);
        }
        internal async Task EditAsync(BookInStoreDTO editBook)
        {
            _dbContext.Update(DTOToModel(editBook));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task AddBookSectionAndPriceAsync(BookInStoreDTO editBook) //neobratne, musi to byt jako grade a pres viewmodel
        {
            editBook.BooksSection = (await _dbContext.BooksSections.FirstOrDefaultAsync(x => x.Id == editBook.BooksSectionId)).BookSection;  
            _dbContext.Update(DTOToModel(editBook));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task SellAsync(BookInStoreDTO bookToSell)
        {
            bookToSell.Quantity -= 1;
            if (bookToSell.Quantity == 0)
                await DeleteAsync(bookToSell.Id);
            else
                await EditAsync(bookToSell);
        }
        internal async Task DeleteAsync(int id)
        {
            var bookToSell = await _dbContext.BooksInStore.FirstOrDefaultAsync(x => x.Id == id);
            _dbContext.BooksInStore.Remove(bookToSell);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<BooksSectionDropdownsViewModel> GetNewBooksSectionDropdownsValues()
        {
            var booksSectionDropdownsData = new BooksSectionDropdownsViewModel()
            {
                BooksSections = await _dbContext.BooksSections.OrderBy(x => x.BookSection).ToListAsync()
            };
            return booksSectionDropdownsData;
        }
        private static BookInStoreDTO ModelToDTO(BookInStore book)
        {
            return new BookInStoreDTO()
            {
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
                StorePlacement = book.StorePlacement,
                BooksSection = book.BooksSection,
                Price = book.Price
            };
        }
        private static BookInStore DTOToModel(BookInStoreDTO bookDTO)
        {
            return new BookInStore()
            {
                Id= bookDTO.Id,
                Name = bookDTO.Name,
                Author = bookDTO.Author,
                ISBN = bookDTO.ISBN,
                Quantity = bookDTO.Quantity,
                StorePlacement = bookDTO.StorePlacement,
                BooksSection = bookDTO.BooksSection,
                Price = bookDTO.Price
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
        private static BookInStoreDTO DTOToDTO(BookOnTheWayDTO book, string to)
        {
            return new BookInStoreDTO()
            {
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
                StorePlacement = to
            };
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
    }
}
