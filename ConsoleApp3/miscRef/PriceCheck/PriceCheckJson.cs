using Newtonsoft.Json;

namespace CeruCore.miscRef.PriceCheck
{
    internal class PriceCheckJson
    {
        public class ApiResponse
        {
            [JsonProperty("results")]
            public List<Result> Results { get; set; }

            [JsonProperty("failedItems")]
            public List<object> FailedItems { get; set; } // Assuming failedItems is an empty array
        }

        public class Result
        {
            [JsonProperty("itemId")]
            public int ItemId { get; set; }

            [JsonProperty("nq")]
            public NQ NQ { get; set; }

            [JsonProperty("hq")]
            public HQ HQ { get; set; }

            [JsonProperty("worldUploadTimes")]
            public List<WorldUploadTime> WorldUploadTimes { get; set; }
        }

        public class NQ
        {
            [JsonProperty("minListing")]
            public Listing MinListing { get; set; }

            [JsonProperty("recentPurchase")]
            public Purchase RecentPurchase { get; set; }

            [JsonProperty("averageSalePrice")]
            public AveragePrice AverageSalePrice { get; set; }

            [JsonProperty("dailySaleVelocity")]
            public DailySaleVelocity DailySaleVelocity { get; set; }
        }

        public class HQ
        {
            [JsonProperty("minListing")]
            public Listing MinListing { get; set; }

            [JsonProperty("recentPurchase")]
            public Purchase RecentPurchase { get; set; }

            [JsonProperty("averageSalePrice")]
            public AveragePrice AverageSalePrice { get; set; }

            [JsonProperty("dailySaleVelocity")]
            public DailySaleVelocity DailySaleVelocity { get; set; }
        }

        public class Listing
        {
            [JsonProperty("dc")]
            public MarketInfo DC { get; set; }

            [JsonProperty("region")]
            public MarketInfo Region { get; set; }
        }

        public class MarketInfo
        {
            [JsonProperty("price")]
            public int Price { get; set; }

            [JsonProperty("worldId")]
            public int? WorldId { get; set; } // WorldId is optional in some cases
        }

        public class Purchase
        {
            [JsonProperty("dc")]
            public PurchaseInfo DC { get; set; }

            [JsonProperty("region")]
            public PurchaseInfo Region { get; set; }
        }

        public class PurchaseInfo
        {
            [JsonProperty("price")]
            public int Price { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("worldId")]
            public int? WorldId { get; set; }
        }

        public class AveragePrice
        {
            [JsonProperty("dc")]
            public PriceInfo DC { get; set; }

            [JsonProperty("region")]
            public PriceInfo Region { get; set; }
        }

        public class PriceInfo
        {
            [JsonProperty("price")]
            public double? Price { get; set; } // Price can be null if not provided
        }

        public class DailySaleVelocity
        {
            [JsonProperty("dc")]
            public VelocityInfo DC { get; set; }

            [JsonProperty("region")]
            public VelocityInfo Region { get; set; }
        }

        public class VelocityInfo
        {
            [JsonProperty("quantity")]
            public double? Quantity { get; set; } // Quantity can be null if not provided
        }

        public class WorldUploadTime
        {
            [JsonProperty("worldId")]
            public int WorldId { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }
        }
        public class World
        {
            [JsonProperty("id")]
            public int WorldId { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
