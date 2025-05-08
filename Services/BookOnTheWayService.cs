using Bookstore.DTO;
using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services;

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
    private async Task<object?> GetByISBNAsync(string from, int ISBN)
    {
        dynamic? book = from.Equals("Warehouse")
            ? await _dbContext.BooksInWarehouse.FirstOrDefaultAsync(x => x.ISBN == ISBN)
            : await _dbContext.BooksInStore.FirstOrDefaultAsync(x => x.ISBN == ISBN && x.StorePlacement == from);

        return book == null ? null : ModelToDTO(book);
    }

    private async Task<object?> GetByISBNEntityAsync(string from, int ISBN)
    {
        return from.Equals("Warehouse")
            ? await _dbContext.BooksInWarehouse.FirstOrDefaultAsync(x => x.ISBN == ISBN)
            : await _dbContext.BooksInStore.FirstOrDefaultAsync(x => x.ISBN == ISBN && x.StorePlacement == from);
    }
    internal async Task EditBookQuantityAsync(string from, int ISBN, int quantity, int id)
    {
        if (quantity < 0)
        {
            return;
        }

        var bookToEdit = await GetByIdAsync(id);
        int quantityDiff = bookToEdit.Quantity - quantity;

        if (quantityDiff == 0)
        {
            return;
        }

        var originBook = await GetOrCreateOriginBookAsync(from, ISBN, bookToEdit, quantityDiff);

        if (originBook == null)
        {
            return;
        }

        AdjustQuantities(bookToEdit, originBook, quantityDiff);

        await SaveOrEditBookToEdit(bookToEdit);
        await SaveOrDeleteOriginBookAsync(originBook, from, ISBN);
    }

    private async Task<dynamic?> GetOrCreateOriginBookAsync(string from, int ISBN, dynamic bookToEdit, int diffQuantity)
    {
        var originBook = await GetByISBNAsync(from, ISBN);

        if (originBook == null && diffQuantity > 0)
        {
            originBook = await CreateAsync(bookToEdit, from);
        }

        return originBook;
    }

    private static void AdjustQuantities(dynamic bookToEdit, dynamic originBook, int quantityDiff)
    {
        if (quantityDiff < 0 && Math.Abs(quantityDiff) > originBook.Quantity)
        {
            quantityDiff = -originBook.Quantity;
        }

        originBook.Quantity += quantityDiff;
        bookToEdit.Quantity -= quantityDiff;
    }

    private async Task SaveOrEditBookToEdit(dynamic book)
    {
        if (book.Quantity == 0)
        {
            DeleteAsync(book);
        }
        else
        {
            _dbContext.Update(DTOToModel(book));
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task SaveOrDeleteOriginBookAsync(dynamic bookOrigin, string from, int ISBN)
    {
        if (bookOrigin.Quantity == 0)
        {
            DeleteAsync(bookOrigin);
        }
        else
        {
            int saveQuantity = bookOrigin.Quantity;
            dynamic? freshBook = await GetByISBNEntityAsync(from, ISBN);

            if (freshBook != null)
            {
                _dbContext.ChangeTracker.Clear();
                freshBook.Quantity = saveQuantity;
                _dbContext.Update(freshBook);
            }
        }

        await _dbContext.SaveChangesAsync();           
    }
    private async Task<object> CreateAsync(BookOnTheWayDTO book, string from)
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
    private void DeleteAsync(BookOnTheWayDTO book)
    {
        _dbContext.BooksOnTheWay.Remove(DTOToModel(book));
    }
    private void DeleteAsync(BookInWarehouseDTO book)
    {
        _dbContext.BooksInWarehouse.Remove(DTOToModel(book));
    }
    private void DeleteAsync(BookInStoreDTO book)
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
    private async Task EditBookOnTheWayQuantityAsync(BookOnTheWayDTO sendBookConvertedDTO, BookOnTheWayDTO bookOnTheWayDTO)
    {
        bookOnTheWayDTO.Quantity += sendBookConvertedDTO.Quantity;
        _dbContext.Update(DTOToModel(bookOnTheWayDTO));
        await _dbContext.SaveChangesAsync();
    }
    private async Task AddBookOnTheWayAsync(BookOnTheWayDTO sendBookOnItsWay)
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
