﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate_GM_Screen.Entities
{
    [Table("EntityRelationships")]
    public class EntityRelationship
    {
        [Unique]
        public int ParentID { get; set; }
        [Unique]
        public int ChildID { get; set; }
        public string Description { get; set; }

        [Ignore]
        public Entity Parent { get { return DatabaseManager.Entity_FromID(ParentID); } }
        [Ignore]
        public Entity Child { get { return DatabaseManager.Entity_FromID(ChildID); } }
    }
}
