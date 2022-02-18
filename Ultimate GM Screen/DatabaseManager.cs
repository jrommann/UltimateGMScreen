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

        static DatabaseManager _instance = null;
        static SQLiteConnection _db;

        DatabaseManager(string filepath)
        {
            _db = new SQLiteConnection(filepath);
            _db.CreateTable<MagicItem>();
            _db.CreateTable<Entity>();
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

                if (item is MagicItem)
                    OnMagicItemsChanged?.Invoke();
                else if (item is Entity)
                    OnEntitiesChanged?.Invoke();
            }
            catch { return false; }

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

            var list = _db.Query<Entity>("SELECT * FROM Entities");
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return list;
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
