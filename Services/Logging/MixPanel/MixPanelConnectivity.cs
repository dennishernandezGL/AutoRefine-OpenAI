using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Services.Logging.MixPanel
{
    public class MixPanelConnectivity : LoggingFacade
    {
        private readonly string MixPanelImportUrl;
        private readonly string MixPanelExportUrl;
        private readonly string MixPanelJqltUrl;
        private readonly string MixPanelToken;
        private readonly string MixPanelApiKey;
        private readonly string MixPanelApiSecret;

        public MixPanelConnectivity(IConfiguration configuration)
        {
            MixPanelImportUrl = configuration["MixPanel:ImportUrl"] ?? throw new ArgumentNullException("MixPanel:ImportUrl");
            MixPanelExportUrl = configuration["MixPanel:ExportUrl"] ?? throw new ArgumentNullException("MixPanel:ExportUrl");
            MixPanelJqltUrl = configuration["MixPanel:JqltUrl"] ?? throw new ArgumentNullException("MixPanel:JqltUrl");
            MixPanelToken = Environment.GetEnvironmentVariable("MIXPANEL_TOKEN") ?? throw new ArgumentNullException("MIXPANEL_TOKEN");
            MixPanelApiKey = Environment.GetEnvironmentVariable("MIXPANEL_API_KEY") ?? throw new ArgumentNullException("MIXPANEL_API_KEY");
            MixPanelApiSecret = Environment.GetEnvironmentVariable("MIXPANEL_API_SECRET") ?? throw new ArgumentNullException("MIXPANEL_API_SECRET");
        }

        public override void LogInfo(string message, object jsonStructure, Context context)
        {
            SendToMixPanel(LogType.Info, jsonStructure, context, message);
        }

        public override void LogWarning(string message, object jsonStructure, Context context)
        {
            SendToMixPanel(LogType.Warning, jsonStructure, context, message);
        }

        public override void LogError(ExceptionDetails exception, object jsonStructure, Context context)
        {
            SendToMixPanel(LogType.Error, jsonStructure, context, $"{exception.Message} {exception.StackTrace}");
        }

        private void SendToMixPanel(LogType logType, object jsonStructure, Context context, string message = null)
        {
            object[] logData = [
                new {
                    @event = logType.ToString(),
                    properties = new {
                        token = MixPanelToken,
                        distinct_id = Guid.NewGuid().ToString(),
                        time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        Context = context,
                        Message = message,
                        Data = jsonStructure
                    }
                }
            ];

            string serializedData = JsonConvert.SerializeObject(logData);
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, MixPanelImportUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{MixPanelApiSecret}:{string.Empty}")));
                request.Content = new StringContent(serializedData, System.Text.Encoding.UTF8, "application/json");

                var response = httpClient.SendAsync(request).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
            }
        }

        public override async Task<IEnumerable<dynamic>> RetrieveLogsAsync(DateTime startDate, DateTime endDate, string eventName)
        {
            return await RetrieveLogsLoggingAsync(startDate, endDate, eventName);
        }

        public override async Task<IEnumerable<dynamic>> RetrieveLogsByContextAsync(Func<Context, bool> contextComparison, DateTime startDate, DateTime endDate, string eventName)
        {
            var logs = await RetrieveLogsLoggingAsync(startDate, endDate, eventName);
            return logs.Where(log => contextComparison(log));
        }

        private async Task<IEnumerable<dynamic>> RetrieveLogsLoggingAsync(DateTime startDate, DateTime endDate, string eventName)
        {        
            var responseContent = await GetLogsUsingExport(startDate, endDate, eventName);
            var lines = responseContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrEmpty(responseContent))
                return Enumerable.Empty<dynamic>();
            
            IEnumerable<dynamic> events = lines
                .Select(line => JsonConvert.DeserializeObject<dynamic>(line))
                .ToList();
            return events;
        }

        private async Task<string> GetLogsUsingExport(DateTime startDate, DateTime endDate, string eventName)
        {
            var url = $"{MixPanelExportUrl}?from_date={Uri.EscapeDataString(startDate.ToString("yyyy-MM-dd"))}&to_date={Uri.EscapeDataString(endDate.ToString("yyyy-MM-dd"))}&event={Uri.EscapeDataString($"[\"{eventName}\"]")}";
            var responseContent = string.Empty;
            HttpResponseMessage response = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5); // Extend timeout to 5 minutes
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Set response type as JSON
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{MixPanelApiSecret}:{string.Empty}")));

                response = httpClient.SendAsync(request).Result;
                responseContent = response.Content.ReadAsStringAsync().Result;
            }
            response.EnsureSuccessStatusCode();
            return responseContent;
        }

        /// <summary>
        /// This required payed plan
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> GetLogsUsingJQL(DateTime startDate, DateTime endDate, string eventName)
        {
            var jqlScript = @$"
                function main() {{
                    return Events({{
                        from_date: '{startDate:yyyy-MM-dd}',
                        to_date: '{endDate:yyyy-MM-dd}'
                    }}).filter(function(event) {{
                        return event.name === '{eventName}';
                    }});
                }}
            ";            
            var responseContent = string.Empty;
            HttpResponseMessage response = null;
            using (var httpClient = new HttpClient())
            {
                var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{MixPanelApiSecret}:{string.Empty}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("script", jqlScript)
                });

                response = await httpClient.PostAsync(MixPanelJqltUrl, content);
                responseContent = await response.Content.ReadAsStringAsync();
            }
            response.EnsureSuccessStatusCode();                
            return responseContent;
        }
    }
}