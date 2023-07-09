using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate_GM_Screen.Resources
{
    [Table("Resources")]
    public class Resource
    {                
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }        
        public string Name { get; set; }
        public string Address { get; set; }
        public int FolderID { get; set; } = -1;
        public override string ToString()
        {
            return Name;
        }        
    }
}
