using Bookstore.Models;

namespace Bookstore.ViewModels
{
    public class BooksSectionDropdownsViewModel
    {
        public IEnumerable<BooksSection> BooksSections { get; set; }
        public BooksSectionDropdownsViewModel()
        {
            BooksSections = new List<BooksSection>();
        }
    }
}
