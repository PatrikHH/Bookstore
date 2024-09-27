using Bookstore.Models;
using Bookstore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class BaseService
    {
        private ApplicationDbContext _dbContext;
        public BaseService(ApplicationDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task<StoreManagementViewModel> GetAllStores()
        {
            var allStores = new StoreManagementViewModel()
            {
                StoresManagement = await _dbContext.StoresManagement.OrderBy(x => x.Name).ToListAsync()
            };
            return allStores;
        }
    }
}
