namespace PixelPort.Server.Helpers
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => TotalCount > 0 && CurrentPage > 0;
        public bool HasNext => TotalCount > 0 && CurrentPage < TotalPages;
        public bool IsValidPage => TotalCount > 0 && CurrentPage >= 0 && CurrentPage <= TotalPages;
    }
}
