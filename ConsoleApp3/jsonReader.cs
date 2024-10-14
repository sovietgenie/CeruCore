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
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JsonStructure data = JsonConvert.DeserializeObject<JsonStructure>(json);

                this.Token = data.Token;
                this.Prefix = data.Prefix;
            }
        }
        internal sealed class JsonStructure
        {
            public required string Token { get; set; }
            public required string Prefix { get; set; }
        }

        public static string FindKeyByNestedProperty(string searchString, string jsonFilePath)
        {
            try
            {
                // Read the JSON file
                var jsonData = File.ReadAllText(jsonFilePath);
                var jsonObject = JObject.Parse(jsonData);

                // Iterate through the key-value pairs in the JSON object
                foreach (var item in jsonObject)
                {
                    // Check all localized names for a match
                    foreach (var language in item.Value.Children<JProperty>())
                    {
                        if (language.Value.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        {
                            return item.Key; // Return the key (ID) when a match is found
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
        public static string FindNameByID(string searchID, string jsonFilePath)
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var jsonObject = JObject.Parse(jsonData);
            try
            {
                foreach (var item in jsonObject)
                {
                    if (searchID == item.Key)
                    {
                        return item.Value.ToString();
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
