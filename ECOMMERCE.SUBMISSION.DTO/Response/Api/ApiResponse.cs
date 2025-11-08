using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.DTO;

public class ApiResponse<T>
{
    public bool success { get; set; }
    public int status_code { get; set; }
    public string? message { get; set; }
    public T? data { get; set; }
    public object? errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            success = true,
            status_code = 200,
            message = message,
            data = data,
            errors = null
        };
    }

    public static ApiResponse<T> FailResponse(string message, object errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            success = false,
            status_code = statusCode,
            message = message,
            data = default,
            errors = errors
        };
    }
}
