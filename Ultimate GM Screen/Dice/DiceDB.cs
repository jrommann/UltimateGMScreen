using SQLite;

namespace Ultimate_GM_Screen.Dice
{
    [Table("Dice")]
    public class DiceDB
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Expression { get; set; }
        public string Name { get; set; }
    }
}
