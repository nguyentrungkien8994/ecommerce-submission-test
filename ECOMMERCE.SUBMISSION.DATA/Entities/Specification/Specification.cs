using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DATA;

[Table(nameof(Specification))]
public class Specification : EntityBase
{
    public required string name { get; set; }
    public required string instructions { get; set; }
    public Guid account_id { get; set; }

    [ForeignKey(nameof(account_id))]
    public Account account { get; set; }
    public ICollection<Order>? orders { get; set; }
}
