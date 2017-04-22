using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Foodbot
{
    public class LuisFoodBot
    {
        public static async Task<RecipeLUIS> ParseUserInput(string strInput)
        {
            string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                string uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/a8cd9b98-dca9-4cd6-b510-7f139a5aee53?subscription-key=fe8e4a6b9f374235986627adfa3f0917&timezoneOffset=0.0&verbose=true&q=" + strEscaped;
                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<RecipeLUIS>(jsonResponse);
                    return _Data;
                }
            }
            return null;
        }
    }

    public class RecipeLUIS
    {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }
}