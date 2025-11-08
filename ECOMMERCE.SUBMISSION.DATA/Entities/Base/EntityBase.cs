using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DATA;

public class EntityBase : IEntityBase
{
    public Guid id { get; set; }
    public long created_at { get; set; }
    public long updated_at { get; set; }
    public required string created_by { get; set; }
    public required string updated_by { get; set; }
}
