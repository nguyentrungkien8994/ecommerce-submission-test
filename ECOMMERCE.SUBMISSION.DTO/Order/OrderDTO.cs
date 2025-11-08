using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DTO;

public class OrderDTO
{
    public required string name { get; set; }
    public decimal amount { get; set; }
    public Guid specification_id { get; set; }
}
