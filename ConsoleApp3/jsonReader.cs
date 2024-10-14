using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CeruCore
{
    internal class JsonReader
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public async Task ReadJSON()
        {
            using (StreamReader SR = new StreamReader("config.json"))
            {
                string Json = await SR.ReadToEndAsync();
                JsonStructure Data = JsonConvert.DeserializeObject<JsonStructure>(Json);

                this.Token = Data.Token;
                this.Prefix = Data.Prefix;
            }
        }
        internal sealed class JsonStructure
        {
            public required string Token { get; set; }
            public required string Prefix { get; set; }
        }

        public static string FindKeyByNestedProperty(string SearchString, string JsonFilePath)
        {
            try
            {
                // Read the JSON file
                var JsonData = File.ReadAllText(JsonFilePath);
                var JsonObject = JObject.Parse(JsonData);

                // Iterate through the key-value pairs in the JSON object
                foreach (var Item in JsonObject)
                {
                    // Check all localized names for a match
                    foreach (var Language in Item.Value.Children<JProperty>())
                    {
                        if (Language.Value.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                        {
                            return Item.Key; // Return the key (ID) when a match is found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null; // Return null if no match is found
        }
        public static string FindNameByID(string SearchID, string JsonFilePath)
        {
            var JsonData = File.ReadAllText(JsonFilePath);
            var JsonObject = JObject.Parse(JsonData);
            try
            {
                foreach (var Item in JsonObject)
                {
                    if (SearchID == Item.Key)
                    {
                        return Item.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return null;
        }
    }
}
