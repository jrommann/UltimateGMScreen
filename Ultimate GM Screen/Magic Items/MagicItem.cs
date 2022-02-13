using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate_GM_Screen.Magic_Items
{    
    [Table("MagicItems")]
    public class MagicItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [Indexed]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public int PageNumber { get; set; }
        public string Tags { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string ToStringFull()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Name);
            sb.AppendLine();
            sb.AppendLine(Description);
            sb.AppendLine();
            sb.Append("Source: ").Append(Source).Append(" pg").Append(PageNumber);
            return sb.ToString();
        }
    }
}
