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
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using System.Linq.Expressions;

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

        public delegate void Event_RevisionsChanged();
        static public event Event_RevisionsChanged OnRevisionsChanged;

        static DatabaseManager _instance = null;
        static SQLiteConnection _db;

        static Notifier _notifier;

        DatabaseManager(string filepath)
        {
            _db = new SQLiteConnection(filepath);
            _db.CreateTable<MagicItem>();
            _db.CreateTable<Entity>();
            _db.CreateTable<EntityRelationship>();
            _db.CreateTable<EntityRevision>();
            _db.CreateTable<Resource>();

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(1),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        #region -> public methods
        static public bool IsOpened { get { return _instance != null; } }

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
                TriggerEvents(item);
                
            }
            catch (System.Exception x)
            {
                MessageBox.Show(x.ToString());
                return false;
            }

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
                {
                    Delete_Relationships(item as Entity);
                    Delete_Revisions(item as Entity);
                }

                TriggerEvents(item, false);
            }
            catch(SystemException x)
            { 
                MessageBox.Show(x.ToString());
                return false;
            }

            return true;
        }

        static public bool Update(object item)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Update(item);
                TriggerEvents(item);
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
        static void Delete_Revisions(Entity entity)
        {
            var list = Entity_GetRevisions(entity.ID, 0, 0);            
            foreach (var r in list)
                Delete(r);
        }

        static void TriggerEvents(object item, bool saved = true)
        {
            if (item is MagicItem)
            {
                if (saved)
                    _notifier.ShowSuccess("Saved " + (item as MagicItem).Name);

                OnMagicItemsChanged?.Invoke(item as MagicItem);
            }
            else if (item is Entity)
            {
                if (saved)
                    _notifier.ShowSuccess("Saved " + (item as Entity).Name);

                OnEntitiesChanged?.Invoke(item as Entity);
            }
            else if (item is EntityRelationship)
                OnRelationshipsChanged?.Invoke(item as EntityRelationship);
            else if (item is Resource)
            {
                if (saved)
                    _notifier.ShowSuccess("Saved " + (item as Resource).Name);

                OnResourcesChanged?.Invoke(item as Resource);
            }
            else if (item is EntityRevision)
                OnRevisionsChanged?.Invoke();
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
        static public List<EntityRevision> Entity_GetRevisions(int entityID, int page, int pageSize)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            int skip = Math.Max(page - 1, 0) * pageSize;
            if (pageSize == 0)
                pageSize = _db.Table<EntityRevision>().Count();

            var list = _db.Table<EntityRevision>().Where(t => t.EntityID == entityID)
                .OrderByDescending(t => t.Date)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return list;
        }

        static public int Entity_RevisionCount(int entityID)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");
            return _db.Table<EntityRevision>().Count(x=>x.EntityID == entityID);
        }

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

        static public List<Entity> Entities_FindByName(string name)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<Entity>().AsEnumerable().Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Name).ToList();
        }
        #endregion

        #region -> magic item specific
        static public List<MagicItem> MagicItem_GetAll()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            var list = _db.Table<MagicItem>().ToList();
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            return list;
        }
        #endregion
    }
}
