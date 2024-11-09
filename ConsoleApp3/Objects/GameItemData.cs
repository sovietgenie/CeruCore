using Lumina.Excel.Sheets;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Buffers;

namespace CeruCore.Objects
{
    internal class GameItemData
    {
        private static string gameDataFilePath = "C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack";

        private static DirectoryInfo? gameDataDirectoryInfo = new DirectoryInfo(gameDataFilePath);

        public static BsonDocument GetGameItemsFromGameData()
        {
            //  If the gameDataDirectoryInfo is not null and the directory defined exists on the host machine
            if (gameDataDirectoryInfo != null && gameDataDirectoryInfo.Exists)
            {
                Console.WriteLine("gameDataDirectoryInfo is valid");
                var lumina = new Lumina.GameData(gameDataFilePath);     // Creates a new Lumina object from the defined gameDateFilePath
                Console.WriteLine("Lumina object created");
                var itemExcelSheet = lumina.GetExcelSheet<Item>();      // Creates an object to store the Item Excel Sheet from the Lumina library
                Console.WriteLine("itemExcelSheet object created");
                BsonDocument itemNameMap = new BsonDocument();     // Creating a list object to store the ID/Name pairs for the Items

                foreach (var itemRow in itemExcelSheet)     //  For each row scraped from the game files corresponding to an 'item' designation
                {
                    if (itemRow.IsUntradable == false && itemRow.Name != "")      // If the item is tradeable
                    {
                        Console.WriteLine($"{itemRow.RowId}" + ":" + $"{itemRow.Name}");
                        itemNameMap[itemRow.RowId.ToString()] = BsonValue.Create(itemRow.Name.ToString());
                    }
                }
                Console.WriteLine("itemNameMap object created");
                return itemNameMap;     // Return the itemNameMap object  
            }
            else
            {
                Console.WriteLine("gameDataDirectoryInfo is null; gameDataFilePath could not be located");
                var nullMap = new BsonDocument();    // create a null list object
                return nullMap;     // return null list
            }
        }

        public static async void pushGameItemsToDB(BsonDocument gameItems)
        {
            var database = Program.MongoClient.GetDatabase("Ceru");

            var collection = database.GetCollection<BsonDocument>("GameItemData");

            var jsonObject = new BsonDocument();

            try
            {
                collection.InsertOne(jsonObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

       
    }
}
