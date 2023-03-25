using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate_GM_Screen.Entities
{
    [Table("EntityRevisions")]
    public class EntityRevision
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int EntityID { get; set; }       
        public string Details { get; set; }

        public EntityRevision() { }

        public EntityRevision(Entity ent)
        {
            Date = DateTime.Now;
            EntityID = ent.ID;           
            Details = ent.Details;
        }

        public override string ToString()
        {
            return Date.ToString();
        }
    }
}
