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
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }            
        public string Name { get; set; }
        public int ParentID { get; set; } = -1;
        public FolderType Type { get; set; }
        public bool IsExpanded { get; set; } = true;

        public override string ToString()
        {
            return Name;
        }
    }
}
