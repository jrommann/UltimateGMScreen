using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate_GM_Screen.Folders
{
    public enum FolderType
    {
        Note,
        Resource,
    }

    [Table("Folders")]
    public class FolderEntry
    {
        public const int NO_PARENT_FOLDER = -1;

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }            
        public string Name { get; set; }
        public int ParentID { get; set; } = NO_PARENT_FOLDER;
        public FolderType Type { get; set; }
        public bool IsExpanded { get; set; } = true;

        string _fullPath = string.Empty;
        [Ignore]
        public string Fullpath 
        { 
            get 
            { 
                if(string.IsNullOrEmpty(_fullPath))
                    _fullPath = DatabaseManager.Folder_Fullpath(ID);

                return _fullPath;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
