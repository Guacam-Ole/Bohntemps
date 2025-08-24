using Newtonsoft.Json;

namespace BohnTemps.BeansApi
{
    public class Communications
    {
        private const string BaseAddress = "https://api.rocketbeans.tv/v1/";

        public static async Task<T> GetResponse<T>(string url, Dictionary<string, object>? parameters)
        {
            var requestUrl = url;
            if (parameters != null)
            {
                var paramStr = string.Empty;
                foreach (var parameter in parameters)
                {
                    paramStr += paramStr.Length == 0 ? "?" : "&";
                    paramStr += string.Concat(parameter.Key, "=", parameter.Value);
                }
                requestUrl += paramStr;
            }

            HttpClient client = new()
            {
                BaseAddress = new Uri(BaseAddress)
            };
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var responseValue = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseValue)!;
        }

        public static async Task<Stream?> DownloadImage(string url, bool useBaseAddress = false)
        {
            HttpClient client = new();
            if (useBaseAddress) client.BaseAddress = new Uri(BaseAddress);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null; // Dont throw error if just image is missing
            return await response.Content.ReadAsStreamAsync();
        }
    }
}