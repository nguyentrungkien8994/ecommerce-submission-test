namespace ECOMMERCE.SUBMISSION.API.ORDER;

public class OrderReq
{
    public required string name { get; set; }
    public decimal amount { get; set; }
    public Guid specification_id { get; set; }
}
