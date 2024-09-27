using Bookstore.DTO;
using Bookstore.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Bookstore.Services
{
    public class BookOnTheWayService
    {
        private ApplicationDbContext _dbContext;
        public BookOnTheWayService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        internal async Task<IEnumerable<BookOnTheWayDTO>> GetAllAsync()
        {
            var allBooks = await _dbContext.BooksOnTheWay.ToListAsync();
            var allBooksDTO = new List<BookOnTheWayDTO>();
            foreach (var book in allBooks)
                allBooksDTO.Add(ModelToDTO(book));
            return allBooksDTO;
        }
        internal async Task<BookOnTheWayDTO> GetByIdAsync(int id)
        {
            var book = await _dbContext.BooksOnTheWay.FirstOrDefaultAsync(x => x.Id == id);
            return ModelToDTO(book);
        }
        internal async Task<object?> GetByISBNAsync(string from, int ISBN)
        {
            dynamic? book = from.Equals("Warehouse") ? await _dbContext.BooksInWarehouse.FirstOrDefaultAsync(x => x.ISBN == ISBN) : await _dbContext.BooksInStore.FirstOrDefaultAsync(x => x.ISBN == ISBN && x.StorePlacement == from);

            return book == null ? null : ModelToDTO(book);
        }
        internal async Task EditAsync(string from, int ISBN, int quantity, int id)
        {
            var bookToEditQuantity = await GetByIdAsync(id);
            dynamic bookOrigin = await GetByISBNAsync(from, ISBN);
            int diff = bookToEditQuantity.Quantity - quantity;
            bool isNewBookOrigin = false;

            if (diff == 0 || quantity < 0)
                return;
                
            if (bookOrigin == null && diff > 0)
            {
                bookOrigin = await CreateAsync(bookToEditQuantity, from);
                isNewBookOrigin = true;
            }

            if (bookOrigin != null)
            { 
                if (diff < 0 && Math.Abs(diff) > bookOrigin.Quantity)
                    diff = -bookOrigin.Quantity;
                bookOrigin.Quantity += diff;
                bookToEditQuantity.Quantity -= diff;

                if (bookToEditQuantity.Quantity == 0)
                {
                    DeleteAsync(bookToEditQuantity);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                     _dbContext.Update(DTOToModel(bookToEditQuantity));
                     await _dbContext.SaveChangesAsync();
                }

                if (bookOrigin.Quantity == 0)
                {
                    DeleteAsync(bookOrigin);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    if (isNewBookOrigin)
                    {
                        int saveQuantity = bookOrigin.Quantity;
                        bookOrigin = await GetByISBNAsync(from, ISBN);
                        _dbContext.ChangeTracker.Clear();
                        bookOrigin.Quantity = saveQuantity;
                    }                  
                _dbContext.Update(DTOToModel(bookOrigin));
                await _dbContext.SaveChangesAsync();
                }
            }        
        }
        internal async Task<object> CreateAsync(BookOnTheWayDTO book, string from)
        {
            if (from.Equals("Warehouse"))
            {
                BookInWarehouseDTO bookInWarehouse = (BookInWarehouseDTO)DTOToDTO(book, from);
                await _dbContext.BooksInWarehouse.AddAsync(DTOToModel(bookInWarehouse));
                await _dbContext.SaveChangesAsync();
                return bookInWarehouse;
            }   
            else
            {
                BookInStoreDTO bookInStore = (BookInStoreDTO)DTOToDTO(book, from);
                await _dbContext.BooksInStore.AddAsync(DTOToModel(bookInStore));
                await _dbContext.SaveChangesAsync();
                return bookInStore;
            }
        }
        internal void DeleteAsync(BookOnTheWayDTO book)
        {
            _dbContext.BooksOnTheWay.Remove(DTOToModel(book));
        }
        internal void DeleteAsync(BookInWarehouseDTO book)
        {
            _dbContext.BooksInWarehouse.Remove(DTOToModel(book));
        }
        internal void DeleteAsync(BookInStoreDTO book)
        {
            _dbContext.BooksInStore.Remove(DTOToModel(book));
        }
        internal async Task SendBookOnTheWayAsync(object sendBookDTO, string from, string to)
        {
            var allBooksOnTheWayDTO = await GetAllAsync();
            var maxContainerId = allBooksOnTheWayDTO.Select(x => x.ContainerId).DefaultIfEmpty(0).Max();
            var sendBookConvertedDTO = ConvertBookToSend(sendBookDTO, from, to, maxContainerId);          

            foreach (var bookOnTheWayDTO in allBooksOnTheWayDTO)
                if (sendBookConvertedDTO.ISBN == bookOnTheWayDTO.ISBN &&
                    sendBookConvertedDTO.From == bookOnTheWayDTO.From &&
                    sendBookConvertedDTO.To == bookOnTheWayDTO.To &&
                    !bookOnTheWayDTO.IsSent)
                {
                    await EditBookOnTheWayQuantityAsync(sendBookConvertedDTO, bookOnTheWayDTO);
                    return;
                }
            await AddBookOnTheWayAsync(sendBookConvertedDTO);
        }
        internal async Task EditBookOnTheWayQuantityAsync(BookOnTheWayDTO sendBookConvertedDTO, BookOnTheWayDTO bookOnTheWayDTO)
        {
            bookOnTheWayDTO.Quantity += sendBookConvertedDTO.Quantity;
            _dbContext.Update(DTOToModel(bookOnTheWayDTO));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task AddBookOnTheWayAsync(BookOnTheWayDTO sendBookOnItsWay)
        {
            await _dbContext.BooksOnTheWay.AddAsync(DTOToModel(sendBookOnItsWay));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task LockBooksAsync(int containerID)
        {
            var allBooksOnTheWayDTO = await GetAllAsync();
            var maxContainerId = allBooksOnTheWayDTO.Select(x => x.ContainerId).Max();

            foreach (var bookOnTheWayDTO in allBooksOnTheWayDTO)
                if (!bookOnTheWayDTO.IsSent && bookOnTheWayDTO.ContainerId == containerID)
                {
                    bookOnTheWayDTO.ContainerId = maxContainerId;
                    bookOnTheWayDTO.IsSent = true;
                    _dbContext.Update(DTOToModel(bookOnTheWayDTO));
                    await _dbContext.SaveChangesAsync();
                }
        }
        private static object DTOToDTO(BookOnTheWayDTO book, string from)
        {
            if (from.Equals("Warehouse"))
                return new BookInWarehouseDTO()
                {
                    Name = book.Name,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Quantity = 0
                };
            else
                return new BookInStoreDTO()
                {
                    Name = book.Name,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Quantity = 0,
                    StorePlacement = from

                };
        }
        private static BookOnTheWayDTO ConvertBookToSend(dynamic book, string from, string to, int maxContainerId)
        {
            return new BookOnTheWayDTO()
            {
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.SelectQuantity,
                From = from,
                To = to,
                ContainerId = maxContainerId + 1 
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
                ContainerId = book.ContainerId,
                IsSent = book.IsSent,
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
                ContainerId = bookDTO.ContainerId,
                IsSent = bookDTO.IsSent,
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
        private static BookInStoreDTO ModelToDTO(BookInStore book)
        {
            return new BookInStoreDTO()
            {
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
                StorePlacement = book.StorePlacement
            };
        }
        private static BookInStore DTOToModel(BookInStoreDTO book)
        {
            return new BookInStore()
            {
                Id = book.Id,
                Name = book.Name,
                Author = book.Author,
                ISBN = book.ISBN,
                Quantity = book.Quantity,
                StorePlacement = book.StorePlacement
            };
        }
    }
}
