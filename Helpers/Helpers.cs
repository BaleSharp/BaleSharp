using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Bale.Helpers
{
    public class ApiSender
    {
        public async Task<string> SendUrl(string url, Dictionary<string, object>? parameters = null)
        {
            HttpClient _http = new HttpClient();            

            if (parameters != null && parameters.Count != 0)
            {
                url += "?";
                foreach (var param in parameters)
                {
                    url += $"{param.Key}={param.Value}&";
                }
                // Remove the trailing '&' if parameters were added
                url = url.TrimEnd('&');
            }

            HttpResponseMessage res = await _http.GetAsync(url);
            try { res.EnsureSuccessStatusCode(); }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            string content = await res.Content.ReadAsStringAsync();
            return JsonConvert.SerializeObject(content);
        }
    }
}
