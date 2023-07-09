using SQLite;
using Ultimate_GM_Screen;

namespace Ultimate_GM_Screen.Entities
{
    [Table("Entities")]
    public class Entity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }        
        public string Name { get; set; }
        public string Details { get; set; }
        public string Tags { get; set; }
        public bool Archived { get; set; } = false;
        public int FolderID { get; set; } = -1;
        public override string ToString()
        {
            return Name;
        }

        string _folderPath = string.Empty;

        [Ignore]
        public string FolderPath
        {
            get 
            {
                if (string.IsNullOrEmpty(_folderPath))
                {
                    var name = DatabaseManager.Folder_Fullpath(FolderID);
                    if (string.IsNullOrEmpty(name))
                        _folderPath = ToString();
                    else
                        _folderPath = name + "/" + ToString();
                }

                return _folderPath;
            }
        }
    }
    public class NoteListing
    {
        public int ID { get; set; }
        public int FolderID { get; set; }        
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        string _folderPath = string.Empty;        
        public string FolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_folderPath))
                {
                    var name = DatabaseManager.Folder_Fullpath(FolderID);
                    if (string.IsNullOrEmpty(name))
                        _folderPath = ToString();
                    else
                        _folderPath = name + "/" + ToString();
                }

                return _folderPath;
            }
        }
    }
}


