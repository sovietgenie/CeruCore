using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Epoch.net;
using Newtonsoft.Json;
using static CeruCore.miscRef.PriceCheck.PriceCheckJson;

namespace CeruCore.commands.Slash
{
    internal class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("PriceCheck", "Returns available NQ/HQ data from Universalis.app for the specified item")]

        public async Task PriceCheckSlashCommand(InteractionContext ctx, [Option("Item","The name of the item you want to price check")] string item)
        {
            await ctx.DeferAsync();
            DiscordWebhookBuilder WebhookBuilder = new DiscordWebhookBuilder();

            var ItemIdMapJSON = "C:\\Users\\eugen\\source\\repos\\ConsoleApp3\\ConsoleApp3\\miscRef\\PriceCheck\\gamefile\\ffxivItemIdMap.json";
            var WorldIdMapJSON = "C:\\Users\\eugen\\source\\repos\\ConsoleApp3\\ConsoleApp3\\miscRef\\PriceCheck\\gamefile\\ffxivWorldIdMap.json";

            var Result = JsonReader.FindKeyByNestedProperty(item, ItemIdMapJSON);

            if (Result != null)
            {
                var SuccessMessage = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0x2F5889),
                    Title = "Requested Item for Price Check",
                    Description = $"Requested item ({item}) was found in data file with ID: {Result}"
                };

                await ctx.EditResponseAsync(WebhookBuilder.AddEmbed(SuccessMessage));

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
                            var EmbedPCHeaderMessage = $"Item: {item} \n" +
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
                                Title = $"Price Check: {item}",
                                Description = EmbedPCHeaderMessage
                            };

                            await ctx.EditResponseAsync(WebhookBuilder.AddEmbed(PriceCheckHeaderEmbedMessageFinal));

                            var PriceCheckNQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{item} - NQ Information",
                                Description = EmbedNQInfoMessage
                            };

                            await ctx.EditResponseAsync(WebhookBuilder.AddEmbed(PriceCheckNQEmbedMessageFinal));

                            var PriceCheckHQEmbedMessageFinal = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x2F5889),
                                Title = $"{item} - HQ Information",
                                Description = EmbedHQInfoMessage
                            };

                            await ctx.EditResponseAsync(WebhookBuilder.AddEmbed(PriceCheckHQEmbedMessageFinal));
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
                    Description = $"Requested item ({item}) was NOT found in data file"
                };

                await ctx.EditResponseAsync(WebhookBuilder.AddEmbed(FailedMessage));
            }
        }
    }
}
