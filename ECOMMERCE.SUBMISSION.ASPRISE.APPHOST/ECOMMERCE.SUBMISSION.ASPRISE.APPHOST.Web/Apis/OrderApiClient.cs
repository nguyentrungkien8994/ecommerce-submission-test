using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.HELPER;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ECOMMERCE.SUBMISSION.WEB.ADMIN;

public class OrderApiClient(HttpClient httpClient, CustomAuthStateProvider _authProvider)
{
    public async Task<List<SpecificationDTO>> GetSpecificationsAsync()
    {
        string token = await _authProvider.GetToken();
        string strResults = await APIHelper.GetAsync(httpClient, "/api/specification", authenticationHeader: new AuthenticationHeaderValue("Bearer", token));
        ApiResponse<List<SpecificationDTO>>? apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<SpecificationDTO>>>(strResults);
        return apiResponse?.data ?? new();
    }

    public async Task<int> CreateSpecificationAsync(SpecificationDTO specification)
    {
        string token = await _authProvider.GetToken();
        string strResults = await APIHelper.PostAsync(httpClient, "/api/specification", specification, authenticationHeader: new AuthenticationHeaderValue("Bearer", token));
        ApiResponse<string>? apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(strResults);
        var success = apiResponse?.success??false;
        return  success? 1 : 0;
    }
}
