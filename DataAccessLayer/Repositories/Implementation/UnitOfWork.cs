using System.Collections.Concurrent;
using DataAccessLayer.Repositories.Interfaces;
using DataAcessLayer.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private ConcurrentDictionary<Type, object> Repositories { get; set; }
            = new ConcurrentDictionary<Type, object>();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await _context.Database.BeginTransactionAsync();




        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (Repositories.TryGetValue(type, out var repo))
                return (IGenericRepository<T>)repo;
            var newRepo = new GenericRepository<T>(_context);
            Repositories[type] = newRepo;
            return newRepo;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
