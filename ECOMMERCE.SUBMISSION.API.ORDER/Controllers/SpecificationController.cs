using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
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
    public class SpecificationController : ControllerBase
    {
        #region Declare/Contructor
        private readonly ILogger<SpecificationController> _logger;
        private readonly IServiceBase<Specification> _serviceSpecification;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;

        public SpecificationController(ILogger<SpecificationController> logger,
            IServiceBase<Specification> serviceSpecification,
            IHttpContextAccessor accessor,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceSpecification = serviceSpecification;
            _configuration = configuration;
            _accessor = accessor;
        }
        #endregion

        #region Private Function
        /// <summary>
        /// Validate access token
        /// </summary>
        /// <returns></returns>
        private (bool success, Guid account_id, string email, string error, int statusCode) ValidateAccessToken()
        {
            string token = JWTHelper.GetAccessTokenFromContext(_accessor);
            if (string.IsNullOrWhiteSpace(token))
                return (false, Guid.Empty, "", EcommerceConstants.ERROR_INVALID_TOKEN, StatusCodes.Status500InternalServerError);

            var parserToken = JWTHelper.ParserAccessToken(token);
            string? sub = parserToken?.Subject;
            string? email = parserToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? sub;
            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out Guid account_id) || string.IsNullOrWhiteSpace(email))
                return (false, Guid.Empty, "", EcommerceConstants.ERROR_INVALID_TOKEN, StatusCodes.Status500InternalServerError);

            return (true, account_id, email, "", StatusCodes.Status200OK);
        }
        #endregion

        #region EndPoint
        [HttpGet]
        public async Task<IActionResult> GetSpecifications()
        {
            try
            {
                (bool success, Guid account_id, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                var specifications = await _serviceSpecification.Search(x => x.account_id == account_id);

                return Ok(ApiResponse<List<Specification>>.SuccessResponse(specifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecificationReq req)
        {
            try
            {
                (bool success, Guid account_id, string email, string error, int statusCode) = ValidateAccessToken();
                if (!success)
                    return StatusCode(statusCode,
                    ApiResponse<string>.FailResponse(error, error, statusCode: statusCode));

                if (req == null || string.IsNullOrWhiteSpace(req.name) || string.IsNullOrWhiteSpace(req.instructions))
                    return StatusCode(StatusCodes.Status400BadRequest,
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));

                Specification specification = new()
                {
                    id = Guid.NewGuid(),
                    account_id = account_id,
                    name = req.name,
                    instructions= req.instructions,
                    created_at = DateTimeHelper.GetUtcTimestamp(),
                    created_by = email,
                    updated_by = email,
                    updated_at = DateTimeHelper.GetUtcTimestamp()
                };

                int effect = await _serviceSpecification.Insert(specification);
                if (effect < 1)
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, EcommerceConstants.ERROR_INTERNAL_SERVER, statusCode: StatusCodes.Status500InternalServerError));

                return Ok(ApiResponse<string>.SuccessResponse("Create specification successfully!"));
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
