namespace SchoolManagementApi.DTOs
{
  public class PageDataResponse
  {
    public string? Status { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public PaginationMetaData? Pagination {get; set; }
  }
}