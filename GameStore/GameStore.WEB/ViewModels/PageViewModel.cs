namespace GameStore.Web.ViewModels
{
    public class PageViewModel
    {
        public PageViewModel(int pageNumber, int totalPages)
        {
            PageNumber = pageNumber;
            TotalPages = totalPages;
        }

        public int PageNumber { get; private set; }

        public int TotalPages { get; private set; }
    }
}
