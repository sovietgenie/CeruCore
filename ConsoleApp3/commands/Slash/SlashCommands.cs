using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Epoch.net;
using Newtonsoft.Json;
using static CeruCore.miscRef.PriceCheck.PriceCheckJson;

namespace CeruCore.commands.Slash
{
    internal class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("pricecheck", "Returns available NQ/HQ data from Universalis.app for the specified item")]

        public async Task PriceCheckSlashCommand(InteractionContext ctx, [Option("Item","The name ofw the item you want to price check")] string item)
        {
            await ctx.DeferAsync();
            DiscordWebhookBuilder webhookBuilder = new DiscordWebhookBuilder();

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

                await ctx.EditResponseAsync(webhookBuilder.AddEmbed(successMessage));

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

                            await ctx.EditResponseAsync(webhookBuilder.AddEmbed(priceCheckHeaderEmbedMessageFinal));

                            var priceCheckNQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{item} - NQ Information",
                                Description = embedNQInfoMessage
                            };

                            await ctx.EditResponseAsync(webhookBuilder.AddEmbed(priceCheckNQEmbedMessageFinal));

                            var priceCheckHQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{item} - HQ Information",
                                Description = embedHQInfoMessage
                            };

                            await ctx.EditResponseAsync(webhookBuilder.AddEmbed(priceCheckHQEmbedMessageFinal));
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

                await ctx.EditResponseAsync(webhookBuilder.AddEmbed(failedMessage));
            }
        }
    }
}
