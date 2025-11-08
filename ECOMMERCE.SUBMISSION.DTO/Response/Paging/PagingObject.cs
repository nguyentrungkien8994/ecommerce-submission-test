namespace ECOMMERCE.SUBMISSION.DTO;

public class PagingObject<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}
