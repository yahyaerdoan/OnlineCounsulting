using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.Linq.Expressions;

namespace OnlineConsulting.DataAccess.Concretions.GenericRepositories;

public class GenericRepository<T>(OnlineConsultingDbContext context) : IGenericRepository<T> where T : BaseEntity
{
    protected readonly OnlineConsultingDbContext _context = context;
    public DbSet<T> Entity => _context.Set<T>();
    // Rastgele Guid yerine zaman sıralı (UUID v7) Id üretir; SQL Server'da clustered index
    // üzerindeki sayfa bölünmesi/fragmentasyonu önler. AutoMapper'ın Create...Dto -> Entity
    // eşlemesi Id'yi hep Guid.Empty bırakır, burada onu doldururuz.
    protected static void EnsureSequentialId(T entity)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.CreateVersion7();
    }

    public async Task<bool> AddAsync(T entity)
    {
        EnsureSequentialId(entity);
        var entityEntry = await Entity.AddAsync(entity);
        return entityEntry.State == EntityState.Added;
    }

    public async Task<Guid> CreateAndReturnIdAsync(T entity)
    {
        EnsureSequentialId(entity);
        _ = await Entity.AddAsync(entity);
        return entity.Id;
    }

    public async Task<bool> AddRangeAsync(List<T> datas)
    {
        datas.ForEach(EnsureSequentialId);
        await Entity.AddRangeAsync(datas);
        return true;
    }
    public IQueryable<T> GetAll(bool traking = true, bool? status = true)
    {
        var query = Entity.AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        return query;
    }
    public async Task<T?> GetByIdAsync(string id, bool traking = true, bool? status = true)
    {
        var query = Entity.AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id) && data.Status == status);
    }
    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> expression, bool tracking = true, bool? status = true)
    {
        var query = Entity.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        // Get entity or null if not found
        var entity = await query.SingleOrDefaultAsync(expression);

        // Explicitly handle null scenario first
        if (entity is null)
            return null;

        // Then safely check Status property (if applicable)
        if (status.HasValue && entity.Status != status.Value)
            return null;

        return entity;
    }
    public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool traking = true, bool? status = true)
    {
        var query = Entity.AsQueryable();
        if (!traking)
            query = query.AsNoTracking();
        return query.Where(expression).Where(data => data.Status == status);
    }
    public async Task<bool> RemoveAsync(T entity, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            entity.Status = false;
            var entityEntry = await Task.Run(() => Entity.Update(entity));
            return true;
        }
        else
        {
            var entityEntry = await Task.Run(() => Entity.Remove(entity));
            return entityEntry.State == EntityState.Deleted;
        }

    }
    public async Task<bool> RemoveByIdAsync(string id, bool isSoftDelete = true)
    {
        var entity = await Entity.FindAsync(id);
        if (entity is null)
            return false;
        return await RemoveAsync(entity, isSoftDelete);
    }
    public bool RemoveRange(List<T> datas, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            foreach (var data in datas)
            {
                data.Status = false;
                _ = Entity.Update(data);
            }
        }
        else
        {
            Entity.RemoveRange(datas);

        }
        return true;
    }
    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    public async Task<bool> UpdateAsync(T entity)
    {
        // Asenkron operasyon simülasyonu
        await Task.Yield();

        // Check if the entity is already tracked
        var trackedEntity = _context.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity.Id == entity.Id);
        // Detach the already tracked entity
        trackedEntity?.State = EntityState.Detached;
        // Attach the entity and mark it as modified
        _context.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        return true;
    }
    public bool UpdateRange(List<T> datas)
    {

        foreach (var data in datas)
        {
            // Check if the entity is already tracked
            var trackedEntity = _context.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity.Id == data.Id);
            // Detach the already tracked entity
            trackedEntity?.State = EntityState.Detached;
            // Attach the entity and mark it as modified
            _context.Attach(data);
            _context.Entry(data).State = EntityState.Modified;
        }
        return true;
    }

    public async Task<bool> RemoveByIdAsync(Guid id, bool isSoftDelete = true)
    {
        var entity = await Entity.FindAsync(id);
        if (entity is null)
            return false;
        return await RemoveAsync(entity, isSoftDelete);
    }
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool tracking = true, bool? status = true)
    {
        var query = Entity.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query.FirstOrDefaultAsync(expression);
    }
}
