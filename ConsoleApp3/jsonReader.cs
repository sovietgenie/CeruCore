using Newtonsoft.Json;

namespace CeruCore
{
    internal class jsonReader
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
    }
}
