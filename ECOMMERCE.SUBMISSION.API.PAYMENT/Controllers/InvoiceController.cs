using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using ECOMMERCE.SUBMISSION.SERVICE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECOMMERCE.SUBMISSION.API.PAYMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        #region Declare/Contructor
        private readonly ILogger<InvoiceController> _logger;
        private readonly IServiceBase<Invoice> _serviceInvoice;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;
        private readonly IEventBus _eventBus;


        public InvoiceController(ILogger<InvoiceController> logger,
            IServiceBase<Invoice> serviceInvoice,
            IHttpContextAccessor accessor,
            IEventBus bus,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceInvoice = serviceInvoice;
            _configuration = configuration;
            _accessor = accessor;
            _eventBus = bus;
        }
        #endregion

        #region Private Function
        /// <summary>
        /// Validate access token
        /// </summary>
        /// <returns></returns>
        private (bool success, Guid accountId, string email, string error, int statusCode) ValidateAccessToken()
        {
            string token = JWTHelper.GetAccessTokenFromContext(_accessor);
            if (string.IsNullOrWhiteSpace(token))
                return (false, Guid.Empty, "", EcommerceConstants.ERROR_INVALID_TOKEN, StatusCodes.Status500InternalServerError);

            var parserToken = JWTHelper.ParserAccessToken(token);
            string? sub = parserToken?.Subject;
            string? email = parserToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? sub;
            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out Guid accountId) || string.IsNullOrWhiteSpace(email))
                return (false, Guid.Empty, "", EcommerceConstants.ERROR_INVALID_TOKEN, StatusCodes.Status500InternalServerError);

            return (true, accountId, email, "", StatusCodes.Status200OK);
        }
        #endregion

        #region EndPoint
        [Authorize]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                (bool success, Guid accountId, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                var Invoices = await _serviceInvoice.Search(x => x.account_id == accountId);
                return Ok(ApiResponse<List<Invoice>>.SuccessResponse(Invoices));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("{invoiceId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceById(string invoiceId)
        {
            try
            {
                (bool success, Guid accountId, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                if (string.IsNullOrWhiteSpace(invoiceId) || !Guid.TryParse(invoiceId, out Guid ivId))
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));

                var invoice = await _serviceInvoice.GetAsync(ivId);
                if (invoice == null || invoice.account_id != accountId)
                    return StatusCode(StatusCodes.Status404NotFound,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_NOT_FOUND, "Not found invoice", statusCode: StatusCodes.Status404NotFound));

                return Ok(ApiResponse<Invoice>.SuccessResponse(invoice));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceReq req)
        {
            try
            {
                if (req == null || req.amount == 0 || req.account_id == Guid.Empty || req.order_id == Guid.Empty || string.IsNullOrWhiteSpace(req.order_name))
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));

                Invoice Invoice = new()
                {
                    id = Guid.NewGuid(),
                    account_id = req.account_id,
                    order_id = req.order_id,
                    order_name = req.order_name,
                    amount = req.amount,
                    status = (int)InvoiceStatus.Temporary,
                    created_at = DateTimeHelper.GetUtcTimestamp(),
                    created_by = "system",
                    updated_by = "system",
                    updated_at = DateTimeHelper.GetUtcTimestamp()
                };

                int effect = await _serviceInvoice.Insert(Invoice);
                if (effect < 1)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, EcommerceConstants.ERROR_INTERNAL_SERVER, statusCode: StatusCodes.Status500InternalServerError));

                return Ok(ApiResponse<string>.SuccessResponse("Create temporary invoice successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }
        [HttpPost("hook/{invoiceId}/{status}")]
        public async Task<IActionResult> Checkout(string invoiceId, string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(invoiceId) || !Guid.TryParse(invoiceId, out Guid ivId) || string.IsNullOrWhiteSpace(status))
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));
                int invoiceStatus = status.Trim().Equals("paid", StringComparison.OrdinalIgnoreCase) ? (int)InvoiceStatus.Paid : (int)InvoiceStatus.Cancel;

                Invoice? invoice = await _serviceInvoice.GetAsync(ivId);
                if (invoice == null || invoice.status != (int)InvoiceStatus.Temporary)
                    return StatusCode(StatusCodes.Status404NotFound,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_NOT_FOUND, "Not found Invoice", statusCode: StatusCodes.Status404NotFound));

                //Push queue to process
                string topic = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_TOPIC, _configuration);
                await _eventBus.PublishAsync<Invoice>(topic, invoice);
                return Ok(ApiResponse<string>.SuccessResponse("Publish successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }
        #endregion
    }
}
