using SQLite;

namespace Ultimate_GM_Screen
{
    [Table("Settings")]
    class SettingsDB
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string TableURL { get; set; }
    }
}
