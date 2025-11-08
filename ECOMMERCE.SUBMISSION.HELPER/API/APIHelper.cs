using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.HELPER;

public class APIHelper
{
    public static async Task<string> GetAsync(HttpClient _httpClient,string url, Dictionary<string, string>? headers = null, AuthenticationHeaderValue? authenticationHeader = null)
    {
        if (headers != null && headers.Count > 0)
            foreach (var item in headers)
            {
                _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        if (authenticationHeader != null)
            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> PostAsync<T>(HttpClient _httpClient, string url, T data, Dictionary<string, string>? headers = null, AuthenticationHeaderValue? authenticationHeader = null)
    {
        if (headers != null && headers.Count > 0)
            foreach (var item in headers)
            {
                _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        if (authenticationHeader != null)
            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
        string jsonData = JsonConvert.SerializeObject(data);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    public static string Combine(string baseUrl, string route)
    {
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(route))
            return "";
        baseUrl = baseUrl.TrimEnd('/');
        route = route.TrimStart('/');
        return $"{baseUrl}/{route}";
    }
}
