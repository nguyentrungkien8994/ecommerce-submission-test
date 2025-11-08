using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.PROCESS.INVOICE
{
    public class OrderPaymentReq
    {
        public Guid order_id { get; set; }
        public Guid invoice_id { get; set; }
        public bool payment_success { get; set; }
    }
}
