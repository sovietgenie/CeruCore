namespace CeruCore.Objects
{
    internal class GameItem
    {
        public GameItem(int inGameID, string name)
        {
            GameID = inGameID;
            Name = name;
        }

        public int GameID { get; set; }
        public string Name { get; set; }
    }
}
