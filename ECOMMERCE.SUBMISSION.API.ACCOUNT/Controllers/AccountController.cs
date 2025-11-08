using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.HELPER;
using ECOMMERCE.SUBMISSION.SERVICE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Principal;

namespace ECOMMERCE.SUBMISSION.API.ACCOUNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region "Declare/Contructor"
        private readonly ILogger<AccountController> _logger;
        private readonly IServiceBase<Account> _serviceAccount;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;

        public AccountController(ILogger<AccountController> logger,
            IServiceBase<Account> serviceAccount,
            IHttpContextAccessor accessor,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceAccount = serviceAccount;
            _configuration = configuration;
            _accessor = accessor;
        }
        #endregion

        #region Private Function

        /// <summary>
        /// Generate access token, refresh token 
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="email"></param>
        /// <param name="givenName"></param>
        /// <returns></returns>
        private object GenerateToken(string sub, string email = "", string givenName = "")
        {
            string jwtKey = ConfigHelper.GetConfigByKey(EcommerceConstants.JWT_KEY, _configuration);
            string issuer = ConfigHelper.GetConfigByKey(EcommerceConstants.JWT_ISSUER, _configuration);
            string audience = ConfigHelper.GetConfigByKey(EcommerceConstants.JWT_AUDIENCE, _configuration);
            string accessToken = JWTHelper.GenerateJwtToken(jwtKey, issuer, audience, DateTime.UtcNow.AddMinutes(15), sub: sub, email: email, givenName: givenName);
            string refreshToken = JWTHelper.GenerateJwtToken(jwtKey, issuer, audience, DateTime.UtcNow.AddMinutes(60), sub: sub, email: email, givenName: givenName);
            return new { access_token = accessToken, refresh_token = refreshToken };
        }
        #endregion

        #region EndPoint

        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] SignupReq req)
        {
            try
            {
                //save data
                if (req == null || string.IsNullOrWhiteSpace(req.email) || string.IsNullOrWhiteSpace(req.password))
                {
                    var res = ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA);
                    return StatusCode(res.status_code, res);
                }
                string email = req.email.ToLower().Trim();
                string pwd = req.password.Trim();
                if (!CommonHelper.ValidateRegex(EcommerceConstants.REGEX_PATTERN_EMAIL, email))
                {
                    var res = ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, EcommerceConstants.MSG_INCORRECT_EMAIL_FORMAT);
                    return StatusCode(res.status_code, res);
                }

                if (!CommonHelper.ValidateRegex(EcommerceConstants.REGEX_PATTERN_PASSWORD, pwd))
                {
                    var res = ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, EcommerceConstants.MSG_INCORRECT_PASSWORD_FORMAT);
                    return StatusCode(res.status_code, res);
                }

                Account account = new()
                {
                    id = Guid.NewGuid(),
                    email = email,
                    password = CryptographyHelper.ComputeSHA256Hash(pwd),
                    created_at = DateTimeHelper.GetUtcTimestamp(),
                    created_by = email,
                    updated_by = email,
                    updated_at = DateTimeHelper.GetUtcTimestamp()
                };
                int effect = await _serviceAccount.Insert(account);
                if (effect < 1)
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, EcommerceConstants.ERROR_INTERNAL_SERVER, statusCode: StatusCodes.Status500InternalServerError));
                return Ok(ApiResponse<string>.SuccessResponse("Register successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.email) || string.IsNullOrWhiteSpace(req.password))
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_DATA, statusCode: StatusCodes.Status400BadRequest));
                string email = req.email.ToLower().Trim();
                string pwd = CryptographyHelper.ComputeSHA256Hash(req.password.Trim());
                var account = await _serviceAccount.SearchOne(x => x.email == req.email && x.password == pwd);
                if (account == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_CREDENTIALS, statusCode: StatusCodes.Status500InternalServerError));

                var res = GenerateToken(account.id.ToString(), account.email, account.email);
                return Ok(ApiResponse<object>.SuccessResponse(res));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                string token = JWTHelper.GetAccessTokenFromContext(_accessor);
                if (string.IsNullOrWhiteSpace(token))
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_TOKEN, statusCode: StatusCodes.Status500InternalServerError));

                var parserToken = JWTHelper.ParserAccessToken(token);
                string? sub = parserToken?.Subject;
                string? email = parserToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(email))
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INVALID_TOKEN, statusCode: StatusCodes.Status500InternalServerError));


                var res = GenerateToken(sub, email, email);
                return await Task.FromResult(Ok(ApiResponse<object>.SuccessResponse(res)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailResponse(EcommerceConstants.ERROR_INTERNAL_SERVER, ex.Message, statusCode: StatusCodes.Status500InternalServerError));
            }
        }
        #endregion
    }
}
