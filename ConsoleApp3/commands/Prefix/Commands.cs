using CeruCore.miscRef;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Epoch.net;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using static CeruCore.miscRef.PriceCheck.PriceCheckJson;

namespace CeruCore.commands.Prefix
{
    public class Commands : BaseCommandModule
    {
        //Help Command
        [Command("Help")]
        public async Task Help(CommandContext ctx)
        {
            var Message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Hello {ctx.User.Username}!")
                .WithDescription($"Thank you for using Ceru!")
                .WithColor(new DiscordColor(0x2F5889)));

            await ctx.Channel.SendMessageAsync(Message);
        }

        //A cardgame bot
        [Command("Cardgame")]
        public async Task Cardgame(CommandContext ctx)
        {
            var UserCard = new CardSystem();

            var UserCardEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Your card is {UserCard.SelectedCard}")
                .WithColor(new DiscordColor(0x2F5889)));

            await ctx.Channel.SendMessageAsync(UserCardEmbed);

            var BotCard = new CardSystem();

            var BotCardEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Ceru's card is {BotCard.SelectedCard}")
                .WithColor(new DiscordColor(0xDBB457)));
            await ctx.Channel.SendMessageAsync(BotCardEmbed);

            if (UserCard.SelectedNumber > BotCard.SelectedNumber)
            {
                //victory
                var UserVictoryEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"{ctx.User.Username} has won with a {UserCard.SelectedCard}!")
                .WithColor(new DiscordColor(0x658241)));

                await ctx.Channel.SendMessageAsync(UserVictoryEmbed);
            }
            else if (UserCard.SelectedNumber == BotCard.SelectedNumber)
            {
                //draw
                var DrawEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("The game ends in a draw!")
                .WithColor(new DiscordColor(0x66304E)));

                await ctx.Channel.SendMessageAsync(DrawEmbed);
            }
            else
            {
                //loss
                var BotVictoryEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Ceru has won with a {BotCard.SelectedCard}!")
                .WithColor(new DiscordColor(0x781A1A)));

                await ctx.Channel.SendMessageAsync(BotVictoryEmbed);
            }
        }

        //poll creation
        [Command("Poll")]
        public async Task poll(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var Interactivity = Program.Client.GetInteractivity();

            var PollTime = TimeSpan.FromSeconds(30);

            DiscordEmoji[] EmojiOptions = {
                DiscordEmoji.FromName(Program.Client,":one:"),
                DiscordEmoji.FromName(Program.Client,":two:"),
                DiscordEmoji.FromName(Program.Client,":three:"),
                DiscordEmoji.FromName(Program.Client,":four:")
                };
            string OptionsDescription = $"{EmojiOptions[0]} | {option1} \n" +
                                        $"{EmojiOptions[1]} | {option2} \n" +
                                        $"{EmojiOptions[2]} | {option3} \n" +
                                        $"{EmojiOptions[3]} | {option4} \n";

            var PollMessage = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0x2F5889),
                Title = pollTitle,
                Description = OptionsDescription
            };

            var SentPoll = await ctx.Channel.SendMessageAsync(embed: PollMessage);

            foreach (var emoji in EmojiOptions)
            {
                Thread.Sleep(1000);
                await SentPoll.CreateReactionAsync(emoji);
            }

            var TotalReactions = await Interactivity.CollectReactionsAsync(SentPoll, PollTime);

            int Count1 = 0;
            int Count2 = 0;
            int Count3 = 0;
            int Count4 = 0;

            foreach (var Emoji in TotalReactions)
            {
                if (Emoji.Emoji == EmojiOptions[0]) { Count1++; }
                else if (Emoji.Emoji == EmojiOptions[1]) { Count2++; }
                else if (Emoji.Emoji == EmojiOptions[2]) { Count3++; }
                else if (Emoji.Emoji == EmojiOptions[3]) { Count4++; }
            }

            int TotalVotes = Count1 + Count2 + Count3 + Count4;

            string ResultsDescription = $"{EmojiOptions[0]} | {option1} : {Count1} votes \n" +
                                        $"{EmojiOptions[1]} | {option2} : {Count2} votes \n" +
                                        $"{EmojiOptions[2]} | {option3} : {Count3} votes \n" +
                                        $"{EmojiOptions[3]} | {option4} : {Count4} votes \n\n" +
                                        $"Total Votes: {TotalVotes}";

            var ResultEmbed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0x2F5889),
                Title = pollTitle,
                Description = ResultsDescription
            };

            await ctx.Channel.SendMessageAsync(embed: ResultEmbed);
        }

        [Command("PriceCheck")]
        public async Task PriceCheck(CommandContext ctx, string Item)
        {
            //var ItemIdMapJSON = "C:\\Users\\eugen\\source\\repos\\ConsoleApp3\\ConsoleApp3\\miscRef\\PriceCheck\\gamefile\\ffxivItemIdMap.json";
            var WorldIdMapJSON = "C:\\Users\\eugen\\source\\repos\\ConsoleApp3\\ConsoleApp3\\miscRef\\PriceCheck\\gamefile\\ffxivWorldIdMap.json";

            //var Result = JsonReader.FindKeyByNestedProperty(Item, ItemIdMapJSON);
            var lumina = new Lumina.GameData("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack");

            var ItemSheet = lumina.GetExcelSheet<Item>();

            var Result = 0;
            
            foreach (var Row in ItemSheet)
            {
                if (Row.Name.ToString() == Item)
                {
                    Result = (int)Row.RowId;
                }

            }

            if (Result != null && Result != 0)
            {
                var SuccessMessage = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0x2F5889),
                    Title = "Requested Item for Price Check",
                    Description = $"Requested item ({Item}) was found in data file with ID: {Result}"
                };

                await ctx.Channel.SendMessageAsync(embed: SuccessMessage);

                var ApiClient = new HttpClient();
                try
                {
                    HttpResponseMessage Response = await ApiClient.GetAsync($"https://universalis.app/api/v2/aggregated/Aether/{Result}");

                    if (Response.IsSuccessStatusCode)
                    {
                        string JsonResponse = await Response.Content.ReadAsStringAsync();
                        ApiResponse ParsedJSON = JsonConvert.DeserializeObject<ApiResponse>(JsonResponse);

                        foreach (var ItemResult in ParsedJSON.Results)
                        {
                            //Price Check Header Message Setup
                            var EmbedPCHeaderMessage = $"Item: {Item} \n" +
                                               $"Item ID: {Result}";

                            //NQ Message Setup
                            var EmbedNQInfoMessage = "";

                            if (ItemResult.NQ.MinListing.DC != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.NQ.MinListing.DC.WorldId.ToString(), WorldIdMapJSON);
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"Min Listing: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {ItemResult.NQ.MinListing.DC.Price} gil \n" +
                                               $"World ID: {FoundWorld} \n";
                            }
                            if (ItemResult.NQ.MinListing.Region != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.NQ.MinListing.Region.WorldId.ToString(), WorldIdMapJSON);
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {ItemResult.NQ.MinListing.Region.Price} gil\n" +
                                               $"World ID: {FoundWorld} \n\n";
                            }
                            if (ItemResult.NQ.RecentPurchase.DC != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.NQ.RecentPurchase.DC.WorldId.ToString(), WorldIdMapJSON);
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"Recent Purchase: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {ItemResult.NQ.RecentPurchase.DC.Price} gil \n" +
                                               $"Timestamp: {ItemResult.NQ.RecentPurchase.DC.Timestamp.ToDateTime()} \n" +
                                               $" World ID: {FoundWorld} \n";
                            }
                            if (ItemResult.NQ.RecentPurchase.Region != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.NQ.RecentPurchase.Region.WorldId.ToString(), WorldIdMapJSON);
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {ItemResult.NQ.RecentPurchase.Region.Price} gil \n" +
                                               $"Timestamp: {ItemResult.NQ.RecentPurchase.Region.Timestamp.ToDateTime()} \n" +
                                               $"World ID: {FoundWorld} \n\n";
                            }
                            if (ItemResult.NQ.AverageSalePrice.DC != null)
                            {
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"Average Sale Price (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {Convert.ToInt64(ItemResult.NQ.AverageSalePrice.DC.Price)} gil \n";
                            }
                            if (ItemResult.NQ.AverageSalePrice.Region != null)
                            {
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {Convert.ToInt64(ItemResult.NQ.AverageSalePrice.Region.Price)} gil \n\n";
                            }
                            if (ItemResult.NQ.DailySaleVelocity.DC != null)
                            {
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"Daily Sale Velocity (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Qty: {Convert.ToInt64(ItemResult.NQ.DailySaleVelocity.DC.Quantity)} units \n";
                            }
                            if (ItemResult.NQ.DailySaleVelocity.Region != null)
                            {
                                EmbedNQInfoMessage = EmbedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Qty: {Convert.ToInt64(ItemResult.NQ.DailySaleVelocity.Region.Quantity)} units";
                            }

                            //HQ Message Setup
                            var EmbedHQInfoMessage = "";

                            if (ItemResult.HQ.MinListing.DC != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.HQ.MinListing.DC.WorldId.ToString(), WorldIdMapJSON);
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"Min Listing: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {ItemResult.HQ.MinListing.DC.Price} gil \n" +
                                               $"World ID: {FoundWorld} \n";
                            }
                            if (ItemResult.HQ.MinListing.Region != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.HQ.MinListing.Region.WorldId.ToString(), WorldIdMapJSON);
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {ItemResult.HQ.MinListing.Region.Price} gil \n" +
                                               $"World ID: {FoundWorld} \n\n";
                            }
                            if (ItemResult.HQ.RecentPurchase.DC != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.HQ.RecentPurchase.DC.WorldId.ToString(), WorldIdMapJSON);
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"Recent Purchase: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {ItemResult.HQ.RecentPurchase.DC.Price} gil \n" +
                                               $"Timestamp: {ItemResult.HQ.RecentPurchase.DC.Timestamp.ToDateTime()} \n" +
                                               $"World ID: {FoundWorld} \n";
                            }
                            if (ItemResult.HQ.RecentPurchase.Region != null)
                            {
                                var FoundWorld = JsonReader.FindNameByID(ItemResult.HQ.RecentPurchase.Region.WorldId.ToString(), WorldIdMapJSON);
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {ItemResult.HQ.RecentPurchase.Region.Price} gil \n" +
                                               $"Timestamp: {ItemResult.HQ.RecentPurchase.Region.Timestamp.ToDateTime()} \n" +
                                               $"World ID: {FoundWorld} \n\n";
                            }
                            if (ItemResult.HQ.AverageSalePrice.DC != null)
                            {
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"Average Sale Price (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {Convert.ToInt64(ItemResult.HQ.AverageSalePrice.DC.Price)} gil \n";
                            }
                            if (ItemResult.HQ.AverageSalePrice.Region != null)
                            {
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {Convert.ToInt64(ItemResult.HQ.AverageSalePrice.Region.Price)} gil \n\n";
                            }
                            if (ItemResult.HQ.DailySaleVelocity.DC != null)
                            {
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"Daily Sale Velocity (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Qty: {Convert.ToInt64(ItemResult.HQ.DailySaleVelocity.DC.Quantity)} units \n";
                            }
                            if (ItemResult.HQ.DailySaleVelocity.Region != null)
                            {
                                EmbedHQInfoMessage = EmbedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Qty: {Convert.ToInt64(ItemResult.HQ.DailySaleVelocity.Region.Quantity)} units";
                            }

                            var PriceCheckHeaderEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"Price Check: {Item}",
                                Description = EmbedPCHeaderMessage
                            };

                            await ctx.Channel.SendMessageAsync(embed: PriceCheckHeaderEmbedMessageFinal);

                            var PriceCheckNQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{Item} - NQ Information",
                                Description = EmbedNQInfoMessage
                            };

                            await ctx.Channel.SendMessageAsync(embed: PriceCheckNQEmbedMessageFinal);

                            var PriceCheckHQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{Item} - HQ Information",
                                Description = EmbedHQInfoMessage
                            };

                            await ctx.Channel.SendMessageAsync(embed: PriceCheckHQEmbedMessageFinal);
                        }



                    }
                    else
                    {
                        Console.WriteLine($"API returned an unsuccessful status code");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred: {e.Message}");
                }

            }
            else
            {
                var FailedMessage = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0x2F5889),
                    Title = "Requested Item for Price Check",
                    Description = $"Requested item ({Item}) was NOT found in data file"
                };

                await ctx.Channel.SendMessageAsync(embed: FailedMessage);
            }

        }

        //Lumina Command
        [Command("Lumina")]
        public async Task Lumina(CommandContext ctx)
        {
            var lumina = new Lumina.GameData("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack");

            var ItemSheet = lumina.GetExcelSheet<Item>();

            Console.WriteLine(ItemSheet.GetRow(44001).Icon.ToString());
            //foreach (var Row in ItemSheet)
            //{   
            //        if (Row.Name != "")
            //    {
            //        Console.WriteLine($"{Row.RowId}" + ": " + $"{Row.Name}");
            //    }

            //}
        }
    }
}
