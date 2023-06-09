using Newtonsoft.Json;

namespace BohnTemps.BeansApi
{
    public class Communications
    {
        private const string _baseAddress = "https://api.rocketbeans.tv/v1/";

        public static async Task<T> GetResponse<T>(string url, Dictionary<string, object> paramters)
        {
            string requestUrl = url;
            if (paramters != null)
            {
                var paramStr = string.Empty;
                foreach (var paramter in paramters)
                {
                    paramStr += paramStr.Length == 0 ? "?" : "&";
                    paramStr += string.Concat(paramter.Key, "=", paramter.Value);
                }
                requestUrl += paramStr;
            }

            HttpClient client = new()
            {
                BaseAddress = new Uri(_baseAddress)
            };
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var responseValue = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseValue)!;
        }

        public static async Task<Stream?> DownloadImage(string url, bool useBaseAddress = false)
        {
            HttpClient client = new();
            if (useBaseAddress) client.BaseAddress = new Uri(_baseAddress);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null; // Dont throw error if just image is missing
            return await response.Content.ReadAsStreamAsync();
        }
    }
}