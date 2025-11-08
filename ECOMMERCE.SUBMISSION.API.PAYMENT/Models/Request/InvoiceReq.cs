namespace ECOMMERCE.SUBMISSION.API.PAYMENT;

public class InvoiceReq
{
    public Guid order_id { get; set; }
    public Guid account_id { get; set; }
    public required string order_name { get; set; }
    public decimal amount { get; set; }
    public int status { get; set; }
}
