using ECOMMERCE.SUBMISSION.HELPER;

namespace ECOMMERCE.SUBMISSION.API.ORDER;

public class OrderStatusReq
{
    public Guid order_id{ get; set; }
    public int status { get; set; }
}
