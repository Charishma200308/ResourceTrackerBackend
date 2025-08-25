namespace ResourceTrackerBackend.Models
{
    public class Filter
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }


    public class PagedEmployeeRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDir { get; set; }
        public string? SearchText { get; set; } // if used
        public List<Filter>? Filters { get; set; }
    }
}
