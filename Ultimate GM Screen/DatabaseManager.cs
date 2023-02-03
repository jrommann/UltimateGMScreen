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
using Ultimate_GM_Screen.Folders;

namespace Ultimate_GM_Screen
{
    class DatabaseManager
    {
        public delegate void Event_MagicItemsChanged(MagicItem specificItem=null, bool pathChanged=false);
        static public event Event_MagicItemsChanged OnMagicItemsChanged;

        public delegate void Event_EntitiesChanged(Entity specificItem = null, bool pathChanged=false);
        static public event Event_EntitiesChanged OnEntitiesChanged;

        public delegate void Event_RelationshipsChanged(EntityRelationship specificItem = null);
        static public event Event_RelationshipsChanged OnRelationshipsChanged;

        public delegate void Event_ResourcesChanged(Resource specificItem = null, bool pathChanged=false);
        static public event Event_ResourcesChanged OnResourcesChanged;

        public delegate void Event_RevisionsChanged();
        static public event Event_RevisionsChanged OnRevisionsChanged;

        public delegate void Event_FoldersChanged();
        static public event Event_FoldersChanged OnFoldersChanged;

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
            _db.CreateTable<Folders.FolderEntry>();

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
                TriggerEvents(item, true, true);
                
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

                TriggerEvents(item, false, true);
            }
            catch(SystemException x)
            { 
                MessageBox.Show(x.ToString());
                return false;
            }

            return true;
        }

        static public bool Update(object item, bool pathChanged)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            try
            {
                _db.Update(item); 
                TriggerEvents(item, true, pathChanged);              
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

        static void TriggerEvents(object item, bool saved = true, bool isNew=false)
        {
            if (item is MagicItem)
            {
                if (saved)
                    _notifier.ShowSuccess("Saved " + (item as MagicItem).Name);

                OnMagicItemsChanged?.Invoke(item as MagicItem, isNew);
            }
            else if (item is Entity)
            {
                if (saved)
                    _notifier.ShowSuccess("Saved " + (item as Entity).Name);

                OnEntitiesChanged?.Invoke(item as Entity, isNew);
            }
            else if (item is EntityRelationship)
                OnRelationshipsChanged?.Invoke(item as EntityRelationship);
            else if (item is Resource)
            {
                if (saved)
                    _notifier.ShowSuccess("Saved " + (item as Resource).Name);

                OnResourcesChanged?.Invoke(item as Resource, isNew);
            }
            else if (item is EntityRevision)
                OnRevisionsChanged?.Invoke();
            else if (item is FolderEntry && isNew)
                OnFoldersChanged?.Invoke();
        }

        #region -> resource specific
        static public List<Resource> Resource_Search(string name)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<Resource>().AsEnumerable().Where(x => x.ToString().Contains(name, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.ToString()).ToList();
        }
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
            list.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
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
        static public List<NoteListing> Entities_GetAll_Listing()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return  _db.Table<Entity>().AsEnumerable()
                .Where(x => x.Archived == false)
                .OrderBy(x => x.ToString())
                .Select(c => new NoteListing { ID = c.ID, FolderID = c.FolderID, Name = c.Name, Path = c.Path })
                .ToList();
        }
        static public List<Entity> Entities_GetAll()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            var list = _db.Table<Entity>().AsEnumerable()
                .Where(x => x.Archived == false)
                .OrderBy(x => x.ToString())
                .ToList();

            return list;
        }

        static public List<Entity> Entities_GetAll_Archieved()
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            var list = _db.Table<Entity>().AsEnumerable()
                .Where(x => x.Archived == true)
                .OrderBy(x => x.ToString())
                .ToList();

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

        static public List<Entity> Entities_Search(string name)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<Entity>().AsEnumerable()
                .Where(x => x.ToString().Contains(name, StringComparison.OrdinalIgnoreCase) || x.Tags.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.Archived == false)
                .OrderBy(x => x.ToString())                
                .ToList();
        }

        static public List<NoteListing> Entities_Search_Listing(string name)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<Entity>().AsEnumerable()
                .Where(x => x.ToString().Contains(name, StringComparison.OrdinalIgnoreCase) || x.Tags.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.Archived == false)
                .OrderBy(x => x.ToString())
                .Select(c => new NoteListing { ID = c.ID, FolderID = c.FolderID, Name = c.Name, Path = c.Path})
                .ToList();
        }

        static public List<Entity> Entities_Search_Archived(string name)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<Entity>().AsEnumerable()
                .Where(x => x.ToString().Contains(name, StringComparison.OrdinalIgnoreCase) || x.Tags.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Where(x => x.Archived == true)
                .OrderBy(x => x.ToString())
                .ToList();
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

        #region -> folder specific
        static public List<FolderEntry> Folders_GetAll(FolderType type)
        {
            if (_db == null)
                throw new Exception("Database NOT opened");

            return _db.Table<FolderEntry>().AsEnumerable()
                .Where(x => x.Type == type)
                .OrderBy(x => x.Name)
                .ToList();
        }
        #endregion
    }
}
