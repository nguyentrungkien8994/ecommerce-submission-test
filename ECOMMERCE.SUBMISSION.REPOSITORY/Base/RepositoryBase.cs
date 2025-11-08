using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECOMMERCE.SUBMISSION.REPOSITORY;

public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
{
    DbSet<T> dbSet;
    EcommerceDataContext _dbContext;
    public DbSet<T> DBSet
    {
        get
        {
            return dbSet;
        }
    }
    public RepositoryBase(EcommerceDataContext dbContext)
    {
        _dbContext = dbContext;
        dbSet = dbContext.Set<T>();
    }

    public async Task<int> Delete(Guid id)
    {
        T? obj = dbSet.Find(id);
        if (obj == null)
        {
            return 0;
        }
        dbSet?.Remove(obj);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<T?> GetAsync(Guid id)
    {
        return await dbSet.FindAsync(id);
    }

    public async Task<int> Insert(T entity)
    {
        dbSet.Add(entity);
        return await _dbContext.SaveChangesAsync();
    }
    public async Task<int> InsertRange(T[] entities)
    {
        dbSet.AddRange(entities);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> Update(T entity)
    {
        dbSet.Update(entity);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<T>> Search(Expression<Func<T, bool>> expression)
    {
        return await dbSet.AsNoTracking().Where(expression).ToListAsync();
    }

    public async Task<T?> SearchOne(Expression<Func<T, bool>> expression)
    {
        return await dbSet.AsNoTracking().Where(expression).FirstOrDefaultAsync();
    }

    public async Task<PagingObject<T>> Paging(Expression<Func<T, bool>>? expression, int skip, int take)
    {
        int totals = 0;
        List<T> datas = new List<T>();
        if (expression == null)
            expression = (x => 1 == 1);
        totals = await dbSet.CountAsync(expression);
        datas = await dbSet.Where(expression).OrderByDescending(x => x.updated_at).Skip(skip).Take(take).ToListAsync();

        //if (expression != null)
        //{
        //    totals = await dbSet.CountAsync(expression);
        //    datas = await dbSet.Where(expression).OrderByDescending(x => x.updated_at).Skip(skip).Take(take).ToListAsync();
        //}
        //else
        //{
        //    totals = await dbSet.CountAsync();
        //    datas = await dbSet.OrderByDescending(x => x.updated_at).Skip(skip).Take(take).ToListAsync();
        //}
        return new PagingObject<T>() { Data = datas, Skip = skip, Take = take, TotalCount = totals };
    }

    public async Task<int> Count(Expression<Func<T, bool>> expression)
    {
        return await dbSet.CountAsync(expression);
    }

    public async Task<int> UpdateRange(T[] entities)
    {
        dbSet.UpdateRange(entities);
        await _dbContext.SaveChangesAsync();
        return entities.Length;
    }

    public async Task<PagingObject<T>> Paging<TKey>(Expression<Func<T, bool>> expression, int skip, int take, Expression<Func<T, TKey>>? sort = null)
    {
        int totals = 0;
        List<T> datas = new List<T>();
        if (expression == null)
        {
            expression = (x => 1 == 1);
        }
        totals = await dbSet.CountAsync(expression);
        if (sort != null)
            datas = await dbSet.Where(expression).OrderByDescending(sort).Skip(skip).Take(take).ToListAsync();
        else
            datas = await dbSet.Where(expression).OrderByDescending(x=>x.updated_at).Skip(skip).Take(take).ToListAsync();
        //if (expression != null)
        //{
        //    totals = await dbSet.CountAsync(expression);
        //    datas = await dbSet.Where(expression).OrderByDescending(sort).Skip(skip).Take(take).ToListAsync();
        //}
        //else
        //{
        //    totals = await dbSet.CountAsync();
        //    datas = await dbSet.OrderByDescending(sort).Skip(skip).Take(take).ToListAsync();
        //}
        return new PagingObject<T>() { Data = datas, Skip = skip, Take = take, TotalCount = totals };
    }

    public async Task<int> DeleteRange(T[] entities)
    {
        dbSet.RemoveRange(entities);
        return await _dbContext.SaveChangesAsync();
    }

    public Task<int> ExcuteNoneQueryBySqlRaw(string sqlText, params object[] objects)
    {
        throw new NotImplementedException();
    }
    public async Task<TResult?> SearchOnceBySqlRaw<TResult>(string sqlText, params object[] objects)
    {
        List<TResult> results = await _dbContext.Database.SqlQueryRaw<TResult>(sqlText, objects).ToListAsync();
        return results.FirstOrDefault();
    }
    public async Task<List<TResult>> SearchBySqlRaw<TResult>(string sqlText, params object[] objects)
    {
        return await _dbContext.Database.SqlQueryRaw<TResult>(sqlText, objects).ToListAsync();
    }
}
