using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;


namespace ReqBot
{
    public class CoreferenceDetection
    {
        public async void Detection()
        {
            var client = new HttpClient();

            // Create the HttpContent for the form to be posted.
            var requestContent = new FormUrlEncodedContent(new[] {new KeyValuePair<string, string>("text", "This is a block of text"),});

            // Get the response.
            HttpResponseMessage response = await client.PostAsync("http://api.repustate.com/v2/demokey/score.json", requestContent);

            // Get the response content.
            HttpContent responseContent = response.Content;
        }
    }
}