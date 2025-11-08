using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DTO;

public class RegisterDTO
{
    public required string email { get; set; }
    public required string password { get; set; }
}
