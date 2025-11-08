using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.HELPER;

public enum OrderStatus
{
    Ready_Checkout = 0,
    Checking_Out = 10,
    Payment_Failed = 20,
    Ready_In_Production = 30,
    In_Production = 40
}
public enum InvoiceStatus
{
    /// <summary>
    /// Checking out
    /// </summary>
    Temporary = 0,
    /// <summary>
    /// Payment failed
    /// </summary>
    Cancel = 10,
    /// <summary>
    /// Paid
    /// </summary>
    Paid = 20
}