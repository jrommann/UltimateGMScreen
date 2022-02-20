using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultimate_GM_Screen.Magic_Items;
using Ultimate_GM_Screen.Entities;

namespace Ultimate_GM_Screen
{
    class DatabaseManager
    {
        public delegate void Event_MagicItemsChanged(MagicItem specificItem=null);
        static public event Event_MagicItemsChanged OnMagicItemsChanged;

        public delegate void Event_EntitiesChanged(Entity specificItem = null);
        static public event Event_EntitiesChanged OnEntitiesChanged;

        public delegate void Event_RelationshipsChanged(EntityRelationship specificItem = null);
        static public event Event_RelationshipsChanged OnRelationshipsChanged;

        static DatabaseManager _instance = null;
        static SQLiteConnection _db;

        DatabaseManager(string filepath)
        {
            _db = new SQLiteConnection(filepath);
            _db.CreateTable<MagicItem>();
            _db.CreateTable<Entity>();
            _db.CreateTable<EntityRelationship>();
        }

        #region -> public methods
        static public DatabaseManager Open(string filepath)
        {
            _instance = new DatabaseManager(filepath);
            return _instance;
        }

        static public void Close(string filepath)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            _db.Close();
            _db = null;
        }

        static public bool Add(object item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Insert(item);

                if (item is MagicItem)
                    OnMagicItemsChanged?.Invoke(item as MagicItem);
                else if (item is Entity)
                    OnEntitiesChanged?.Invoke(item as Entity);
                else if (item is EntityRelationship)
                    OnRelationshipsChanged?.Invoke(item as EntityRelationship);
            }
            catch { return false; }

            return true;
        }

        static public bool Delete(object item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Delete(item);
                if (item is Entity)
                    Delete_Relationships(item as Entity);

                if (item is MagicItem)
                    OnMagicItemsChanged?.Invoke();
                else if (item is Entity)
                    OnEntitiesChanged?.Invoke();
                else if (item is EntityRelationship)
                    OnRelationshipsChanged?.Invoke();
            }
            catch { return false; }

            return true;
        }

        static public void Delete_Relationships(Entity e)
        {
            var list = EntityRelationship_GetAll(e.ID);
            foreach (var r in list)
                Delete(r);
        }

        static public bool Update(object item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Update(item);

                if (item is MagicItem)
                    OnMagicItemsChanged?.Invoke(item as MagicItem);
                else if (item is Entity)
                    OnEntitiesChanged?.Invoke(item as Entity);
                else if (item is EntityRelationship)
                    OnRelationshipsChanged?.Invoke(item as EntityRelationship);
            }
            catch { return false; }

            return true;
        }
        #endregion

        #region -> entity specific
        static public List<Entity> Entiies_GetAll()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            var list = _db.Table<Entity>().ToList();
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return list;
        }

        static public Entity Entity_FromID(int id)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Find<Entity>(x => x.ID == id);
        }

        static public List<EntityRelationship> EntityRelationship_GetAll(int parentID)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<EntityRelationship>().Where(t => t.ParentID == parentID).ToList();
        }
        #endregion

        #region -> magic item specific
        static public List<MagicItem> MagicItem_GetAll()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");
            
            var list = _db.Query<MagicItem>("SELECT * FROM MagicItems");
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return list;
        }
        #endregion
    }
}
