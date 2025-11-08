using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DTO
{
    public class TokenDTO
    {
        public required string access_token { get; set; }
        public required string refresh_token { get; set; }
    }
}
