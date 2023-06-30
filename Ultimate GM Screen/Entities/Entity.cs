using SQLite;

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
        public int FolderID { get; set; } = -1;
        public override string ToString()
        {
            return Path + Name;
        }
        [Ignore]
        public string FolderPath
        {
            get 
            {
                var name = DatabaseManager.Folder_Fullpath(FolderID);
                if (string.IsNullOrEmpty(name))
                    return ToString();
                else
                    return name + "/" + ToString();
            }
        }
    }
}

public class NoteListing
{
    public int ID { get; set; }
    public int FolderID { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return Path + Name;
    }
}
