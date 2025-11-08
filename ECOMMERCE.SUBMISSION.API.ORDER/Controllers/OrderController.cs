using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using ECOMMERCE.SUBMISSION.SERVICE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECOMMERCE.SUBMISSION.API.ORDER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {

        #region Declare/Contructor
        private readonly ILogger<OrderController> _logger;
        private readonly IServiceBase<Order> _serviceOrder;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;
        private readonly IEventBus _eventBus;


        public OrderController(ILogger<OrderController> logger,
            IServiceBase<Order> serviceOrder,
            IHttpContextAccessor accessor,
            IEventBus bus,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceOrder = serviceOrder;
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
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                (bool success, Guid accountId, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                var orders = await _serviceOrder.Search(x => x.account_id == accountId);
                return Ok(ApiResponse<List<Order>>.SuccessResponse(orders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            try
            {
                (bool success, Guid accountId, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                if (string.IsNullOrWhiteSpace(orderId) || !Guid.TryParse(orderId, out Guid odId))
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));

                var order = await _serviceOrder.GetAsync(odId);
                if (order == null || order.account_id != accountId)
                    return StatusCode(StatusCodes.Status404NotFound,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_NOT_FOUND, "Not found order", statusCode: StatusCodes.Status404NotFound));

                return Ok(ApiResponse<Order>.SuccessResponse(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderReq req)
        {
            try
            {
                (bool success, Guid accountId, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                if (req == null || string.IsNullOrWhiteSpace(req.name) || req.amount == 0 || req.specification_id == Guid.Empty)
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));

                Order order = new()
                {
                    id = Guid.NewGuid(),
                    account_id = accountId,
                    name = req.name,
                    amount = req.amount,
                    specification_id = req.specification_id,
                    status = (int)OrderStatus.Ready_Checkout,
                    created_at = DateTimeHelper.GetUtcTimestamp(),
                    created_by = email,
                    updated_by = email,
                    updated_at = DateTimeHelper.GetUtcTimestamp()
                };

                int effect = await _serviceOrder.Insert(order);
                if (effect < 1)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, EcommerceConstants.ERROR_INTERNAL_SERVER, statusCode: StatusCodes.Status500InternalServerError));

                return Ok(ApiResponse<string>.SuccessResponse("Create order successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }
        [HttpPost("checkout/{orderId}")]
        public async Task<IActionResult> Checkout(string orderId)
        {
            try
            {
                (bool success, Guid accountId, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                if (string.IsNullOrWhiteSpace(orderId) || !Guid.TryParse(orderId, out Guid odId))
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));

                Order? order = await _serviceOrder.GetAsync(odId);
                if (order == null || order.account_id != accountId || order.status != (int)OrderStatus.Ready_Checkout)
                    return StatusCode(StatusCodes.Status404NotFound,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_NOT_FOUND, "Not found order", statusCode: StatusCodes.Status404NotFound));
                //Push queue to process
                string topic = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_TOPIC, _configuration);
                await _eventBus.PublishAsync<Order>(topic, order);
                return Ok(ApiResponse<string>.SuccessResponse("Update order status successfully!"));
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
