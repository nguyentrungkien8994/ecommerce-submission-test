using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.EMAIL.CORE;
using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.HELPER;
using ECOMMERCE.SUBMISSION.SERVICE;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMMERCE.SUBMISSION.API.EMAIL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        #region Declare/Contructor
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEventBus _eventBus;

        public EmailController(ILogger<EmailController> logger,
            IServiceBase<Order> serviceOrder,
            IHttpContextAccessor accessor,
            IEventBus bus,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _eventBus = bus;
        }
        #endregion

        #region EndPoint
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailRequest req)
        {
            try
            {
                string topic = ConfigHelper.GetConfigByKey(EcommerceConstants.KAFKA_TOPIC, _configuration);
                await _eventBus.PublishAsync<EmailRequest>(topic, req);
                return Ok(ApiResponse<string>.SuccessResponse("", "Push email to queue successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Problem(ex.Message);
            }
        }
        #endregion
    }
}
