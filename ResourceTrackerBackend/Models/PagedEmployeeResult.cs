namespace ResourceTrackerBackend.Models
{
    public class PagedEmployeeResult
    {
        public int TotalCount { get; set; }
        public List<Details> Employees { get; set; }

    }
}
