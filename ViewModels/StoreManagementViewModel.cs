using Bookstore.Models;

namespace Bookstore.ViewModels
{
    public class StoreManagementViewModel
    {
        public IEnumerable<StoreManagement> StoresManagement { get; set; }
        public StoreManagementViewModel()
        {
            StoresManagement = new List<StoreManagement>();
        }
    }
}
