using Bookstore.DTO;
using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class StoreManagementService
    {
        private ApplicationDbContext _dbContext;
        public StoreManagementService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        internal async Task<List<StoreManagementDTO>> GetAllAsync()
        {
            var allStores = await _dbContext.StoresManagement.ToListAsync();
            var allStoresDTO = new List<StoreManagementDTO>();
            foreach (var store in allStores)
                allStoresDTO.Add(ModelToDTO(store));
            return allStoresDTO;
        }
        internal async Task<StoreManagementDTO> GetByIdAsync(int id)
        {
            var store = await _dbContext.StoresManagement.FirstOrDefaultAsync(x => x.Id == id);
            return ModelToDTO(store);
        }
        internal async Task CreateAsync(StoreManagementDTO newStore)
        {
            await _dbContext.StoresManagement.AddAsync(DTOToModel(newStore));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task EditStoreNameAsync(StoreManagementDTO editStore, string originalName)
        {
            await EditBooksPlacementAsync(originalName, editStore.Name);
            _dbContext.Update(DTOToModel(editStore));
            await _dbContext.SaveChangesAsync();
        }
        internal async Task EditBooksPlacementAsync(string originalName, string newName)
        {
            var booksToRename = await _dbContext.BooksInStore.Where(x => x.StorePlacement == originalName).ToListAsync();
            foreach (var book in booksToRename)
            {
                book.StorePlacement = newName;
            }
            _dbContext.UpdateRange(booksToRename);
            await _dbContext.SaveChangesAsync();
        }
        internal async Task DeleteAsync(int id)
        {
            var storeToDelete = await _dbContext.StoresManagement.FirstOrDefaultAsync(x => x.Id == id) ?? throw new InvalidOperationException($"Store with ID {id} not found.");
            var booksPlacementEdit = await _dbContext.BooksOnTheWay.Where(x => x.To == storeToDelete.Name).ToListAsync();
                
            foreach (var book in booksPlacementEdit)
            {
                book.From = storeToDelete.Name;
                book.To = "Warehouse";
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.StoresManagement.Remove(storeToDelete);
                _dbContext.UpdateRange(booksPlacementEdit);
                await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }
        internal async Task<bool> IsBooksInStore(string store)
        {
            var IsBooksInStore = await _dbContext.BooksInStore.FirstOrDefaultAsync(x => x.StorePlacement == store);
            return (IsBooksInStore != null);
        }
        private StoreManagementDTO ModelToDTO(StoreManagement store)
        {
            return new StoreManagementDTO()
            {
                Id = store.Id,
                Name = store.Name,
                Street = store.Street,
                City = store.City,
                Email = store.Email,
                Phone = store.Phone
            };
        }
        private StoreManagement DTOToModel(StoreManagementDTO storeDTO)
        {
            return new StoreManagement()
            {
                Id = storeDTO.Id,
                Name = storeDTO.Name,
                Street = storeDTO.Street,
                City = storeDTO.City,
                Email = storeDTO.Email,
                Phone = storeDTO.Phone
            };
        }
    }
}
