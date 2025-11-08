using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DATA;

[Table(nameof(Order))]
public class Order : EntityBase
{
    public required string name { get; set; }
    public decimal amount { get; set; }

    public Guid specification_id { get; set; }

    [ForeignKey(nameof(specification_id))]
    public Specification? specification { get; set; }

    public Guid account_id { get; set; }

    [ForeignKey(nameof(account_id))]
    public Account? account { get; set; }
    public int status { get; set; }

}
