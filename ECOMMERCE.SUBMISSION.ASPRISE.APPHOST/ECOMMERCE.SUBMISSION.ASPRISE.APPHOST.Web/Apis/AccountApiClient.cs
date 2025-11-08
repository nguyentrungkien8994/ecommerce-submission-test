using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.HELPER;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using Newtonsoft.Json;

namespace ECOMMERCE.SUBMISSION.WEB.ADMIN;

public class AccountApiClient(HttpClient httpClient, CustomAuthStateProvider _authProvider)
{
    public async Task<int> Register(RegisterDTO req)
    {
        string strResult = await APIHelper.PostAsync(httpClient, "/api/account/register", req);
        return 1;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        string strResult = await APIHelper.PostAsync(httpClient, "/api/account/login", new LoginDTO() { email= email , password = password});
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TokenDTO>>(strResult);

        var accessToken = apiResponse?.data?.access_token;
        if (string.IsNullOrWhiteSpace(accessToken))
            return false;

        await _authProvider.SetTokenAsync(accessToken);
        return true;
    }

    public async Task LogoutAsync() => await _authProvider.RemoveTokenAsync();
}
