using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DATA;

public interface IEntityBase
{
    [Key()]
    Guid id { get; }
    string created_by { get; }
    string updated_by { get; }
    long created_at { get; }
    long updated_at { get; }
}
