namespace ResourceTrackerBackend.Models
{
    public class BulkUpdateRequest
    {
        public List<int> EmployeeIds { get; set; } = new List<int>();
        public string? dsgntion { get; set; }
        public string? reporting { get; set; }
        public string? billable { get; set; }
        public string? skills { get; set; }
        public string? projalloc { get; set; }
        public string? location { get; set; }
        public string? doj { get; set; }
    }


}
