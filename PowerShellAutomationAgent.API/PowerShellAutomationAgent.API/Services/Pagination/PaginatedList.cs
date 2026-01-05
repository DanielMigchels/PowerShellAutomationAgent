namespace PowerShellAutomationAgent.API.Services.Pagination;

public class PaginatedList<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public List<T> Data { get; set; } = [];
}
