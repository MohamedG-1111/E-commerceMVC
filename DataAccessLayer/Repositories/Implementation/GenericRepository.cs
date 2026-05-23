using System.Linq.Expressions;
using DataAccessLayer.Repositories.Interfaces;
using DataAcessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbSet.AddRangeAsync(entities);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);



        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }
            return await _dbSet.CountAsync(predicate);
        }

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public void DeleteRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<T> GetAsQuery(bool noTracking = true)
        {
            var query = _dbSet.AsQueryable();
            return noTracking ? query.AsNoTracking() : query;
        }

        public async Task<T?> FindAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
