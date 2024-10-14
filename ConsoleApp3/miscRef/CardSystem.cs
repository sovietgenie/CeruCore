namespace CeruCore.miscRef
{
    internal class CardSystem
    {
        private int[] CardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private string[] Suits = { "Clubs", "Spades", "Diamonds", "Hearts" };

        public int SelectedNumber { get; set; }
        public string SelectedNumberAsString { get; set; }
        public string SelectedCard { get; set; }

        public CardSystem()
        {
            var Random = new Random();
            int NumberIndex = Random.Next(0, CardNumbers.Length - 1);
            int SuitIndex = Random.Next(0, Suits.Length - 1);

            this.SelectedNumber = CardNumbers[NumberIndex];
            switch (SelectedNumber)
            {
                case 1:
                    this.SelectedNumberAsString = "Two";
                    break;
                case 2:
                    this.SelectedNumberAsString = "Three";
                    break;
                case 3:
                    this.SelectedNumberAsString = "Four";
                    break;
                case 4:
                    this.SelectedNumberAsString = "Five";
                    break;
                case 5:
                    this.SelectedNumberAsString = "Six";
                    break;
                case 6:
                    this.SelectedNumberAsString = "Seven";
                    break;
                case 7:
                    this.SelectedNumberAsString = "Eight";
                    break;
                case 8:
                    this.SelectedNumberAsString = "Nine";
                    break;
                case 9:
                    this.SelectedNumberAsString = "Ten";
                    break;
                case 10:
                    this.SelectedNumberAsString = "Jack";
                    break;
                case 11:
                    this.SelectedNumberAsString = "Queen";
                    break;
                case 12:
                    this.SelectedNumberAsString = "King";
                    break;
                case 13:
                    this.SelectedNumberAsString = "Ace";
                    break;
                default:
                    break;
            }
            this.SelectedCard = $"{SelectedNumberAsString} of {Suits[SuitIndex]}";
        }
    }
}
