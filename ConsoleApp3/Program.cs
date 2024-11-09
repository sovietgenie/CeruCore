using CeruCore.commands.Prefix;
using CeruCore.commands.Slash;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using MongoDB.Driver;

namespace CeruCore
{
    internal class Program
    {
        public static DiscordClient? Client { get; set; }
        public static CommandsNextExtension? Commands { get; set; }

        public static MongoClient? MongoClient { get; set; }

        static async Task Main(string[] args)
        {
            var JsonReader = new JsonReader();
            await JsonReader.ReadJSON();

            var DiscordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = JsonReader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(DiscordConfig);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;

            var CommandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { JsonReader.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,

            };

            Commands = Client.UseCommandsNext(CommandsConfig);

            var SlashCommandsConfiguration = Client.UseSlashCommands();

            Commands.RegisterCommands<Commands>();
            SlashCommandsConfiguration.RegisterCommands<SlashCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

            var mongoSettings = MongoClientSettings.FromConnectionString(JsonReader.DbConnectionString);
            mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
            MongoClient = new MongoClient(mongoSettings);
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
