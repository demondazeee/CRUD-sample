namespace test_crud.Services
{
    public class PageMetadata
    {
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public PageMetadata(int totalItemCount, int pageSize, int currentPage)
        {
            this.CurrentPage = currentPage;
            this.TotalItemCount = totalItemCount;
            this.PageSize = pageSize;
            this.TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}
