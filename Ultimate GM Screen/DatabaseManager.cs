using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultimate_GM_Screen.Magic_Items;
using Ultimate_GM_Screen.Entities;
using Ultimate_GM_Screen.Resources;
using System.Windows;

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

        public delegate void Event_ResourcesChanged(Resource specificItem = null);
        static public event Event_ResourcesChanged OnResourcesChanged;

        static DatabaseManager _instance = null;
        static SQLiteConnection _db;

        DatabaseManager(string filepath)
        {
            _db = new SQLiteConnection(filepath);
            _db.CreateTable<MagicItem>();
            _db.CreateTable<Entity>();
            _db.CreateTable<EntityRelationship>();
            _db.CreateTable<Resource>();
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
                else if (item is Resource)
                    OnResourcesChanged?.Invoke(item as Resource);
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
                else if (item is Resource)
                    OnResourcesChanged?.Invoke();
            }
            catch(SystemException x) { MessageBox.Show(x.ToString()); return false; }

            return true;
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
                else if (item is Resource)
                    OnResourcesChanged?.Invoke(item as Resource);
            }
            catch { return false; }

            return true;
        }
        #endregion
        static void Delete_Relationships(Entity e)
        {
            var list = EntityRelationship_GetAll(e.ID);
            foreach (var r in list)
                Delete(r);
        }

        #region -> resource specific
        static public int Resource_Count()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");
            return _db.Table<Resource>().Count();
        }
        static public List<Resource> Resources_GetAll()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            var list = _db.Table<Resource>().ToList();
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return list;
        }
        #endregion

        #region -> entity specific
        static public int Entity_Count()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");
            return _db.Table<Entity>().Count();
        }
        static public List<Entity> Entities_GetAll()
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
