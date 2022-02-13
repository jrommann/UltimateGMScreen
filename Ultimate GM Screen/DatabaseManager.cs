using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultimate_GM_Screen.Magic_Items;

namespace Ultimate_GM_Screen
{
    class DatabaseManager
    {
        public delegate void Event_MagicItemsChanged(MagicItem specificItem=null);
        static public event Event_MagicItemsChanged OnMagicItemsChanged;

        static DatabaseManager _instance = null;
        static SQLiteConnection _db;

        DatabaseManager(string filepath)
        {
            _db = new SQLiteConnection(filepath);
            _db.CreateTable<MagicItem>();
        }

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

        #region -> magic items
        static public List<MagicItem> MagicItem_GetAll()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");
            
            var list = _db.Query<MagicItem>("SELECT * FROM MagicItems");
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return list;
        }

        static public bool MagicItem_Add(MagicItem item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try 
            { 
                _db.Insert(item);

                OnMagicItemsChanged?.Invoke(item);
            }
            catch { return false; }

            return true;
        }

        static public bool MagicItem_Delete(MagicItem item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Delete(item);

                OnMagicItemsChanged?.Invoke(item);
            }
            catch { return false; }

            return true;
        }

        static public bool MagicItem_Update(MagicItem item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Update(item);

                OnMagicItemsChanged?.Invoke(item);
            }
            catch { return false; }

            return true;
        }
        #endregion
    }
}
