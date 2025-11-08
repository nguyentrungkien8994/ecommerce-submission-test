using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DATA;

[Table(nameof(Account))]
public class Account : EntityBase
{
    public required string email { get; set; }
    public required string password { get; set; }
    public ICollection<Order>? orders { get; set; }
    public ICollection<Specification>? specifications { get;set; }
}
