using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DTO;

public class InvoiceDTO
{
    public Guid order_id { get; set; }
    public Guid account_id { get; set; }
    public required string order_name { get; set; }
    public decimal amount { get; set; }
    public int status { get; set; }
}
