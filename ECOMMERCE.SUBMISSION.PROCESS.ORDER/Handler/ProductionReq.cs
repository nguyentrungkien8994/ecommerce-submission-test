using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.PROCESS.ORDER
{
    public class ProductionReq
    {
        public Guid order_id { get; set; }
        public Guid invoice_id { get; set; }
    }
}
