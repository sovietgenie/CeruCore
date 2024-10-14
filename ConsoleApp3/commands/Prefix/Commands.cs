using CeruCore.miscRef;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Epoch.net;
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
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Hello {ctx.User.Username}!")
                .WithDescription($"Thank you for using Ceru!")
                .WithColor(new DiscordColor(0x2F5889)));

            await ctx.Channel.SendMessageAsync(message);
        }

        //A cardgame bot
        [Command("Cardgame")]
        public async Task Cardgame(CommandContext ctx)
        {
            var userCard = new CardSystem();

            var userCardEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Your card is {userCard.SelectedCard}")
                .WithColor(new DiscordColor(0x2F5889)));

            await ctx.Channel.SendMessageAsync(userCardEmbed);

            var botCard = new CardSystem();

            var botCardEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Ceru's card is {botCard.SelectedCard}")
                .WithColor(new DiscordColor(0xDBB457)));
            await ctx.Channel.SendMessageAsync(botCardEmbed);

            if (userCard.SelectedNumber > botCard.SelectedNumber)
            {
                //victory
                var userVictoryEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"{ctx.User.Username} has won with a {userCard.SelectedCard}!")
                .WithColor(new DiscordColor(0x658241)));

                await ctx.Channel.SendMessageAsync(userVictoryEmbed);
            }
            else if (userCard.SelectedNumber == botCard.SelectedNumber)
            {
                //draw
                var drawEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("The game ends in a draw!")
                .WithColor(new DiscordColor(0x66304E)));

                await ctx.Channel.SendMessageAsync(drawEmbed);
            }
            else
            {
                //loss
                var botVictoryEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Ceru has won with a {botCard.SelectedCard}!")
                .WithColor(new DiscordColor(0x781A1A)));

                await ctx.Channel.SendMessageAsync(botVictoryEmbed);
            }
        }

        //poll creation
        [Command("Poll")]
        public async Task poll(CommandContext ctx, string option1, string option2, string option3, string option4, [RemainingText] string pollTitle)
        {
            var interactivity = Program.Client.GetInteractivity();

            var pollTime = TimeSpan.FromSeconds(30);

            DiscordEmoji[] emojiOptions = {
                DiscordEmoji.FromName(Program.Client,":one:"),
                DiscordEmoji.FromName(Program.Client,":two:"),
                DiscordEmoji.FromName(Program.Client,":three:"),
                DiscordEmoji.FromName(Program.Client,":four:")
                };
            string optionsDescription = $"{emojiOptions[0]} | {option1} \n" +
                                        $"{emojiOptions[1]} | {option2} \n" +
                                        $"{emojiOptions[2]} | {option3} \n" +
                                        $"{emojiOptions[3]} | {option4} \n";

            var pollMessage = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0x2F5889),
                Title = pollTitle,
                Description = optionsDescription
            };

            var sentPoll = await ctx.Channel.SendMessageAsync(embed: pollMessage);

            foreach (var emoji in emojiOptions)
            {
                Thread.Sleep(1000);
                await sentPoll.CreateReactionAsync(emoji);
            }

            var totalReactions = await interactivity.CollectReactionsAsync(sentPoll, pollTime);

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;

            foreach (var emoji in totalReactions)
            {
                if (emoji.Emoji == emojiOptions[0]) { count1++; }
                else if (emoji.Emoji == emojiOptions[1]) { count2++; }
                else if (emoji.Emoji == emojiOptions[2]) { count3++; }
                else if (emoji.Emoji == emojiOptions[3]) { count4++; }
            }

            int totalVotes = count1 + count2 + count3 + count4;

            string resultsDescription = $"{emojiOptions[0]} | {option1} : {count1} votes \n" +
                                        $"{emojiOptions[1]} | {option2} : {count2} votes \n" +
                                        $"{emojiOptions[2]} | {option3} : {count3} votes \n" +
                                        $"{emojiOptions[3]} | {option4} : {count4} votes \n\n" +
                                        $"Total Votes: {totalVotes}";

            var resultEmbed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0x2F5889),
                Title = pollTitle,
                Description = resultsDescription
            };

            await ctx.Channel.SendMessageAsync(embed: resultEmbed);
        }

        [Command("PriceCheck")]
        public async Task PriceCheck(CommandContext ctx, string item)
        {
            Console.WriteLine("Inside of Price Check Function...");
            var itemIdMapJSON = "C:\\Users\\eugen\\source\\repos\\ConsoleApp3\\ConsoleApp3\\miscRef\\PriceCheck\\gamefile\\ffxivItemIdMap.json";
            var worldIdMapJSON = "C:\\Users\\eugen\\source\\repos\\ConsoleApp3\\ConsoleApp3\\miscRef\\PriceCheck\\gamefile\\ffxivWorldIdMap.json";

            var result = JsonSearcher.FindKeyByNestedProperty(item, itemIdMapJSON);

            if (result != null)
            {
                var successMessage = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0x2F5889),
                    Title = "Requested Item for Price Check",
                    Description = $"Requested item ({item}) was found in data file with ID: {result}"
                };

                await ctx.Channel.SendMessageAsync(embed: successMessage);

                var ApiClient = new HttpClient();
                try
                {
                    HttpResponseMessage response = await ApiClient.GetAsync($"https://universalis.app/api/v2/aggregated/Aether/{result}");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        ApiResponse parsedJSON = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                        foreach (var itemResult in parsedJSON.Results)
                        {
                            //Price Check Header Message Setup
                            var embedPCHeaderMessage = $"Item: {item} \n" +
                                               $"Item ID: {result}";

                            //NQ Message Setup
                            var embedNQInfoMessage = "";

                            if (itemResult.NQ.MinListing.DC != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.NQ.MinListing.DC.WorldId.ToString(), worldIdMapJSON);
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"Min Listing: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {itemResult.NQ.MinListing.DC.Price} gil \n" +
                                               $"World ID: {foundWorld} \n";
                            }
                            if (itemResult.NQ.MinListing.Region != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.NQ.MinListing.Region.WorldId.ToString(), worldIdMapJSON);
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {itemResult.NQ.MinListing.Region.Price} gil\n" +
                                               $"World ID: {foundWorld} \n\n";
                            }
                            if (itemResult.NQ.RecentPurchase.DC != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.NQ.RecentPurchase.DC.WorldId.ToString(), worldIdMapJSON);
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"Recent Purchase: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {itemResult.NQ.RecentPurchase.DC.Price} gil \n" +
                                               $"Timestamp: {itemResult.NQ.RecentPurchase.DC.Timestamp.ToDateTime()} \n" +
                                               $" World ID: {foundWorld} \n";
                            }
                            if (itemResult.NQ.RecentPurchase.Region != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.NQ.RecentPurchase.Region.WorldId.ToString(), worldIdMapJSON);
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {itemResult.NQ.RecentPurchase.Region.Price} gil \n" +
                                               $"Timestamp: {itemResult.NQ.RecentPurchase.Region.Timestamp.ToDateTime()} \n" +
                                               $"World ID: {foundWorld} \n\n";
                            }
                            if (itemResult.NQ.AverageSalePrice.DC != null)
                            {
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"Average Sale Price (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {Convert.ToInt64(itemResult.NQ.AverageSalePrice.DC.Price)} gil \n";
                            }
                            if (itemResult.NQ.AverageSalePrice.Region != null)
                            {
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {Convert.ToInt64(itemResult.NQ.AverageSalePrice.Region.Price)} gil \n\n";
                            }
                            if (itemResult.NQ.DailySaleVelocity.DC != null)
                            {
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"Daily Sale Velocity (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Qty: {Convert.ToInt64(itemResult.NQ.DailySaleVelocity.DC.Quantity)} units \n";
                            }
                            if (itemResult.NQ.DailySaleVelocity.Region != null)
                            {
                                embedNQInfoMessage = embedNQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Qty: {Convert.ToInt64(itemResult.NQ.DailySaleVelocity.Region.Quantity)} units";
                            }

                            //HQ Message Setup
                            var embedHQInfoMessage = "";

                            if (itemResult.HQ.MinListing.DC != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.HQ.MinListing.DC.WorldId.ToString(), worldIdMapJSON);
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"Min Listing: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {itemResult.HQ.MinListing.DC.Price} gil \n" +
                                               $"World ID: {foundWorld} \n";
                            }
                            if (itemResult.HQ.MinListing.Region != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.HQ.MinListing.Region.WorldId.ToString(), worldIdMapJSON);
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {itemResult.HQ.MinListing.Region.Price} gil \n" +
                                               $"World ID: {foundWorld} \n\n";
                            }
                            if (itemResult.HQ.RecentPurchase.DC != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.HQ.RecentPurchase.DC.WorldId.ToString(), worldIdMapJSON);
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"Recent Purchase: \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {itemResult.HQ.RecentPurchase.DC.Price} gil \n" +
                                               $"Timestamp: {itemResult.HQ.RecentPurchase.DC.Timestamp.ToDateTime()} \n" +
                                               $"World ID: {foundWorld} \n";
                            }
                            if (itemResult.HQ.RecentPurchase.Region != null)
                            {
                                var foundWorld = JsonSearcher.FindNameByID(itemResult.HQ.RecentPurchase.Region.WorldId.ToString(), worldIdMapJSON);
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {itemResult.HQ.RecentPurchase.Region.Price} gil \n" +
                                               $"Timestamp: {itemResult.HQ.RecentPurchase.Region.Timestamp.ToDateTime()} \n" +
                                               $"World ID: {foundWorld} \n\n";
                            }
                            if (itemResult.HQ.AverageSalePrice.DC != null)
                            {
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"Average Sale Price (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Price: {Convert.ToInt64(itemResult.HQ.AverageSalePrice.DC.Price)} gil \n";
                            }
                            if (itemResult.HQ.AverageSalePrice.Region != null)
                            {
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Price: {Convert.ToInt64(itemResult.HQ.AverageSalePrice.Region.Price)} gil \n\n";
                            }
                            if (itemResult.HQ.DailySaleVelocity.DC != null)
                            {
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"Daily Sale Velocity (Last 4 days): \n" +
                                               $"--- In DC --- \n" +
                                               $"Qty: {Convert.ToInt64(itemResult.HQ.DailySaleVelocity.DC.Quantity)} units \n";
                            }
                            if (itemResult.HQ.DailySaleVelocity.Region != null)
                            {
                                embedHQInfoMessage = embedHQInfoMessage +
                                               $"--- In Region --- \n" +
                                               $"Qty: {Convert.ToInt64(itemResult.HQ.DailySaleVelocity.Region.Quantity)} units";
                            }

                            var priceCheckHeaderEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"Price Check: {item}",
                                Description = embedPCHeaderMessage
                            };

                            await ctx.Channel.SendMessageAsync(embed: priceCheckHeaderEmbedMessageFinal);

                            var priceCheckNQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{item} - NQ Information",
                                Description = embedNQInfoMessage
                            };

                            await ctx.Channel.SendMessageAsync(embed: priceCheckNQEmbedMessageFinal);

                            var priceCheckHQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{item} - HQ Information",
                                Description = embedHQInfoMessage
                            };

                            await ctx.Channel.SendMessageAsync(embed: priceCheckHQEmbedMessageFinal);
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
                var failedMessage = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0x2F5889),
                    Title = "Requested Item for Price Check",
                    Description = $"Requested item ({item}) was NOT found in data file"
                };

                await ctx.Channel.SendMessageAsync(embed: failedMessage);
            }

        }
    }
}
