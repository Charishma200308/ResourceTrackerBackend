namespace ResourceTrackerBackend.Models
{
    public class PagedEmployeeRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDir { get; set; }
        public string? SearchText { get; set; } // if used
    }
}
