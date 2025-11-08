using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DATA;

[Table(nameof(Invoice))]
public class Invoice : EntityBase
{
    public Guid order_id { get; set; }
    public Guid account_id { get; set; }
    public required string order_name { get; set; }
    public decimal amount { get; set; }
    public int status { get; set; }
}
