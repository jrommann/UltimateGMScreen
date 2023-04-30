using SQLite;

namespace Ultimate_GM_Screen.Dice
{
    [Table("Dice")]
    class DiceDB
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Expression { get; set; }
    }
}
