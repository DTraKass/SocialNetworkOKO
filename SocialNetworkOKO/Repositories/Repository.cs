using Microsoft.EntityFrameworkCore;
using SocialNetworkOKO.DbContext;
using static SocialNetworkOKO.Repositories.IRepository;

namespace SocialNetworkOKO.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _db;

        public DbSet<T> Set
        {
            get;
            private set;
        }

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            var set = _db.Set<T>();
            set.Load();

            Set = set;
        }

        public void Create(T item)
        {
            Set.Add(item);
            _db.SaveChanges();
        }

        public void Delete(T item)
        {
            Set.Remove(item);
            _db.SaveChanges();
        }

        public T Get(int id)
        {
            return Set.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Set;
        }

        public void Update(T item)
        {
            Set.Update(item);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
