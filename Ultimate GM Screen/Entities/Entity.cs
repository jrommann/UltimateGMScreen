﻿using SQLite;

namespace Ultimate_GM_Screen.Entities
{
    [Table("Entities")]
    public class Entity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Tags { get; set; }
        public bool Archived { get; set; } = false;
        public override string ToString()
        {
            return Path + Name;
        }
    }
}
